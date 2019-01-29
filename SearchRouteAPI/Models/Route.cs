using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchRouteAPI.Models
{
    public class Route
    {
        public string airline { get; set; }
        public string srcAirport { get; set; }
        public string destAirport { get; set; }
    }
}
