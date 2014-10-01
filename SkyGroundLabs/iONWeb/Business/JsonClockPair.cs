using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iONWeb.Data.Tables;

namespace OnlineiONWeb.Business
{
	public class JsonClockPair
	{
		public double InPunchLatitude { get; private set; }
		public double InPunchLongitude { get; private set; }
		public double OutPunchLatitude { get; private set; }
		public double OutPunchLongitude { get; private set; }

		public JsonClockPair(UserClocking inPunch, UserClocking outPunch)
		{
			InPunchLatitude = Math.Round(inPunch.Latitude, 6);
			InPunchLongitude = Math.Round(inPunch.Longitude, 6);
			OutPunchLatitude = Math.Round(outPunch.Latitude, 6);
			OutPunchLongitude = Math.Round(outPunch.Longitude, 6);
		}
	}
}