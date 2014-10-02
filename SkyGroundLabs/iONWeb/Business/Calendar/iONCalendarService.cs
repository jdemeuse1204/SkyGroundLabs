using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iONWeb.Business.Calendar
{
	public class iONCalendarService
	{
		private iONCalendarCredentials _credentials { get; set; }

		public iONCalendarService(iONCalendarCredentials credentials)
		{
			_credentials = credentials;
		}

		public bool Find(iONCalendarEvent calendarEvent)
		{
			return true;
		}
	}
}
