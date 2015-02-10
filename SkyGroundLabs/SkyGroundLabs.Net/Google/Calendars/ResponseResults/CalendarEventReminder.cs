namespace SkyGroundLabs.Net.Google.Calendars.ResponseResults
{
	public class CalendarEventReminder
	{
		public int Days { get; set; }
		public int Minutes { get; set; }
		public int Hours { get; set; }
		public global::Google.GData.Extensions.Reminder.ReminderMethod Type { get; set; }
	}
}
