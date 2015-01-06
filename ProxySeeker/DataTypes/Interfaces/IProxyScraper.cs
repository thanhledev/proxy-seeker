using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProxySeeker.DataTypes;

namespace ProxySeeker.DataTypes
{
    public interface IProxyScraper
    {
        List<SystemProxy> GetProxies(HtmlDocument document, string url);
    }
}
