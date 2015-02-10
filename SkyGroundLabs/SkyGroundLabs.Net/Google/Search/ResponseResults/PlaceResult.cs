using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Search.ResponseResults
{
    public class PlaceResult
    {
        public string formatted_address { get; set; }

        public  GeometryLocation geometry { get; set; }

        public string icon { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public OpeningHours opening_hours { get; set; }

        public string place_id { get; set; }

        public float rating { get; set; }

        public string reference { get; set; }

        public List<string> types { get; set; }

        public List<PlacePhoto> photos { get; set; }
    }
}
