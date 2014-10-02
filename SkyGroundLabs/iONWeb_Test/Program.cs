using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Business.Calendar;
using iONWeb.Data.Tables;
using SkyGroundLabs.Net.Google;
using SkyGroundLabs.Net.Google.Calendars;

namespace iONWeb_Test
{
	class Program
	{
		static void Main(string[] args)
		{
			GuidInsertTest();
		}

		#region Database Tests
		static void GuidInsertTest()
		{
			iONCalendarCredentials creds = new iONCalendarCredentials();
			creds.Username = "jdemeuse1204";
			creds.Password = "aiwa1122";
			creds.Server = "lin.arvixe.com";
			creds.Database = "iONWebDataStore_Live";
			creds.UserID = 1;

			iONCalendarEvent e = new iONCalendarEvent(creds);
			e.Location = "My House";
			e.Title = "Test";
			e.StartTime = DateTime.Now;
			e.EndTime = DateTime.Now;
			e.Contents = "Contents";
			e.Save();

		}
		#endregion

		#region Google Calendar Tests
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
		#endregion
	}
}
