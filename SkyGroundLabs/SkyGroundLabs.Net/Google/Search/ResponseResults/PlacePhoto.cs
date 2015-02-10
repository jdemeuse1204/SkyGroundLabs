using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Search.ResponseResults
{
    public class PlacePhoto
    {
        public int height { get; set; }

        public List<string> html_attributions { get; set; }
 
        public string photo_reference { get; set; }

        public int width { get; set; }
    }
}
