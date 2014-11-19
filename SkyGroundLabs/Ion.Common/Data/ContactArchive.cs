using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("ContactsArchive")]
	public class ContactArchive 
	{
		public ContactArchive()
		{
			ContactDateEntered = Convert.ToDateTime("1/1/1900 12:00 AM");
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
