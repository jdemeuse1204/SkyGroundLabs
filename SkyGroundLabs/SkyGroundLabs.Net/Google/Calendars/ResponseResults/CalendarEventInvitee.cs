using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Google.Calendars.ResponseResults
{
	public class CalendarEventInvitee
	{
		public string Email { get; set; }
		public string Name { get; set; }
		public bool Accepted { get; set; }
	}
}
