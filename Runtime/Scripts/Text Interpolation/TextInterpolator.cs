using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HHG.Common.Runtime
{
    public static class TextInterpolator
    {
        public static string[] Interpolate(IEnumerable<string> texts, IReadOnlyDictionary<string, object> variables)
        {
            string[] retval = new string[texts.Count()];

            int i = 0;

            foreach (string text in texts)
            {
                retval[i++] = Interpolate(text, variables);
            }

            return retval;
        }

        public static string Interpolate(string text, IReadOnlyDictionary<string, object> variables)
        {
            StringBuilder sb = new StringBuilder(text);

            foreach (KeyValuePair<string, object> kvpair in variables)
            {
                sb.Replace("{" + kvpair.Key + "}", kvpair.Value.ToString());
            }

            return sb.ToString();
        }
    }
}