namespace SkyGroundLabs.Net.Google.Maps.ResponseResults
{
    public class GoogleMapsStep
    {
        public MapPair distance { get; set; }

        public MapPair duration { get; set; }

        public GeometryLocation end_location { get; set; }

        public string html_instructions { get; set; }

        public GeometryLocation start_location { get; set; }

        public string travel_mode { get; set; }
    }
}
