using System.Text;

namespace HHG.Common.Runtime
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Trim(this StringBuilder sb)
        {
            while (sb.Length > 0 && char.IsWhiteSpace(sb[^1]))
            {
                sb.Length--;
            }

            return sb;
        }
    }
}