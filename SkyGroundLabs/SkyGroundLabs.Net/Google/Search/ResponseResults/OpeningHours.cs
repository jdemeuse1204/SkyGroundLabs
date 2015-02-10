using System.Collections.Generic;

namespace SkyGroundLabs.Net.Google.Search.ResponseResults
{
    public class OpeningHours
    {
        public bool open_now { get; set; }

        public List<string> weekday_text { get; set; } 
    }
}
