using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchRouteAPI.Models
{
    public class Airport
    {
        public string name { get; set; }
        public string alias { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int altitude { get; set; }
    }
}
