using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Business.Services
{
	public static class ContactServices
	{
		public static string BuildDisplayName(string firstName, string lastName)
		{
			string space = " ";
			if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
				return firstName + space + lastName;
			else if (!String.IsNullOrWhiteSpace(firstName))
				return firstName;
			else if (!String.IsNullOrWhiteSpace(lastName))
				return lastName;
			else
				return string.Empty;
		}

		public static string BuildAddress(object street, object city, object state, object zip)
		{
			street = (street == null ? "" : street);
			city = (city == null ? "" : city);
			state = (state == null ? "" : state);
			zip = (zip == null ? "" : zip);

			return BuildAddress(street.ToString(), city.ToString(), state.ToString(), zip.ToString());
		}

		public static string BuildAddress(string street, string city, string state, string zip)
		{
			string address = "";
			bool isStreetEmpty = string.IsNullOrWhiteSpace(street);
			bool isCityEmpty = string.IsNullOrWhiteSpace(city);
			bool isStateEmpty = string.IsNullOrWhiteSpace(state);
			bool isZipEmpty = string.IsNullOrWhiteSpace(zip);

			if (!isStreetEmpty)
				address += street;

			if (!isCityEmpty)
				address += (!isStreetEmpty ? ", " : "") + city;

			if (!isStateEmpty)
				address += (!isCityEmpty ? ", " : (!isStreetEmpty ? ", " : "")) + state;

			if (!isZipEmpty)
				address += (!isCityEmpty ? " " : (!isStreetEmpty ? " " : (!isStateEmpty ? " " : ""))) + zip;

			return address;
		}
	}
}
