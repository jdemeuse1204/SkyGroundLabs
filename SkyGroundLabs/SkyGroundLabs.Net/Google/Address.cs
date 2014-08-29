using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Google
{
	public class Address
	{
		public string City { get; set; }
		public string State { get; set; }
		public string Street { get; set; }
		public string ZipCode { get; set; }

		public string GetAddressString()
		{
			string result = "";
			bool hasStreet = !string.IsNullOrEmpty(this.Street);
			bool hasCity = !string.IsNullOrEmpty(this.City);
			bool hasState = !string.IsNullOrEmpty(this.State);
			bool hasZip = !string.IsNullOrEmpty(this.ZipCode) && this.ZipCode != "0";

			if (hasStreet)
			{
				result += this.Street;
			}

			if (hasCity)
			{
				result += ", " + this.City;
			}

			if (hasState)
			{
				result += ", " + this.State;
			}

			if (hasZip)
			{
				result += (!hasCity || !hasState ? ", " : " ") + this.Street;
			}

			return result;
		}
	}
}
