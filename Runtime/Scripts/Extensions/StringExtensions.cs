using System;

namespace HHG.Common
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
    }
}