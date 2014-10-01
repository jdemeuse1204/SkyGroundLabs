using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Calendars")]
	public class Calendar : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Alias { get; set; }

		public string Description { get; set; }

	}
}
