using System;
using System.Collections.Generic;
using SkyGroundLabs.Data.Sql.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Common.Data
{
	[Table("Floors")]
	public class Floor
	{
		public long ID { get; set; }

		public string Value { get; set; }
	}
}
