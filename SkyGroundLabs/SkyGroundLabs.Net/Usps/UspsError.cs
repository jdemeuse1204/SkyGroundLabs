using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Net.Usps
{
    public class UspsError
    {
        public string Number { get; set; }

        public string Source { get; set; }

        public string Description { get; set; }

        public string HelpFile { get; set; }

        public string HelpContext { get; set; }
    }
}
