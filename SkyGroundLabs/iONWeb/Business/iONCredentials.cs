using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iONWeb.Business
{
	public class iONCredentials
	{
		public string Server { get; set; }

		public string Database { get; set; }

		public readonly string Username = "username";

		public int UserID { get; set; }

		public readonly string Password = "password";
	}
}
