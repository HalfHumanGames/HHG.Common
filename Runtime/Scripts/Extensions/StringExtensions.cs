using System;
using System.Globalization;

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
    }
}