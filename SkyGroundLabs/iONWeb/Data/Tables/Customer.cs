using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Customers")]
	public class Customer : DbTableEquatable<IDbTableEquatable<int>>
	{
		public int ID { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string Phone1 { get; set; }

		public int Phone1Type { get; set; }

		public string Phone2 { get; set; }

		public int Phone2Type { get; set; }

		public string Comments { get; set; }
	}
}
