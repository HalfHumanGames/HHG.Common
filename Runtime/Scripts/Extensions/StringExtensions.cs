using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class StringExtensions
    {
        public static string ReplaceMany(this string s, char[] search, char replace)
        {
            string[] temp = s.Split(search, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(replace, temp);
        }

        public static string ReplaceMany(this string s, string[] search, string replace)
        {
            string[] temp = s.Split(search, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(replace, temp);
        }

        public static string ReplaceMany(this string s, char[] search, string replace)
        {
            string[] temp = s.Split(search, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(replace, temp);
        }

        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        public static string ToNicified(this string s)
        {
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).ToTitleCase().Trim();
        }
    }
}