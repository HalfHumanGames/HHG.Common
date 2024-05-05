using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class TagUtil
    {
        public static string GetTag(string name)
        {
            return Regex.Replace(name, @"[\(\[\{].*?[\)\]\}]|\s+|[^a-zA-Z\s]", "");
        }
    }
}