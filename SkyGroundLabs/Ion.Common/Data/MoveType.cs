using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("MoveTypes")]
	public class MoveType 
	{
		public long ID { get; set; }

		public string Value { get; set; }

		public long? MoveTypeCategoryID { get; set; }
	}
}
