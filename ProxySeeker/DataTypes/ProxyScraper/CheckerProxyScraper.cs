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
    public class CheckerProxyScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            string proxyPattern = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}";

            List<SystemProxy> proxies = new List<SystemProxy>();

            var mainContent = document.DocumentNode.SelectSingleNode("//table[@id='result-box-table']");

            var rows = mainContent.Descendants("tr").ToList();

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var cells = row.Descendants("td").ToList();

                    if (cells != null)
                    {
                        foreach (var cell in cells)
                        {
                            if (cell.Attributes["class"] != null)
                            {
                                if (cell.Attributes["class"].Value == "proxy-ipport")
                                {
                                    string proxy = cell.InnerText.Trim();

                                    MatchCollection collection = Regex.Matches(proxy, proxyPattern);

                                    foreach (Match match in collection)
                                    {
                                        if (match.Success)
                                        {
                                            proxy = match.Groups[0].Value.Trim();

                                            string[] args = proxy.Split(':');

                                            SystemProxy newItem = new SystemProxy(args[0], args[1], "", "");

                                            proxies.Add(newItem);
                                        }
                                    }
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
