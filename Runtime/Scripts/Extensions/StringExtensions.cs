using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class StringExtensions
    {
        private static StringBuilder sb = new StringBuilder();

        public static string PrependLines(this string text, string prefix)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (string.IsNullOrEmpty(prefix)) return text;

            sb.Clear();
            sb.Append(prefix);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                sb.Append(c);

                if (c == '\n' && i < text.Length - 1)
                {
                    sb.Append(prefix);
                }
            }

            return sb.ToString();
        }

        public static string WordWrap(this string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (maxLineLength <= 0) throw new ArgumentException("'length' must be > 0");

            sb.Clear();
            StringBuilder line = new StringBuilder();
            int visibleLength = 0;

            foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                int wordVisibleLen = word.CountVisibleCharacters();

                if (visibleLength > 0 && visibleLength + 1 + wordVisibleLen > maxLineLength)
                {
                    sb.AppendLine(line.ToString());
                    line.Clear();
                    visibleLength = 0;
                }

                if (visibleLength > 0)
                {
                    line.Append(' ');
                    visibleLength++;
                }

                line.Append(word);
                visibleLength += wordVisibleLen;
            }

            if (line.Length > 0) sb.Append(line);

            return sb.ToString();
        }

        public static int CountVisibleCharacters(this string s)
        {
            if (s == null) return 0;

            int count = 0;
            bool inTag = false;

            foreach (char c in s)
            {
                if (c == '<')
                {
                    inTag = true;
                    continue;
                }
                if (c == '>' && inTag)
                {
                    inTag = false;
                    continue;
                }
                if (!inTag)
                    count++;
            }
            return count;
        }

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