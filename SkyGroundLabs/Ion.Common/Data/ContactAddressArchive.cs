using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Common.Data
{
	public class ContactAddressArchive 
	{
		public long ID { get; set; }

		public long ContactID { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }
	}
}
