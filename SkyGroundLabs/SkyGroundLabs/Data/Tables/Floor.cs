using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("Floors")]
	public class Floor : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Value { get; set; }
	}
}
