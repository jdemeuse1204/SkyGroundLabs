using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace Ion.Data.Tables
{
	[Table("Crews")]
	public class Crew : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Alias { get; set; }

		public string DefaultForeground { get; set; }

		public string DefaultBackground { get; set; }

		public string AccountEmailAddress { get; set; }

		public long? CrewLeaderEmployeeID { get; set; }

		public string Comments { get; set; }

		public string Password { get; set; }

		public string PrivateICSLink { get; set; }

		public string PrivateXMLLink { get; set; }
	}
}
