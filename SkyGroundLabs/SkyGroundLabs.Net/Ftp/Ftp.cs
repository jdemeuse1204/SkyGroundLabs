using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Ftp
{
	public class Ftp : IDisposable
	{
		private FtpCredentials _credentials { get; set; }

		public Ftp(FtpCredentials credentials)
		{
			_credentials = credentials;
		}

		public string GetFiles(string folder)
		{
			var ftpAddress = _credentials.FtpAddress;
			var fileString = string.Empty;
			var basePath = ftpAddress.EndsWith("/") ? ftpAddress : ftpAddress + "/";
			var request = (FtpWebRequest)WebRequest.Create(basePath + folder);

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential(_credentials.Username, _credentials.Password);

			var response = (FtpWebResponse)request.GetResponse();
			var responseStream = response.GetResponseStream();
			var reader = new StreamReader(responseStream);

			fileString += reader.ReadToEnd();

			reader.Close();

			return fileString;
		}

		public void Dispose()
		{
			_credentials = null;
		}
	}
}
