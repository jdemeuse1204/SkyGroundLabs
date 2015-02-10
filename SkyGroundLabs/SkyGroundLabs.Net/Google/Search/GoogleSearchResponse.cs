using System.Collections.Generic;
using SkyGroundLabs.Net.Google.Search.ResponseResults;

namespace SkyGroundLabs.Net.Google.Search
{
    public class GoogleSearchResponse
    {
        List<string> html_attributions { get; set; }
 
        public string next_page_token { get; set; }

        public List<PlaceResult> results { get; set; }
 
        public string status { get; set; }
    }
}
