using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Companies")]
	public class Company : DbTableEquatable<IDbTableEquatable<int>>
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public string Phone1 { get; set; }

		public string Phone2 { get; set; }

		public string Fax { get; set; }

		public string Website { get; set; }

		public string Email { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }
	}
}
