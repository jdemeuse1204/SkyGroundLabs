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
            var result = "";
			var hasStreet = !string.IsNullOrEmpty(this.Street);
            var hasCity = !string.IsNullOrEmpty(this.City);
            var hasState = !string.IsNullOrEmpty(this.State);
            var hasZip = !string.IsNullOrEmpty(this.ZipCode) && this.ZipCode != "0";

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
