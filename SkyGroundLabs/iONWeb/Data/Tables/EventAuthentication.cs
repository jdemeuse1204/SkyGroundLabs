using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	public class EventAuthentication : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		public Guid ID { get; set; }

		public Guid EventID { get; set; }

		public DateTime AccessStartDate { get; set; }

		public DateTime AccessEndDate { get; set; }

		public int CustomerID { get; set; }

		public DateTime LastAccessDate { get; set; }
	}
}
