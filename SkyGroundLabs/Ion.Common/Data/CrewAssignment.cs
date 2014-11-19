using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("CrewAssignments")]
	public class CrewAssignment
	{
		public long ID { get; set; }
		[Column("EmployeeID")]
		public long UserID { get; set; }

		public long CrewID { get; set; }
	}
}
