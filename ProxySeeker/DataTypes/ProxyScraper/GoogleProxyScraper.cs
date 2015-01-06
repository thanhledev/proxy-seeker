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
    public class GoogleProxyScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            List<SystemProxy> proxies = new List<SystemProxy>();

            var mainContent = document.DocumentNode.SelectSingleNode("//table[@id='proxylisttable']");

            var rows = mainContent.Descendants("tr").ToList();

            string ipAddress = "";
            string port = "";
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var cells = row.Descendants("td").ToList();

                    if (cells != null)
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            if (i == 0)
                            {
                                ipAddress = cells[i].InnerText;
                            }
                            else if (i == 1)
                            {
                                port = ":" + cells[i].InnerText;
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

            return proxies;
        }
    }
}
