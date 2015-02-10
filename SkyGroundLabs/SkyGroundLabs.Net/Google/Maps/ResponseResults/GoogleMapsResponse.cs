using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Maps.ResponseResults
{
    public class GoogleMapsResponse
    {
        public List<GoogleMapRoute> routes { get; set; } 

        public string status { get; set; }
    }
}
