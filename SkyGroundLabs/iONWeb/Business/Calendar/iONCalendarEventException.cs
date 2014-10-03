using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iONWeb.Business.Calendar
{
	public class iONCalendarEventException : Exception
	{
		public iONCalendarEvent Event { get; set; }

		public iONCalendarEventException(iONCalendarEvent calendarEvent, string message)
			: base(message)
		{
			Event = calendarEvent;
		}
	}
}
