using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.DataTypes
{
    public class IpGeoLocation
    {
        public long minThreshold { get; set; }
        public long maxThreshold { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }

        public IpGeoLocation(long min, long max, string code, string name)
        {
            minThreshold = min;
            maxThreshold = max;
            countryCode = code;
            countryName = name;
        }
    }
}
