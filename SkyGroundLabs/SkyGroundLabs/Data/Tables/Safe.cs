using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("Safes")]
	public class Safe : DbTableEquatable<IDbTableEquatable<long>>
	{
		public Safe()
		{
			Weight = 0;
		}

		public long ID { get; set; }

		public string Name { get; set; }

		public double Weight { get; set; }
	}
}
