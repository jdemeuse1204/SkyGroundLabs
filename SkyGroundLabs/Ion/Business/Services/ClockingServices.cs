using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Business.Services
{
	public static class ClockingServices
	{
		public static DateTime Round(this DateTime clocking)
		{
			TimeSpan span = TimeSpan.FromMinutes(2.5d);
			var delta = clocking.Ticks % span.Ticks;
			var roundUp = delta > span.Ticks / 2;

			return roundUp ? clocking._roundUp(span) : clocking._roundDown(span);
		}

		private static DateTime _roundUp(this DateTime dt, TimeSpan d)
		{
			var delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
			return new DateTime(dt.Ticks + delta);
		}

		private static DateTime _roundDown(this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			return new DateTime(dt.Ticks - delta);
		}

		public static string GetPunchTimeString(DateTime punch)
		{
			return punch.ToShortDateString() + " " + punch.ToShortTimeString();
		}
	}
}
