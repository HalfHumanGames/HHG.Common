using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class CsvUtil
    {
        public static List<List<string>> Parse(string csv)
        {
            List<List<string>> rows = new List<List<string>>();
            Parse(csv, rows);
            return rows;
        }

        public static void Parse(string csv, List<List<string>> rows)
        {
            rows.Clear();

            string[] lines = csv.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                List<string> parsedLine = new List<string>();
                ParseLine(line, parsedLine);
                rows.Add(parsedLine);
            }
        }

        public static List<string> ParseLine(string line)
        {
            List<string> values = new List<string>();
            ParseLine(line, values);
            return values;
        }

        public static void ParseLine(string line, List<string> values)
        {
            values.Clear();

            Regex regex = new Regex(
                @"
                (?:^|,)               # Start of the line or a comma
                (                     # Start of capture group
                    ""([^""]*)""      # Quoted field (handles escaped quotes inside quotes)
                    |                 # OR
                    ([^"",]*)         # Unquoted field (no commas or quotes allowed)
                )                     # End of capture group
                (?=,|$)               # Followed by a comma or end of line
                ",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

            foreach (Match match in regex.Matches(line))
            {
                values.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
            }
        }
    }
}
