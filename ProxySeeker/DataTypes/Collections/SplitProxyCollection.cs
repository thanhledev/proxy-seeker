using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.DataTypes
{
    public class SplitProxyCollection
    {
        public bool SplitForScraper { get; set; }
        public List<SystemProxy> _scraperProxies { get; set; }
        public bool SplitForPoster { get; set; }
        public List<SystemProxy> _posterProxies { get; set; }
        public bool SplitForForums { get; set; }
        public List<SystemProxy> _forumsProxies { get; set; }

        public SplitProxyCollection()
        {
            _scraperProxies = new List<SystemProxy>();
            _posterProxies = new List<SystemProxy>();
            _forumsProxies = new List<SystemProxy>();
        }
    }
}
