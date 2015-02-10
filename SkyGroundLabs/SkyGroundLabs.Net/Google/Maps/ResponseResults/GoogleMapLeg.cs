namespace SkyGroundLabs.Net.Google.Maps.ResponseResults
{
    public class GoogleMapLeg
    {
        public MapPair distance { get; set; }

        public MapPair duration { get; set; }

        public string end_address { get; set; }

        public GeometryLocation end_location { get; set; }

        public string start_address { get; set; }

        public GeometryLocation start_location { get; set; }
    }
}
