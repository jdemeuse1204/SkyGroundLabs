using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Business;
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
			ClockIn();
		}

		#region Clocking Tests
		static void ClockIn()
		{
			
		}
		#endregion

		#region Database Tests
		static void CalendarServiceTest_LoadCalendar()
		{
			iONCredentials creds = new iONCredentials();
			creds.Server = "lin.arvixe.com";
			creds.Database = "iONWebDataStore_Live";
			creds.UserID = 1;

			iONCalendarService service = new iONCalendarService(creds);
			service.LazyLoad();
		}

		static void SaveCalendarTest()
		{
			iONCredentials creds = new iONCredentials();
			creds.Server = "lin.arvixe.com";
			creds.Database = "iONWebDataStore_Live";
			creds.UserID = 1;

			iONCalendar calendar = new iONCalendar(creds);
			calendar.Name = "Test";
			calendar.Description = "Test";
			calendar.Insert();

			iONCalendarEvent e = new iONCalendarEvent(creds);
			e.Location = "My House";
			e.Title = "Test";
			e.StartTime = DateTime.Now;
			e.EndTime = DateTime.Now;
			e.Contents = "Contents";
			e.Insert(calendar.CalendarID);

		}

		static void LoadTest_EventsOnCalendar()
		{
			iONCredentials creds = new iONCredentials();
			creds.Server = "lin.arvixe.com";
			creds.Database = "iONWebDataStore_Live";
			creds.UserID = 1;

			iONCalendarService service = new iONCalendarService(creds);
			var calendar = service.FindCalendarByName("test").FirstOrDefault();

			for (int i = 0; i < 10000; i++)
			{
				iONCalendarEvent e = new iONCalendarEvent(creds);
				e.Location = "My House" + i;
				e.Title = "Test" + i;
				e.StartTime = DateTime.Now;
				e.EndTime = DateTime.Now;
				e.Contents = "Contents";
				e.Insert(calendar.CalendarID);
			}
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
