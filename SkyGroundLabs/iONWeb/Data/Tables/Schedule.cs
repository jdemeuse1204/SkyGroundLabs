using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Schedules")]
	public class Schedule : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public int UserID { get; set; }

		public Guid EventID { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }


	}
}
