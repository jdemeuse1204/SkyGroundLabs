using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Indicators")]
	public class Indicator
	{
		public long ID { get; set; }

		public string Description { get; set; }

		public bool IsReadOnly { get; set; }

		public bool Value { get; set; }
	}
}
