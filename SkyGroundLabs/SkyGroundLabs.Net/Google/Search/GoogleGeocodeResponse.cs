using System.Collections.Generic;
using SkyGroundLabs.Net.Google.Search.ResponseResults;

namespace SkyGroundLabs.Net.Google.Search
{
    public class GoogleGeocodeResponse
    {
        public List<GeocodeResult> results { get; set; }

        public string status { get; set; }
    }
}
