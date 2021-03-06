﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ion.Business;
using Ion.Business.Extension;

namespace OnlineIon.Business
{
	public class ClockingDisplayEmployeeValue : ClockingDisplayValue
	{
		public string Name { get; set; }
		public bool Approved { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string Code { get; set; }

		public ClockingDisplayEmployeeValue(UserClockingPair pair)
			: base(pair)
		{
			Code = pair.InPunch.SpecialCode;
			Longitude = pair.Longitude;
			Latitude = pair.Latitude;
			Approved = pair.OutPunch.IsApproved;
			Name = pair.Name;
		}
	}
}