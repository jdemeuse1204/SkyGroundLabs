using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("ContactsArchive")]
	public class ContactArchive : DbTableEquatable<IDbTableEquatable<long>>
	{
		public ContactArchive()
		{
			ContactDateEntered = Defaults.MinDateTime.Date;
		}

		public long ID { get; set; }

		public long ContactID { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public DateTime ContactDateEntered { get; set; }
	}
}
