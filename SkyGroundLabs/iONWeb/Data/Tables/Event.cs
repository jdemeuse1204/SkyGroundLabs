using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Events")]
	public class Event : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string Location { get; set; }

		public string Title { get; set; }

		public string Contents { get; set; }

		public bool IsAllDay { get; set; }

		public bool IsReoccuring { get; set; }

		public int CreatedByUserID { get; set; }
	}
}
