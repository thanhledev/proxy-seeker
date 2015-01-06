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
    public class AliveProxyScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            List<SystemProxy> proxies = new List<SystemProxy>();

            var mainContent = document.DocumentNode.SelectSingleNode("//table[@class='cm or']");

            var rows = mainContent.Descendants("tr").ToList();
            string proxyPattern = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}";
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var cells = row.Descendants("td");

                    if (cells != null)
                    {
                        foreach (var cell in cells)
                        {
                            string value = cell.InnerText;

                            value = value.Replace("Name unavailable", "").Trim();

                            MatchCollection collection = Regex.Matches(value, proxyPattern);

                            foreach (Match match in collection)
                            {
                                if (match.Success)
                                {
                                    string proxy = match.Groups[0].Value.Trim();

                                    string[] args = proxy.Split(':');

                                    SystemProxy newItem = new SystemProxy(args[0], args[1], "", "");

                                    proxies.Add(newItem);
                                }
                            }
                        }
                    }
                }
            }

            return proxies;
        }
    }
}
