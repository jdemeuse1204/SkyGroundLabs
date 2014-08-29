using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAGetMail;

namespace SkyGroundLabs.Net.Email
{
	public class ElectronicMailAddress
	{
		public ElectronicMailAddress() { }

		public ElectronicMailAddress(MailAddress mailAddress)
		{
			this.Additional = mailAddress.Additional;
			this.Address = mailAddress.Address;
			this.Name = mailAddress.Name;
		}

		public string Additional { get; set; }
		public string Address { get; set; }
		public string Name { get; set; }
	}
}
