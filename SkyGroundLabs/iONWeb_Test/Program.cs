using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Net.Google;
using SkyGroundLabs.Net.Google.Calendars;

namespace iONWeb_Test
{
	class Program
	{
		static void Main(string[] args)
		{
			GoogleCalendarTest_Nickbooks();
		}

		static void GoogleCalendarTest_Nickbooks()
		{
			var credentials = new GoogleCalendarCredentials()
			{
				CalendarUri = "http://www.google.com/calendar/feeds/default/private/full",
				EmailAddress = "nickbooks.public.access@gmail.com",
				Password = "aiwa1122"
			};

			var calendar = new GoogleCalendars(credentials);

			var events = calendar.All();

			foreach (var item in events)
			{
				if (item != null)
				{

				}
			}
		}

		static void GoogleCalendarTest_MyAccount()
		{
			var credentials = new GoogleCalendarCredentials()
			{
				CalendarUri = "http://www.google.com/calendar/feeds/default/private/full",
				EmailAddress = "james.demeuse@gmail.com",
				Password = "Jermaine1122"
			};

			var calendar = new GoogleCalendars(credentials);

			var calEvent = new CalendarEvent(credentials)
			{
				Contents = "Test",
				EndTime = DateTime.Now.AddMinutes(30),
				StartTime = DateTime.Now,
				Location = "My House",
				Title = "Saved from New Framework"
			};

			calEvent.Save();
			calEvent.EndTime = DateTime.Now.AddMinutes(120);
			calEvent.Save();
		}
	}
}
