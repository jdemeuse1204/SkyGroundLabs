using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace Ion.Data.Tables
{
	[Table("AppointmentContentsView")]
	public class AppointmentContentsView : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Description { get; set; }

		public bool IsSelected { get; set; }

		public long Tag { get; set; }

		public string PrependTag { get; set; }
	}
}
