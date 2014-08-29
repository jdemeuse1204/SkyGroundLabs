using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("CrewAssignments")]
	public class CrewAssignment : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }
		[Column("EmployeeID")]
		public long UserID { get; set; }

		public long CrewID { get; set; }
	}
}
