using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace Ion.Data.Tables
{
	[Table("Parameters")]
	public class Parameter : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Description { get; set; }

		public bool IsReadOnly { get; set; }

		public string TextValue { get; set; }

		public long? IntegerValue { get; set; }

		public DateTime? DateValue { get; set; }

		public int Type { get; set; }
	}
}
