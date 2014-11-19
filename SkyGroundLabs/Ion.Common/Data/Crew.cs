using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Crews")]
	public class Crew
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
