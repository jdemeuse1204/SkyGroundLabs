using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data;
using iONWeb.Data.Enumeration;
using iONWeb.Data.Tables;
using SkyGroundLabs.Net.Google;

namespace iONWeb.Business.Calendar
{
	public class iONCalendarEvent
	{
		#region Properties
		private iONCalendarCredentials _credentials { get; set; }
		#endregion

		#region Constructors And Helpers
		public iONCalendarEvent(iONCalendarCredentials credentials)
		{
			Invitees = new List<EventInvitee>();
			Reminders = new List<EventReminder>();
			_credentials = credentials;
		}
		#endregion

		#region Properties
		public DateTime DateCreated { get; private set; }
		public DateTime DateEdied { get; private set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Location { get; set; }
		public string Title { get; set; }
		public string Contents { get; set; }
		public Guid EventID { get; private set; }
		public bool IsAllDay { get; set; }
		public bool IsReoccuring { get; set; }
		public EventInvitee Creator { get; private set; }
		public CalendarEvent GoogleCalendarEvent { get; private set; }
		public bool IsSaved { get { return EventID != Guid.Empty; } }
		public List<EventReminder> Reminders { get; set; }
		public List<EventInvitee> Invitees { get; set; }
		#endregion

		#region Methods
		public bool Delete()
		{
			return true;
		}

		public void Save()
		{
			using (var context = new CalendarConext(_credentials))
			{
				// save here so it can be used later
				var isNewEvent = this.EventID == Guid.Empty;
				var e = new Event();

				e.Contents = this.Contents;
				e.EndTime = this.EndTime;
				e.ID = this.EventID;
				e.IsAllDay = this.IsAllDay;
				e.IsReoccuring = this.IsReoccuring;
				e.Location = this.Location;
				e.StartTime = this.StartTime;
				e.Title = this.Title;

				// Only mark if the event was never in the database
				if (e.ID == Guid.Empty)
				{
					e.CreatedByUserID = _credentials.UserID;
				}

				// update the reminders
				foreach (var item in Reminders)
				{
					if (item.MarkedForRemoval)
					{
						context.EventReminders.Remove(item);
					}
				}

				// update the invitees
				foreach (var item in Invitees)
				{
					if (item.MarkedForRemoval)
					{
						context.EventInvitees.Remove(item);
					}
				}
				// how do we link the reminder and the event if the id isnt set yet?  make a method


				// is it in the database yet or not?
				if (e.ID == Guid.Empty)
				{
					context.Events.Add(e);
				}

				// do an atomic save here
				context.SaveChanges();

				// Grab the event ID after saved
				this.EventID = e.ID;

				// need to save the date created and date edited
				// need to do this here because we need the event id
				EventHistory history = new EventHistory();
				history.UserID = _credentials.UserID;
				history.TransactionDate = DateTime.Now;
				history.EventID = e.ID;
				history.TransactionType = isNewEvent ? EventHistoryType.Create : EventHistoryType.Edit;
				context.EventHistory.Add(history);

				// save the history
				context.SaveChanges();
			}
		}

		// Connection to only be used with events
		private class CalendarConext : DbContext
		{
			public CalendarConext(iONCalendarCredentials credentials)
				: base(credentials.Server, credentials.Database, credentials.Username, credentials.Password)
			{

			}
		}
		#endregion
	}
}
