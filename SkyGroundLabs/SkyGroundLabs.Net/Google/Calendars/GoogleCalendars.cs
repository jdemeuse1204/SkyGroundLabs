﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;

namespace SkyGroundLabs.Net.Google.Calendars
{
	public class GoogleCalendars
	{
		#region Properties
		private readonly string _applicationName = "CalendarSampleApp";
		private GoogleCalendarCredentials _credentials { get; set; }
		#endregion

		#region Constructor
		public GoogleCalendars(GoogleCalendarCredentials credentials)
		{
			_credentials = credentials;
		}
		#endregion

		#region Methods
		public CalendarEvent Find(string eventId, string searchText)
		{
			return _getEventByID(eventId, searchText);
		}

		public IEnumerable<CalendarEvent> Find(string searchText)
		{
			return _getEventsByQueryString(searchText);
		}

		public IEnumerable<CalendarEvent> All()
		{
			return _getEventsByQueryString("");
		}

		public IEnumerable<CalendarEvent> Find(string searchText, DateTime searchStartDate, DateTime searchEndDate)
		{
			return _getEventsByQueryString(searchText, searchStartDate, searchEndDate);
		}

		private IEnumerable<CalendarEvent> _getEventsByQueryString(string queryString)
		{
			return _getEventsByQueryString(queryString, DateTime.MinValue, DateTime.MinValue);
		}

		private IEnumerable<CalendarEvent> _getEventsByQueryString(string queryString, DateTime startTime, DateTime endTime)
		{
			var results = new List<CalendarEvent>();

			try
			{
				var service = new CalendarService(_applicationName);
				service.setUserCredentials(_credentials.EmailAddress, _credentials.Password);
				FeedQuery myQuery = new EventQuery(_credentials.CalendarUri);

				if (endTime != DateTime.MinValue)
				{
					myQuery.StartDate = endTime;
				}

				if (endTime != DateTime.MinValue)
				{
					myQuery.EndDate = endTime;
				}

				myQuery.Query = queryString;
				var myResultsFeed = service.Query(myQuery);

				for (int i = 0; i < myResultsFeed.Entries.Count; i++)
				{
					EventEntry entry = myResultsFeed.Entries[i] as EventEntry;
					results.Add(new CalendarEvent(_credentials, entry));

					// go to the next page if there is one
					if ((myResultsFeed.Entries.Count - 1) == i && !string.IsNullOrWhiteSpace(myResultsFeed.NextChunk))
					{
						myQuery = new FeedQuery(myResultsFeed.NextChunk);
						myResultsFeed = service.Query(myQuery);
						i = -1;
					}
				}

				return results;
			}
			catch
			{
				return results;
			}
		}

		private CalendarEvent _getEventByID(string eventId, string searchText)
		{
			var list = _getEventsByQueryString(searchText);

			foreach (var item in list)
			{
				if (item.EventID == eventId)
				{
					return item;
				}
			}

			return null;
		}
		#endregion
	}
}
