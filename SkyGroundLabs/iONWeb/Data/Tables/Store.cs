using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Stores")]
	public class Store : DbTableEquatable<IDbTableEquatable<long>>
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
