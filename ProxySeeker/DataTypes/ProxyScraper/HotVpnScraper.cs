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
    public class HotVpnScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            List<SystemProxy> proxies = new List<SystemProxy>();

            var mainContent = document.DocumentNode.SelectSingleNode("//table[@id='live']");

            var rows = mainContent.Descendants("tr").ToList();

            string ipAddress = "";
            string port = "";
            if (rows != null)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    if (j != 0)
                    {
                        var cells = rows[j].Descendants("td").ToList();

                        if (cells != null)
                        {
                            for (int i = 0; i < cells.Count; i++)
                            {
                                if (i == 0)
                                    continue;
                                else if (i == 1)
                                {
                                    var rawHtml = cells[i].InnerHtml;

                                    var hiddenDivs = cells[i].SelectNodes("div[@style='display:none']");
                                    var hiddenSpans = cells[i].SelectNodes("span[@style='display:none']");

                                    if (hiddenDivs != null)
                                    {
                                        foreach (var hidden in hiddenDivs)
                                        {
                                            rawHtml = rawHtml.Replace(hidden.OuterHtml, "");
                                        }
                                    }

                                    if (hiddenSpans != null)
                                    {
                                        foreach (var hidden in hiddenSpans)
                                        {
                                            rawHtml = rawHtml.Replace(hidden.OuterHtml, "");
                                        }
                                    }

                                    ipAddress += HtmlSanitizer.StripHtml(rawHtml);
                                }
                                else if (i == 2)
                                {
                                    port += cells[i].InnerText;
                                }
                                else
                                    break;
                            }

                            SystemProxy newItem = new SystemProxy(ipAddress, port, "", "");
                            proxies.Add(newItem);
                            ipAddress = port = "";
                        }
                    }
                }
            }

            return proxies;
        }
    }
}
