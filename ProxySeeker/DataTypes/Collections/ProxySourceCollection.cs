using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.DataTypes
{
    public class ProxySourceCollection
    {
        private List<ProxySource> _sources;

        public List<ProxySource> Sources
        {
            get { return _sources; }
            set { _sources = value; }
        }

        public ProxySourceCollection()
        {
            _sources = new List<ProxySource>();
        }

        public ProxySource FindSource(string url)
        {
            foreach (var source in _sources)
            {
                if (source.SourceUrl == url)
                    return source;
            }

            return null;
        }

        public void AddProxyToSource(string url, SystemProxy proxy)
        {
            foreach (var source in _sources)
            {
                if (source.SourceUrl == url)
                    source.Proxies.Add(proxy);
            }
        }

        public void ChangeFounded(string url, int value)
        {
            foreach (var source in _sources)
            {
                if (source.SourceUrl == url)
                    source.Founded = value;
            }
        }

        public void ChangeAdded(string url, int value)
        {
            foreach (var source in _sources)
            {
                if (source.SourceUrl == url)
                    source.Added = value;
            }
        }

        public void ChangeWorking(SystemProxy working)
        {
            foreach (var source in _sources)
            {
                if (source.IsExisted(working))
                    source.Working++;
            }
        }

        public void ChangeAnonymous(SystemProxy anonymous)
        {
            foreach (var source in _sources)
            {
                if (source.IsExisted(anonymous))
                {
                    source.Working++;
                    source.Anonymous++;
                }
            }
        }        
    }
}
