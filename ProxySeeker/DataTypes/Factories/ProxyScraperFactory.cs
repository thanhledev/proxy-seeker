using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProxySeeker.DataTypes;
using System.Text.RegularExpressions;

namespace ProxySeeker.DataTypes
{
    public static class ProxyScraperFactory
    {
        public static IProxyScraper GetProxyScraper(string url)
        {
            if (url.Contains(".txt"))
            {
                return new TxtProxyScraper();
            }
            else
            {
                if (url.Contains("aliveproxy.com"))
                    return new AliveProxyScraper();
                else if (url.Contains("checkerproxy.net"))
                    return new CheckerProxyScraper();
                else if (url.Contains("cool-tests.com"))
                    return new CoolTestScraper();
                else if (url.Contains("fineproxy.org"))
                    return new FineProxyScraper();
                else if (url.Contains("getproxy.jp"))
                    return new GetProxyJPScraper();
                else if (url.Contains("google-proxy.net"))
                    return new GoogleProxyScraper();
                else if (url.Contains("hotvpn.com"))
                    return new HotVpnScraper();
                else if (url.Contains("letushide.com"))
                    return new LetushideScraper();                
                else if (url.Contains("notan.h1.ru"))
                    return new NotanH1Scraper();
                else if (url.Contains("sakura.ne.jp"))
                    return new ProxySakuraScraper();
                else
                    return new NullProxyScraper();
            }
        }
    }
}
