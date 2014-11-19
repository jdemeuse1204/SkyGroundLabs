using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ion.Common.Enumeration;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("UserClockings")]
	public class UserClocking 
	{
		public UserClocking()
		{
			PunchTime = Convert.ToDateTime("1/1/1900 12:00 AM"); ;
			PunchTimeRounded = Convert.ToDateTime("1/1/1900 12:00 AM"); ;
			OriginalPunchTime = Convert.ToDateTime("1/1/1900 12:00 AM"); ;
		}

		public long ID { get; set; }

		public long UserID { get; set; }

		public PunchType PunchType { get; set; }

		public DateTime PunchTime { get; set; }

		public DateTime PunchTimeRounded { get; set; }

		public string SpecialCode { get; set; }

		public long AppointmentID { get; set; }

		public bool IsApproved { get; set; }

		public Guid PairingID { get; set; }

		public DateTime OriginalPunchTime { get; set; }

		public bool IsAdjusted { get; set; }

		public long AdjustedUserID { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string PunchTimeZoneName { get; set; }

		public string PunchTimeZoneOffset { get; set; }

		public long PunchUserID { get; set; }
	}
}
