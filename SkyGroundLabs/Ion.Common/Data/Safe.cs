using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Safes")]
	public class Safe
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
