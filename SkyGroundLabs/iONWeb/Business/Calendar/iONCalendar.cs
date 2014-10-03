using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data;
using iONWeb.Data.Tables;

namespace iONWeb.Business.Calendar
{
	public class iONCalendar
	{
		#region Properties
		public Guid CalendarID { get; private set; }
		public string Description { get; set; }
		public string Name { get; set; }
		private iONCalendarCredentials _credentials { get; set; }

		public bool WasLazyLoaded { get; private set; }

		public int AuthorID { get; private set; }

		private List<CalendarInvitee> _calendarInvitees { get; set; }
		public IEnumerable<CalendarInvitee> CalendarInvitees { get { return _calendarInvitees; } }

		private List<iONCalendarEvent> _calendarEvents { get; set; }
		public IEnumerable<iONCalendarEvent> CalendarEvents { get { return _calendarEvents; } }
		#endregion

		#region Constructor
		public iONCalendar(iONCalendarCredentials credentials)
		{
			_credentials = credentials;
			this.AuthorID = credentials.UserID;
		}

		public iONCalendar(iONCalendarCredentials credentials, Data.Tables.Calendar calendar)
		{
			_credentials = credentials;
			this.AuthorID = credentials.UserID;
			this.CalendarID = calendar.ID;
			this.Name = calendar.Name;
			this.Description = this.Description;
			this.AuthorID = calendar.AuthorID;
			this.WasLazyLoaded = true;
		}
		#endregion

		#region Methods
		public bool FindEvent(iONCalendarEvent calendarEvent)
		{
			return true;
		}

		public void Load(Guid calendarID)
		{
			this.CalendarID = calendarID;
			_load(false);
			this.WasLazyLoaded = false;
		}

		public void LazyLoad(Guid calendarID)
		{
			this.CalendarID = calendarID;
			_load(true);
			this.WasLazyLoaded = true;
		}

		private void _load(bool isLazyLoad)
		{
			using (var context = new CalendarContext(_credentials))
			{
				var calendar = context.Calendars.Where(w => w.ID == this.CalendarID).FirstOrDefault();

				if (calendar == null)
				{
					throw new iONCalendarException(this, "Calendar Not Found");
				}

				// This is who the calendar is shared with
				_calendarInvitees = context.CalendarInvitees.Where(w => w.CalendarID == this.CalendarID).ToList();

				if (_calendarEvents == null)
				{
					_calendarEvents = new List<iONCalendarEvent>();
				}

				// load the events
				foreach (var e in context.Events.Where(w => w.CalendarID == this.CalendarID))
				{
					var ionCalendarEvent = new iONCalendarEvent(_credentials, e);

					if (!isLazyLoad)
					{
						ionCalendarEvent.Load(e.ID);
					}

					_calendarEvents.Add(ionCalendarEvent);
				}

				// this is the ID of who created the calendar
				this.AuthorID = calendar.AuthorID;
			}
		}

		private void Refresh()
		{
			_load(false);
		}

		public void Delete()
		{

		}

		public void Update()
		{
			using (var context = new CalendarContext(_credentials))
			{
				// add the calendar
				var calendar = context.Calendars.Where(w => w.ID == this.CalendarID).First();
				calendar.AuthorID = _credentials.UserID;
				calendar.Description = this.Description;
				calendar.Name = this.Name;

				// update the invitees
				// check id's to update?

				// add the history record
				var history = new CalendarHistory();
				history.CalendarID = this.CalendarID;
				history.TransactionDate = DateTime.Now;
				history.TransactionType = Data.Enumeration.CalendarHistoryType.Edit;
				history.UserID = _credentials.UserID;
				context.CalendarHistory.Add(history);
				context.SaveChanges();
			}
		}

		public void Insert()
		{
			using (var context = new CalendarContext(_credentials))
			{
				// add the calendar
				var calendar = new Data.Tables.Calendar();
				calendar.AuthorID = _credentials.UserID;
				calendar.Description = this.Description;
				calendar.Name = this.Name;
				context.Calendars.Add(calendar);
				context.SaveChanges();
				this.CalendarID = calendar.ID;

				// Sharing
				foreach (var invitee in _calendarInvitees)
				{
					invitee.CalendarID = this.CalendarID;
					context.CalendarInvitees.Add(invitee);
				}

				// add the history record
				var history = new CalendarHistory();
				history.CalendarID = this.CalendarID;
				history.TransactionDate = DateTime.Now;
				history.TransactionType = Data.Enumeration.CalendarHistoryType.Create;
				history.UserID = _credentials.UserID;
				context.CalendarHistory.Add(history);
				context.SaveChanges();
			}
		}

		public void Share(CalendarInvitee invitee)
		{
			invitee.CalendarID = this.CalendarID;
			_calendarInvitees.Add(invitee);
		}
		#endregion

		// Connection to only be used with events
		private class CalendarContext : DbContext
		{
			public CalendarContext(iONCalendarCredentials credentials)
				: base(credentials.Server, credentials.Database, credentials.Username, credentials.Password)
			{

			}
		}
	}
}
