using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxySeeker.DataTypes;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using HtmlAgilityPack;

namespace ProxySeeker.DataTypes
{
    public class TxtProxyScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            List<SystemProxy> proxies = new List<SystemProxy>();

            var value = document.DocumentNode.InnerText;

            string proxyPattern = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}";

            MatchCollection collection = Regex.Matches(value, proxyPattern);
            foreach (Match match in collection)
            {
                if (match.Success)
                {
                    string proxy = match.Groups[0].Value;

                    string[] args = proxy.Split(':');

                    SystemProxy newItem = new SystemProxy(args[0], args[1], "", "");

                    proxies.Add(newItem);
                }
            }

            return proxies;
        }
    }
}
