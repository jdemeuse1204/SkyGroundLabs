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
		private iONCredentials _credentials { get; set; }
		#endregion

		#region Constructors And Helpers
		public iONCalendarEvent(iONCredentials credentials)
		{
			Invitees = new List<EventInvitee>();
			Reminders = new List<EventReminder>();
			_credentials = credentials;
			this.Type = EventType.Private;
		}

		public iONCalendarEvent(iONCredentials credentials, Event e)
		{
			Invitees = new List<EventInvitee>();
			Reminders = new List<EventReminder>();
			_credentials = credentials;
			this.EventID = e.ID;
			this.StartTime = e.StartTime;
			this.EndTime = e.EndTime;
			this.CalendarID = e.CalendarID;
			this.Type = e.Type;
			this.Title = e.Title;
			this.Location = e.Location;
			this.Contents = e.Contents;
			WasLazyLoaded = true;
		}
		#endregion

		#region Properties
		public bool WasLazyLoaded { get; private set; }
		public DateTime DateCreated { get; private set; }
		public DateTime DateEdied { get; private set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Location { get; set; }
		public string Title { get; set; }
		public string Contents { get; set; }
		public Guid EventID { get; private set; }
		public Guid CalendarID { get; private set; }
		public EventType Type { get; set; }
		public bool IsAllDay { get; set; }
		public bool IsReoccuring { get; set; }
		public User Creator { get; private set; }
		public CalendarEvent GoogleCalendarEvent { get; private set; }
		public bool IsSaved { get { return EventID != Guid.Empty; } }
		public List<EventReminder> Reminders { get; set; }
		public List<EventInvitee> Invitees { get; set; }
		#endregion

		#region Methods
		public void Delete()
		{

		}

		public void Insert(Guid calendarID)
		{
			using (var context = new EventContext(_credentials))
			{
				this.CalendarID = calendarID;

				var e = new Event();
				e.Contents = this.Contents;
				e.EndTime = this.EndTime;
				e.ID = this.EventID;
				e.IsAllDay = this.IsAllDay;
				e.IsReoccuring = this.IsReoccuring;
				e.Location = this.Location;
				e.StartTime = this.StartTime;
				e.Title = this.Title;
				e.Type = this.Type;
				e.CalendarID = this.CalendarID;
				e.AuthorID = _credentials.UserID;

				// Add and save here so we have the event id
				context.Events.Add(e);
				context.SaveChanges();

				// Grab the event ID after saved
				this.EventID = e.ID;

				// Add the reminders
				foreach (var item in Reminders)
				{
					context.EventReminders.Add(item);
				}

				if (this.Type == EventType.None)
				{
					this.Type = EventType.Private;
				}

				// Do the calendar sharing
				if (this.Type == EventType.Public || this.Type == EventType.EmployeesOnly)
				{
					// Add the individuals who are already invited to the calendar
					// only if the event is made public
					var employeeInvitees = (from c in context.CalendarInvitees
											join u in context.Users on c.InviteeID equals u.ID
											where c.Type == InviteeType.Employee
											select new EventInvitee
											{
												EventID = this.EventID,
												InviteeID = u.ID,
												Type = InviteeType.Employee
											}).ToList();

					Invitees.AddRange(employeeInvitees);

					// add the customers
					if (this.Type == EventType.Public)
					{
						var customerInvitees = (from c in context.CalendarInvitees
												join customers in context.Customers on c.InviteeID equals customers.ID
												where c.Type == InviteeType.Employee
												select new EventInvitee
												{
													EventID = this.EventID,
													InviteeID = customers.ID,
													Type = InviteeType.Customer
												}).ToList();

						Invitees.AddRange(customerInvitees);
					}
				}

				// Add the invitees
				foreach (var item in Invitees)
				{
					context.EventInvitees.Add(item);
				}

				// set the transaction dates
				this.DateCreated = DateTime.Now;
				this.DateEdied = this.DateCreated;

				// need to save the date created and date edited
				// need to do this here because we need the event id
				EventHistory history = new EventHistory();
				history.UserID = _credentials.UserID;
				history.TransactionDate = this.DateCreated;
				history.EventID = this.EventID;
				history.TransactionType = EventHistoryType.Create;
				context.EventHistory.Add(history);

				// save the history
				context.SaveChanges();

				// set the creator
				this.Creator = context.Users.Where(w => w.ID == e.AuthorID).First();
			}
		}

		public void Update()
		{
			WasLazyLoaded = false;
			// need to make sure the event is saved
			if (this.EventID == Guid.Empty)
			{
				throw new iONCalendarEventException(this, "Event ID Empty, cannot update");
			}

			using (var context = new EventContext(_credentials))
			{
				var e = context.Events.Find(this.EventID);

				// someone else could have removed the event
				if (e == null)
				{
					throw new iONCalendarEventException(this, "Event not found, cannot update");
				}

				e.Contents = this.Contents;
				e.EndTime = this.EndTime;
				e.ID = this.EventID;
				e.IsAllDay = this.IsAllDay;
				e.IsReoccuring = this.IsReoccuring;
				e.Location = this.Location;
				e.StartTime = this.StartTime;
				e.Title = this.Title;

				// update the reminders, need to see if they changed
				foreach (var item in Reminders)
				{
					if (item.MarkedForRemoval)
					{
						context.EventReminders.Remove(item);
					}
				}

				// update the invitees, need to see if they changed
				foreach (var item in Invitees)
				{
					if (item.MarkedForRemoval)
					{
						context.EventInvitees.Remove(item);
					}
				}

				// do an atomic save here
				context.SaveChanges();

				// need to save the date created and date edited
				// need to do this here because we need the event id
				EventHistory history = new EventHistory();
				history.UserID = _credentials.UserID;
				history.TransactionDate = DateTime.Now;
				history.EventID = e.ID;
				history.TransactionType = EventHistoryType.Edit;
				context.EventHistory.Add(history);

				// save the history
				context.SaveChanges();
			}
		}

		public void Load(Guid eventID)
		{
			this.EventID = eventID;
			Refresh();
			WasLazyLoaded = false;
		}

		public void Refresh()
		{
			WasLazyLoaded = false;
			// need to make sure the event is saved
			if (this.EventID == Guid.Empty)
			{
				throw new iONCalendarEventException(this, "Event ID Empty, cannot update");
			}

			using (var context = new EventContext(_credentials))
			{
				var e = context.Events.Find(this.EventID);

				// someone else could have removed the event
				if (e == null)
				{
					throw new iONCalendarEventException(this, "Event not found, cannot update");
				}

				this.Type = e.Type;
				this.Contents = e.Contents;
				this.EndTime = e.EndTime;
				this.EventID = e.ID;
				this.IsAllDay = e.IsAllDay;
				this.IsReoccuring = e.IsReoccuring;
				this.Location = e.Location;
				this.StartTime = e.StartTime;
				this.Title = e.Title;
				this.CalendarID = e.CalendarID;
				this.Creator = context.Users.Where(w => w.ID == e.AuthorID).First();
				this.Reminders = context.EventReminders.Where(w => w.EventID == this.EventID).ToList();
				this.Invitees = context.EventInvitees.Where(w => w.EventID == this.EventID).ToList();
				this.DateCreated = context.EventHistory.Where(w => w.EventID == this.EventID && w.TransactionType == EventHistoryType.Create).Select(w => w.TransactionDate).First();

				var edits = context.EventHistory.Where(w => w.EventID == this.EventID && w.TransactionType == EventHistoryType.Edit);

				this.DateEdied = edits.Count() > 0 ? edits.Select(w => w.TransactionDate).Max() : DateTime.MinValue;
			}
		}

		// Connection to only be used with events
		private class EventContext : DbContext
		{
			public EventContext(iONCredentials credentials)
				: base(credentials.Server, credentials.Database, credentials.Username, credentials.Password)
			{

			}
		}
		#endregion
	}
}
