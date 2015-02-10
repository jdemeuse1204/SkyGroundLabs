using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Search.ResponseResults
{
    public class GeocodeResult
    {
        public List<AddressComponent> address_components { get; set; }

        public string formatted_address { get; set; }

        public PlaceGeometry geometry { get; set; }

        public List<string> types { get; set; }

        public Address GetAddress()
        {
            if (address_components == null || address_components.Count == 0)
            {
                return null;
            }

            var result = new Address();

            foreach (var component in address_components)
            {
                if (component.types.Contains("street_number"))
                {
                    result.Street = component.short_name;
                }
                else if (component.types.Contains("route"))
                {
                    result.Street += (string.IsNullOrWhiteSpace(result.Street) ? "" : " ") + component.short_name;
                }
                else if (component.types.Contains("locality") && component.types.Contains("political"))
                {
                    result.City = component.short_name;
                }
                else if (component.types.Contains("administrative_area_level_1") && component.types.Contains("political"))
                {
                    result.State = component.short_name;
                }
                else if (component.types.Contains("postal_code"))
                {
                    result.ZipCode = component.short_name;
                }
            }

            return result;
        }
    }
}
