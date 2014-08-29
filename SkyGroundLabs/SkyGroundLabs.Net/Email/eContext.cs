using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EAGetMail;

namespace SkyGroundLabs.Net.Email
{
	public class eContext : IDisposable
	{
		#region Properties
		protected string _emailAddress { get; set; }
		protected string _password { get; set; }
		protected string _authKey { get; set; }
		protected string _smtpServierAddress { get; set; }
		protected string _popImapServerAddress { get; set; }
		protected int _sendPort { get; set; }
		protected int _recievePort { get; set; }
		protected bool _sendSSL { get; set; }
		protected bool _retrieveSSL { get; set; }
		protected MailServer _server { get; set; }
		protected MailClient _client { get; set; }
		protected ServerProtocol _protocol { get; set; }
		protected static int _maxEmails { get; set; }
		private static object _lock = new object();
		protected static bool? _isEmailOn { get; set; }
		private int _connectionAttempts { get; set; }

		private string _emailInboxPath { get; set; }
		private string _emailFileExtension { get { return @".ieml"; } }

		private static List<ElectronicMail> _emails;
		public IEnumerable<ElectronicMail> Emails
		{
			get
			{
				// Incase _emails is being populated by the update command
				var list = new List<ElectronicMail>();
				list.AddRange(_emails);
				return list;
			}
		}
		#endregion

		#region Constructor
		public eContext(
			string account,
			string password,
			string authenticationKey,
			string SmtpServerAddress,
			string Pop3ServerAddress,
			int sendPortNumber,
			int retreivePortNumber,
			bool sendUseSSL,
			bool retreiveUseSSL,
			ServerProtocol serverProtocol,
			string inboxPath,
			bool isEmailOn = true,
			int maxEmails = 10)
		{
			_emails = new List<ElectronicMail>();
			_emailAddress = account;
			_password = password;
			_authKey = authenticationKey;
			_smtpServierAddress = SmtpServerAddress;
			_popImapServerAddress = Pop3ServerAddress;
			_sendPort = sendPortNumber;
			_recievePort = retreivePortNumber;
			_sendSSL = sendUseSSL;
			_retrieveSSL = retreiveUseSSL;
			_protocol = serverProtocol;
			_emailInboxPath = inboxPath;

			if (Pop3ServerAddress.ToUpper().Contains("GMAIL"))
			{
				_protocol = ServerProtocol.Imap4;
			}

			_server = new MailServer(
				_popImapServerAddress,
				_emailAddress,
				_password,
				_protocol);

			_server.SSLConnection = _retrieveSSL;
			_server.Port = _recievePort;

			_client = new MailClient(_authKey);

			if (!Directory.Exists(_emailInboxPath))
			{
				Directory.CreateDirectory(_emailInboxPath);
			}

			_isEmailOn = isEmailOn;
			_maxEmails = maxEmails;
		}
		#endregion

		#region Methods
		public List<MailInfo> GetReadMailInfos()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<MailInfo>();
			}

			if (Connect())
			{
				_client.GetMailInfosOption = GetMailInfosOptionType.ReadOnly;
				return _client.GetMailInfos().ToList();
			}
			else
			{
				return new List<MailInfo>();
			}
		}

		public List<MailInfo> GetUnreadMailInfos()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<MailInfo>();
			}

			if (Connect())
			{
				_client.GetMailInfosOption = GetMailInfosOptionType.NewOnly;
				return _client.GetMailInfos().ToList();
			}
			else
			{
				return new List<MailInfo>();
			}
		}

		public List<MailInfo> GetAllMailInfos()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<MailInfo>();
			}

			if (Connect())
			{
				_client.GetMailInfosOption = GetMailInfosOptionType.All;
				return _client.GetMailInfos().ToList();
			}
			else
			{
				return new List<MailInfo>();
			}
		}

		public List<Mail> GetAllMailItems(MailInfo[] infos)
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<Mail>();
			}

			if (Connect())
			{
				_client.GetMailInfosOption = GetMailInfosOptionType.All;
				return _client.u_GetMails(infos).ToList();
			}
			else
			{
				return new List<Mail>();
			}
		}

		public string GetHtmlBodyFromFile(string uidl)
		{
			var path = _emailInboxPath + CleanUIDL(uidl) + _emailFileExtension;

			var file = new FileInfo(path);
			XmlSerializer xml = new XmlSerializer(new WritableElectronicMail().GetType());

			if (file.Exists)
			{
				using (Stream s = file.OpenRead())
				{
					WritableElectronicMail m = xml.Deserialize(s) as WritableElectronicMail;
					return m.HtmlBody;
				}
			}
			return "";
		}

		public string GetTextBodyFromFile(string uidl)
		{
			var path = _emailInboxPath + CleanUIDL(uidl) + _emailFileExtension;

			var file = new FileInfo(path);
			XmlSerializer xml = new XmlSerializer(new WritableElectronicMail().GetType());

			if (file.Exists)
			{
				using (Stream s = file.OpenRead())
				{
					WritableElectronicMail m = xml.Deserialize(s) as WritableElectronicMail;
					return m.TextBody;
				}
			}
			return "";
		}

		public Mail GetMail(MailInfo info)
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return null;
			}

			if (Connect())
			{
				return _client.GetMail(info);
			}
			else
			{
				return null;
			}
		}

		public void LoadMailFromLocalDisk()
		{
			_emails = GetAllEMailFromLocal();
		}

		public List<ElectronicMail> GetRemovalEmails()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<ElectronicMail>();
			}

			var currentInbox = GetCurrentInbox();
			return _emails.Where(w => !currentInbox.Select(x => CleanUIDL(x.UIDL)).Contains(CleanUIDL(w.UIDL))).ToList();
		}

		public IEnumerable<MailInfo> GetCurrentInbox()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<MailInfo>();
			}

			return GetAllMailInfos().OrderByDescending(w => w.Index).Take(_maxEmails);
		}

		public IEnumerable<MailInfo> GetDownloadEmails()
		{
			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<MailInfo>();
			}

			var currentInbox = GetCurrentInbox();
			return currentInbox.Where(w => !_emails.Select(x => CleanUIDL(x.UIDL)).Contains(CleanUIDL(w.UIDL)));
		}

		public void Add(MailInfo info)
		{
			try
			{
				if (Connect())
				{
					var mail = _client.GetMail(info);
					var email = new ElectronicMail();
					var writableEmail = new WritableElectronicMail();
					email.Set(mail, info);
					writableEmail.Set(mail, info);

					if (!_emails.Select(w => CleanUIDL(w.UIDL)).Contains(CleanUIDL(info.UIDL)))
					{
						_emails.Add(email);
						SaveEmail(info.UIDL, writableEmail);
					}
				}
			}
			catch (Exception)
			{

			}
		}

		public void Remove(ElectronicMail email)
		{
			var path = _emailInboxPath + CleanUIDL(email.UIDL) + _emailFileExtension;

			if (File.Exists(path))
			{
				File.Delete(path);
			}
			_emails.Remove(email);
		}

		public void Update()
		{
			lock (_lock)
			{
				if (_isEmailOn == null || !_isEmailOn.Value)
				{
					return;
				}

				if (Connect())
				{
					var currentInbox = GetAllMailInfos().OrderByDescending(w => w.Index).Take(_maxEmails);
					var download = currentInbox.Where(w => !_emails.Select(x => CleanUIDL(x.UIDL)).Contains(CleanUIDL(w.UIDL)));
					var remove = _emails.Where(w => !currentInbox.Select(x => CleanUIDL(x.UIDL)).Contains(CleanUIDL(w.UIDL))).ToList();

					// Download
					foreach (var emailInfo in download)
					{
						Add(emailInfo);
					}

					// Remove
					for (int i = 0; i < remove.Count; i++)
					{
						var email = remove[i];
						var path = _emailInboxPath + CleanUIDL(email.UIDL) + _emailFileExtension;

						if (File.Exists(path))
						{
							File.Delete(path);
						}
						_emails.Remove(email);
					}
				}
			}
		}

		public List<ElectronicMail> GetAllEMailFromLocal()
		{

			if (_isEmailOn == null || !_isEmailOn.Value)
			{
				return new List<ElectronicMail>();
			}

			var allLocalMail = new List<ElectronicMail>();
			var emails = Directory.GetFiles(_emailInboxPath, "*" + _emailFileExtension, SearchOption.TopDirectoryOnly);

			try
			{
				foreach (var email in emails)
				{
					var queue = new FileInfo(email);
					XmlSerializer xml = new XmlSerializer(new WritableElectronicMail().GetType());

					try
					{
						using (Stream s = queue.OpenRead())
						{
							WritableElectronicMail m = xml.Deserialize(s) as WritableElectronicMail;
							allLocalMail.Add(m as ElectronicMail);
						}
					}
					catch (Exception)
					{
						// File is corrupt, delete it
						queue.Delete();
					}
				}
			}
			catch (Exception)
			{

				throw;
			}


			return allLocalMail;
		}

		public bool IsEmailOnLocalDrive(string uidl)
		{
			var emails = Directory.GetFiles(_emailInboxPath, "*" + CleanUIDL(uidl) + _emailFileExtension, SearchOption.TopDirectoryOnly);
			return emails.Count() > 0;
		}

		public ElectronicMail GetEmailFromLocal(string uidl)
		{
			var emails = Directory.GetFiles(_emailInboxPath, "*" + CleanUIDL(uidl) + _emailFileExtension, SearchOption.TopDirectoryOnly);

			if (emails.Count() == 0)
			{
				return null;
			}

			var queue = new FileInfo(emails[0]);
			XmlSerializer xml = new XmlSerializer(new WritableElectronicMail().GetType());

			using (Stream s = queue.OpenRead())
			{
				WritableElectronicMail m = xml.Deserialize(s) as WritableElectronicMail;
				return m as ElectronicMail;
			}
		}

		public void SaveEmail(string uidl, WritableElectronicMail email)
		{
			XmlSerializer xmls = new XmlSerializer(email.GetType());
			var path = _emailInboxPath + CleanUIDL(uidl) + _emailFileExtension;
			FileInfo file = new FileInfo(path);

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			using (Stream s = file.OpenWrite())
			{
				xmls.Serialize(s, email);
				s.Close();
			}
		}

		public bool Connect()
		{
			try
			{
				if (!_client.Connected)
				{
					if (_connectionAttempts == 0)
					{
						_client.Connect(_server);
					}
					else
					{
						_client.ReConnect();
					}
					_connectionAttempts++;
				}
				return true;
			}
			catch (Exception)
			{
				try
				{
					_client.Connect(_server);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		private string CleanUIDL(string UIDL)
		{
			foreach (var c in InvalidCharacters())
			{
				UIDL = UIDL.Replace(c.ToString(), "");
			}
			return UIDL;
		}

		private List<char> InvalidCharacters()
		{
			List<char> invalidChars = Path.GetInvalidPathChars().ToList();
			invalidChars.Add('/');
			invalidChars.Add('#');
			invalidChars.Add(Convert.ToChar("'"));
			return invalidChars;
		}

		public void Dispose()
		{
			_emailAddress = "";
			_password = "";
			_authKey = "";
			_smtpServierAddress = "";
			_popImapServerAddress = "";
			_sendPort = 0;
			_recievePort = 0;
			_sendSSL = false;
			_retrieveSSL = false;
			_server = null;
			_client = null;
		}
		#endregion
	}
}
