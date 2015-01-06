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
    public class ProxySakuraScraper : IProxyScraper
    {
        public List<SystemProxy> GetProxies(HtmlDocument document, string url)
        {
            List<SystemProxy> proxies = new List<SystemProxy>();

            var mainContent = document.DocumentNode.SelectSingleNode("//div[@class='blockf']");

            var table = mainContent.SelectSingleNode("//table");

            var rows = table.Descendants("tr").ToList();

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var cells = row.Descendants("td").ToList();

                    if (cells != null)
                    {
                        foreach (var cell in cells)
                        {
                            if (cell.Attributes["id"] != null)
                            {
                                if (cell.Attributes["id"].Value == "address")
                                {
                                    string rawText = cell.InnerText.Trim();
                                    rawText = rawText.Replace("<!--", "");
                                    rawText = rawText.Replace("// -->", "");

                                    rawText = rawText.Trim();

                                    rawText = rawText.Replace("proxy(", "").Trim();
                                    rawText = rawText.Replace(");", "").Trim();
                                    rawText = rawText.Replace("'", "").Trim();

                                    string[] args = rawText.Split(',');

                                    string proxy = extractProxy(args[0], args[1], args[2], args[3], args[4], args[5]);
                                    string[] pArgs = proxy.Split(':');

                                    SystemProxy newItem = new SystemProxy(pArgs[0], pArgs[1], "", "");

                                    proxies.Add(newItem);
                                }
                            }
                        }
                    }
                }
            }

            return proxies;
        }

        private string extractProxy(string mode, string arg1, string arg2, string arg3, string arg4, string port)
        {
            string value = "";

            switch (mode)
            {
                case "1":
                    value = arg1 + "." + arg2 + "." + arg3 + "." + arg4;
                    break;
                case "2":
                    value = arg4 + "." + arg1 + "." + arg2 + "." + arg3;
                    break;
                case "3":
                    value = arg3 + "." + arg4 + "." + arg1 + "." + arg2;
                    break;
                case "4":
                    value = arg2 + "." + arg3 + "." + arg4 + "." + arg1;
                    break;
            };

            value += ":" + port;

            return value;
        }
    }
}
