using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Parameters")]
	public class Parameter
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
