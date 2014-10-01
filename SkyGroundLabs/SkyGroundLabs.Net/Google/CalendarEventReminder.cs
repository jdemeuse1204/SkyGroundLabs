using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Google
{
	public class CalendarEventReminder
	{
		public int Days { get; set; }
		public int Minutes { get; set; }
		public int Hours { get; set; }
		public global::Google.GData.Extensions.Reminder.ReminderMethod Type { get; set; }
	}
}
