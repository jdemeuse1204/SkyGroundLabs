using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iONWeb.Data
{
	public static class Defaults
	{
		public static DateTime MinDateTime
		{
			get { return Convert.ToDateTime("1/1/1900 12:00 AM"); }
		}

		public static string EmailArchitectKey { get { return "EG-AA1150422356-00709-7F7EF5F2A0750BE4955616D9A15DCE37"; } }
	}
}
