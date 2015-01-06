using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.ThirdParties
{
    public class Token
    {
        public string Text { get; private set; }
        public string Replacement { get; private set; }
        public int Index { get; set; }

        public Token()
        {
        }

        public Token(string text, string replacement)
        {
            Text = text;
            Replacement = replacement;
        }
    }
}
