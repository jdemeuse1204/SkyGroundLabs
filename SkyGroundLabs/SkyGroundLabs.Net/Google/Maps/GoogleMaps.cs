using System.Net;
using System.Text;
using Newtonsoft.Json;
using SkyGroundLabs.Net.Google.Maps.ResponseResults;

namespace SkyGroundLabs.Net.Google.Maps
{
    public static class GoogleMaps
    {
        public static GoogleMapsResponse GetDirections(Address startAddress, Address destinationAddress)
        {
            var url = "http://maps.googleapis.com/maps/api/directions/json?origin=" +
                startAddress.GetAddressString().Replace(" ", "+") +
                "&destination=" +
                destinationAddress.GetAddressString().Replace(" ", "+") +
                "&sensor=false";
            var wc = new WebClient();
            var response = wc.DownloadData(url);
            var data = Encoding.ASCII.GetString(response);

            return JsonConvert.DeserializeObject<GoogleMapsResponse>(data);
        }
    }
}
