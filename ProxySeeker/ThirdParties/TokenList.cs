using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeeker.ThirdParties
{
    public class TokenList : List<Token>
    {
        public void Add(string text, string replacement)
        {
            Add(new Token(text, replacement));
        }

        private Token GetFirstToken()
        {
            Token result = null;

            int index = int.MaxValue;

            foreach (Token token in this)
            {
                if (token.Index != -1 && token.Index < index)
                {
                    index = token.Index;
                    result = token;
                }
            }

            return result;
        }

        public string Replace(string text)
        {
            StringBuilder result = new StringBuilder();
            foreach (Token token in this)
            {
                token.Index = text.IndexOf(token.Text);
            }

            int index = 0;
            Token next = null;
            while ((next = GetFirstToken()) != null)
            {
                if (index < next.Index)
                {
                    result.Append(text, index, next.Index - index);
                    index = next.Index;
                }
                result.Append(next.Replacement);
                index += next.Text.Length;
                foreach (Token token in this)
                {
                    if (token.Index != -1 && token.Index < index)
                    {
                        token.Index = text.IndexOf(token.Text, index);
                    }
                }
            }
            if (index < text.Length)
            {
                result.Append(text, index, text.Length - index);
            }
            return result.ToString();
        }
    }
}
