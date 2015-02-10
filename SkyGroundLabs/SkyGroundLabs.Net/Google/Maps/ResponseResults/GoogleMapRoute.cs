using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Maps.ResponseResults
{
    public class GoogleMapRoute
    {
        public string copyrights { get; set; }

        public List<GoogleMapLeg> legs { get; set; }

        public string summary { get; set; }

        public List<string> warnings { get; set; }

        public List<string> waypoint_order { get; set; }

        public double GetDistance()
        {
            if (legs == null || legs.Count == 0)
            {
                return 0d;
            }

            // convert meters to miles
            return legs[0].distance.value * 0.00062137;
        }
    }
}
