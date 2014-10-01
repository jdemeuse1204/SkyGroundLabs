using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	public class ContactAddressArchive : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public long ContactID { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }
	}
}
