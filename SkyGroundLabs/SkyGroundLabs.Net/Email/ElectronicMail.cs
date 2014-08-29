using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAGetMail;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.Net.Email
{
	public class ElectronicMail
	{
		public ElectronicMail() { }

		public void Set(Mail mail, MailInfo mailInfo)
		{
			ReflectionManager.SetValuesWithSkip(mail, this, "To", "From", "Cc", "Bcc", "ReplyTo", "SignerCert", "Attachments", "BodyHeaders", "Headers");
			ReflectionManager.SetValuesWithSkip(mailInfo, this, "To", "From", "Cc", "Bcc", "ReplyTo", "SignerCert", "Attachments", "BodyHeaders", "Headers");
			_SetMailAddresses(mail);
		}

		public bool Deleted { get; set; }
		public int Index { get; set; }
		public bool Read { get; set; }
		public string UIDL { get; set; }
		public ElectronicMailAddress[] Bcc { get; set; }
		public ElectronicMailAddress[] Cc { get; set; }
		public ElectronicMailAddress From { get; set; }
		public MailPriority Priority { get; set; }
		public DateTime ReceivedDate { get; set; }
		public ElectronicMailAddress ReplyTo { get; set; }
		public DateTime SentDate { get; set; }
		public string Subject { get; set; }
		public ElectronicMailAddress[] To { get; set; }

		private void _SetMailAddresses(Mail mail)
		{
			this.To = _getEmailAddressList(mail.To).ToArray();
			this.Cc = _getEmailAddressList(mail.Cc).ToArray();
			this.Bcc = _getEmailAddressList(mail.Bcc).ToArray();
			this.From = new ElectronicMailAddress(mail.From);
			this.ReplyTo = new ElectronicMailAddress(mail.ReplyTo);
		}

		private List<ElectronicMailAddress> _getEmailAddressList(MailAddress[] mailAddresses)
		{
			var addresses = new List<ElectronicMailAddress>();
			foreach (var address in mailAddresses)
			{
				var emailAddress = new ElectronicMailAddress(address);
				addresses.Add(emailAddress);
			}

			return addresses;
		}
	}
}
