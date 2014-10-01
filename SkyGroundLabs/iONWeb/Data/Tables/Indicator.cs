using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Indicators")]
	public class Indicator : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Description { get; set; }

		public bool IsReadOnly { get; set; }

		public bool Value { get; set; }
	}
}
