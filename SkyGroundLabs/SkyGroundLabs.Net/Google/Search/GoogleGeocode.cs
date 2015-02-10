using System.Net;
using System.Text;
using Newtonsoft.Json;
using SkyGroundLabs.Net.Google.Search.ResponseResults;

namespace SkyGroundLabs.Net.Google.Search
{
    public static class GoogleGeocode
    {
        public static GoogleGeocodeResponse VerifyAddress(PlaceResult place)
        {
            return VerifyAddress(place.formatted_address);
        }

        public static GoogleGeocodeResponse VerifyAddress(string address)
        {
            var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
               address.Replace("'", "").Replace(" ", "+"),
               GoogleAPISettings.Key);
            var wc = new WebClient();
            var response = wc.DownloadData(url);
            var data = Encoding.ASCII.GetString(response);

            return JsonConvert.DeserializeObject<GoogleGeocodeResponse>(data);
        }
    }
}
