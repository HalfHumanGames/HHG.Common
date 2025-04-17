using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public static class RegexUtil
    {
        public static float ExtractFloat(string input)
        {
            float value = float.Parse(Regex.Match(input, @"-?\d*\.?\d+").Value);
            return input.Contains('%') ? value / 100f : value;
        }

        public static int ExtractInt(string input)
        {
            return int.Parse(Regex.Match(input, @"-?\d+").Value);
        }

        public static bool ExtractBool(string input)
        {
            return bool.Parse(Regex.Match(input, @"\b(true|false)\b", RegexOptions.IgnoreCase).Value.ToLower());
        }
    }
}
