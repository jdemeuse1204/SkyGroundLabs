using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iONWeb.Business.Calendar
{
	public class iONCalendarException : Exception
	{
		public iONCalendar Calendar { get; set; }

		public iONCalendarException(iONCalendar calendar, string message)
			: base(message)
		{
			Calendar = calendar;
		}
	}
}
