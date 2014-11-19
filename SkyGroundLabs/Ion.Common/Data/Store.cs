using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Stores")]
	public class Store
	{
		public Store()
		{
			Zip = "";
		}

		public long ID { get; set; }

		public string Name { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string Phone { get; set; }

		public string DisplayName { get; set; }

		public string Email { get; set; }
	}
}
