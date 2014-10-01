using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;
using SkyGroundLabs.Net.Google.Calendars;

namespace SkyGroundLabs.Net.Google
{
	public class CalendarEvent
	{
		#region Properties
		private readonly string _applicationName = "CalendarSampleApp";
		private GoogleCalendarCredentials _credentials { get; set; }
		#endregion

		#region Constructors And Helpers
		public CalendarEvent(GoogleCalendarCredentials credentials)
		{
			Invitees = new List<CalendarEventInvitee>();
			Reminders = new List<CalendarEventReminder>();
			_credentials = credentials;
		}

		public CalendarEvent(GoogleCalendarCredentials credentials, AtomEntry entry)
		{
			Invitees = new List<CalendarEventInvitee>();
			Reminders = new List<CalendarEventReminder>();
			_credentials = credentials;
			_setContstructor((EventEntry)entry);
		}

		public CalendarEvent(GoogleCalendarCredentials credentials, EventEntry entry)
		{
			Invitees = new List<CalendarEventInvitee>();
			Reminders = new List<CalendarEventReminder>();
			_credentials = credentials;
			_setContstructor(entry);
		}

		private void _setContstructor(EventEntry entry)
		{
			_entry = entry;
			DateCreated = entry.Published;
			DateEdied = entry.Updated;

			foreach (var participant in entry.Participants)
			{
				CalendarEventInvitee invitee = new CalendarEventInvitee();
				invitee.Accepted = participant.Attendee_Status == null ? false : participant.Attendee_Status.Value == "accepted";
				invitee.Email = participant.Email;
				invitee.Name = participant.ValueString;
				Invitees.Add(invitee);
			}

			foreach (var reimnder in entry.Reminders)
			{
				CalendarEventReminder calReminder = new CalendarEventReminder();
				calReminder.Days = reimnder.Days;
				calReminder.Minutes = reimnder.Minutes;
				calReminder.Hours = reimnder.Hours;
				calReminder.Type = reimnder.Method;
				Reminders.Add(calReminder);
			}

			Creator = new CalendarEventInvitee();
			Creator.Accepted = true;
			Creator.Email = entry.Authors[0].Email;
			Creator.Name = entry.Authors[0].Name;

			if (entry.Times.Count > 0)
			{
				StartTime = entry.Times[0].StartTime;
				EndTime = entry.Times[0].EndTime;
				IsAllDay = entry.Times[0].AllDay;
			}
			else
			{
				IsReoccuring = true;
			}

			Location = entry.Locations[0].ValueString;

			Title = entry.Title.Text;
			Contents = entry.Content.Content;
			EventID = entry.EventId;
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
		public string EventID { get; set; }
		public bool IsAllDay { get; set; }
		public bool IsReoccuring { get; set; }
		public CalendarEventInvitee Creator { get; set; }
		private EventEntry _entry { get; set; }
		public bool IsSaved { get { return _entry != null; } }
		public List<CalendarEventReminder> Reminders { get; set; }
		public List<CalendarEventInvitee> Invitees { get; set; }
		#endregion

		#region Methods
		public bool Delete()
		{
			if (_entry == null)
			{
				_entry.Delete();
				return true;
			}
			return false;
		}

		public bool Save()
		{
			// TODO : Test me!
			try
			{
				if (_entry == null)
				{
					var service = new CalendarService(_applicationName);
					service.setUserCredentials(_credentials.EmailAddress, _credentials.Password);

					// this will create the entry for us and set it to the current calendar event info
					_setEventEntryToCurrent();

					// this is the calendar it is being posted to
					var postUri = new Uri(_credentials.CalendarUri);

					// Send the request and receive the response:
					this._entry = service.Insert(postUri, _entry);

					// grab the eventId back
					this.EventID = _entry.EventId;

				}
				else
				{
					// set the entry to the current calendar event info and update
					_setEventEntryToCurrent();
					_entry.Update();
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		private void _setEventEntryToCurrent()
		{
			// create the entry if needed
			if (_entry == null)
			{
				_entry = new EventEntry();
			}

			// Set the title
			_entry.Title.Text = this.Title;

			// Set the contents
			_entry.Content.Content = this.Contents;

			// Set a location for the event.
			_entry.Locations.Clear();
			_entry.Locations.Add(new Where()
			{
				ValueString = this.Location
			});

			// Set the event time
			_entry.Times.Clear();
			_entry.Times.Add(new When(this.StartTime, this.EndTime));

			// reminders clear first so we can update it
			_entry.Reminders.Clear();

			foreach (var item in Reminders)
			{
				var newReminder = new Reminder();
				newReminder.Method = item.Type;
				newReminder.Minutes = item.Minutes;
				newReminder.Hours = item.Hours;
				newReminder.Days = item.Days;
				_entry.Reminders.Add(newReminder);
			}

			// participants clear first so we can update it
			_entry.Participants.Clear();

			foreach (var item in Invitees)
			{
				var newInvitee = new Who();
				newInvitee.Email = item.Email;
				newInvitee.ValueString = item.Name;
				_entry.Participants.Add(newInvitee);
			}
		}
		#endregion
	}
}
