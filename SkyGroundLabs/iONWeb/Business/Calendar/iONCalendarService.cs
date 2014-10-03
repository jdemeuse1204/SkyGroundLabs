using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data;

namespace iONWeb.Business.Calendar
{
	public class iONCalendarService
	{
		public iONCalendarService(iONCredentials credentials)
		{
			_credentials = credentials;
		}

		#region Properties
		private iONCredentials _credentials { get; set; }

		public List<iONCalendar> _calendars { get; set; }
		public IEnumerable<iONCalendar> Calendars { get { return _calendars; } }

		public List<iONCalendarEvent> _allEvents { get; set; }
		public IEnumerable<iONCalendarEvent> AllEvents { get { return _allEvents; } }
		#endregion

		public IEnumerable<iONCalendar> FindCalendarByName(string name)
		{
			var results = new List<iONCalendar>();

			using (var context = new ServiceContext(_credentials))
			{
				foreach (var calendar in context.Calendars.Where(w => w.Name.ToUpper().Contains(name.ToUpper())))
				{
					iONCalendar cal = new iONCalendar(_credentials, calendar);
					results.Add(cal);
				}
			}

			return results;
		}

		public void LazyLoad()
		{
			_load(true);
		}

		private void _load(bool isLazyLoad)
		{
			using (var context = new ServiceContext(_credentials))
			{
				if (_calendars == null)
				{
					_calendars = new List<iONCalendar>();
				}

				// load calendars
				foreach (var calendar in context.Calendars.Where(w => w.AuthorID == _credentials.UserID))
				{
					iONCalendar cal = new iONCalendar(_credentials, calendar);

					if (!isLazyLoad)
					{
						cal.Load(calendar.ID);
					}
					else
					{
						cal.LazyLoad(calendar.ID);
					}
					
					_calendars.Add(cal);
				}

				// load all events that are parts of calendars the user did not create
				var query = (from invites in context.EventInvitees
							 join events in context.Events on invites.EventID equals events.ID
							 join calendars in context.Calendars on events.CalendarID equals calendars.ID
							 where invites.InviteeID == _credentials.UserID
							 && invites.Type == Data.Enumeration.InviteeType.Employee
							 && calendars.AuthorID != _credentials.UserID
							 select events);

				foreach (var e in query)
				{
					iONCalendarEvent calendarEvent = new iONCalendarEvent(_credentials, e);

					if (!isLazyLoad)
					{
						calendarEvent.Load(e.ID);
					}
					
					_allEvents.Add(calendarEvent);
				}
			}
		}

		public void Load()
		{
			_load(false);
		}

		// Connection to only be used with events
		private class ServiceContext : DbContext
		{
			public ServiceContext(iONCredentials credentials)
				: base(credentials.Server, credentials.Database, credentials.Username, credentials.Password)
			{

			}
		}
	}
}
