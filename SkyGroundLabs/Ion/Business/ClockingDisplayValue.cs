using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ion.Business;
using Ion.Business.Extension;
using Ion.Data.Tables;

namespace OnlineIon.Business
{
	public class ClockingDisplayValue
	{
		#region Properties
		public string InPunchTime 
		{ 
			get { return _inPunchTime.ToDateTimeString(); }
			set { _inPunchTime = Convert.ToDateTime(value); }
		}
		public string OutPunchTime 
		{
			get { return _outPunchTime.ToDateTimeString(); }
			set { _outPunchTime = Convert.ToDateTime(value); }
		}

		public string OriginalInPunchTime
		{
			get { return _originalInPunchTime.ToDateTimeString(); }
			set { _originalInPunchTime = Convert.ToDateTime(value); }
		}
		public string OriginalOutPunchTime
		{
			get { return _originalOutPunchTime.ToDateTimeString(); }
			set { _originalOutPunchTime = Convert.ToDateTime(value); }
		}

		public string IsApproved { get { return _isApproved ? "Yes" : "No"; } }
		public string Hours { get { return _hours < 0 ? "Awaiting Out Punch" : _hours.ToString("0.00"); } }
		public Guid ID { get; set; }

		private DateTime _inPunchTime { get; set; }
		private DateTime _outPunchTime { get; set; }
		private DateTime _originalInPunchTime { get; set; }
		private DateTime _originalOutPunchTime { get; set; }
		private bool _isApproved { get; set; }
		private bool _isInPunch { get; set; }
		private double _hours { get; set; }
		#endregion

		#region Constructor
		public ClockingDisplayValue(UserClockingPair pair)
		{
			_inPunchTime = pair.InPunch.PunchTimeRounded;
			_outPunchTime = pair.OutPunch.PunchTimeRounded;
			_originalInPunchTime = pair.InPunch.OriginalPunchTime;
			_originalOutPunchTime = pair.OutPunch.OriginalPunchTime;
			_isApproved = pair.OutPunch.IsApproved;
			_hours = pair.GetHours();
			ID = pair.InPunch.PairingID;
		}
		#endregion
	}
}