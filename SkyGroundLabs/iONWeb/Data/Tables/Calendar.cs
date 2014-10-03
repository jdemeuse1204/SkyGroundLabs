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
	public class Calendar : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		/// <summary>
		/// Who the calendar belongs to
		/// </summary>
		public int AuthorID { get; set; }
	}
}
