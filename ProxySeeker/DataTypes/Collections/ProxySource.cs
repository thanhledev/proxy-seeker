using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.DataTypes
{
    public class ProxySource
    {
        public string SourceUrl { get; set; }
        public int Founded { get; set; }
        public int Added { get; set; }
        public int Working { get; set; }
        public int Anonymous { get; set; }

        private List<SystemProxy> _proxies;

        public List<SystemProxy> Proxies
        {
            get { return _proxies; }
            set { _proxies = value; }
        }

        public ProxySource(string url)
        {
            SourceUrl = url;
            _proxies = new List<SystemProxy>();
            Founded = Added = Working = Anonymous = 0;
        }        

        public bool IsExisted(SystemProxy check)
        {
            foreach (var p in _proxies)
            {
                if (p.ProxyIp == check.ProxyIp && p.ProxyPort == check.ProxyPort)
                    return true;
            }
            return false;
        }
    }
}
