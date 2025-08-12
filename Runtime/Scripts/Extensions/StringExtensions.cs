using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class StringExtensions
    {
        public static string RemoveBacketedText(this string s)
        {
            return Regex.Replace(s, @"\s*[\(\[\<][^)\]\>]*[\)\]\>]", string.Empty);
        }

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

        public static string Format(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static string Format(this string s, IDictionary<string, string> parameters)
        {
            foreach (KeyValuePair<string, string> kvpair in parameters)
            {
                s = Regex.Replace(s, $"{{{kvpair.Key}}}", kvpair.Value);
            }

            return s;
        }
    }
}