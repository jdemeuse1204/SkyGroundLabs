using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Ftp
{
	public class FtpCredentials
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public string FtpAddress { get; private set; }

		public FtpCredentials(string username, string password, string ftpAddress)
		{
			Username = username;
			Password = password;
			FtpAddress = ftpAddress;
		}
	}
}
