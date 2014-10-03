using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Business.Clocking;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Clockings")]
	public class Clocking : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public int UserID { get; set; }

		public PunchType PunchType { get; set; }

		public DateTime PunchTime { get; set; }

		public DateTime PunchTimeRounded { get; set; }

		public string SpecialCode { get; set; }

		public Guid EventID { get; set; }

		public bool IsApproved { get; set; }

		public Guid PairingID { get; set; }

		public DateTime OriginalPunchTime { get; set; }

		public bool IsAdjusted { get; set; }

		public long AdjustedUserID { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string PunchTimeZoneName { get; set; }

		public string PunchTimeZoneOffset { get; set; }

		public int PunchUserID { get; set; }
	}
}
