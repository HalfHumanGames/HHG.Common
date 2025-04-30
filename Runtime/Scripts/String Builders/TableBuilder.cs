using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HHG.Common.Runtime
{
    public class TableBuilder
    {
        private static readonly Regex richTextRegex = new(@"<.*?>", RegexOptions.Compiled);

        private readonly StringBuilder sb = new();
        private readonly int[] widths;
        private readonly Align[] alignments;

        private enum Align { Left, Center, Right }

        public TableBuilder(params int[] columns)
        {
            widths = columns;
            alignments = new Align[columns.Length];
            Array.Fill(alignments, Align.Left);
        }

        public TableBuilder(params string[] columns)
        {
            widths = new int[columns.Length];
            alignments = new Align[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                widths[i] = int.Parse(Regex.Match(columns[i], @"^\d+").Value);
                alignments[i] = ParseAlignment(Regex.Match(columns[i], @"[LCR]").Value);
            }
        }

        public void AppendRow() => AppendRow(string.Empty);

        public void AppendRow(params object[] row) => AppendRow((IEnumerable<object>)row);

        public void AppendRow(IEnumerable<object> row)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            int i = 0;
            foreach (object item in row)
            {
                if (i >= widths.Length)
                {
                    break;
                }

                string cell = item?.ToString() ?? string.Empty;
                int width = widths[i];
                int visualLength = GetLength(cell);
                int padding = Math.Max(0, width - visualLength);

                switch (alignments[i])
                {
                    case Align.Left:
                    default:
                        sb.Append(cell);
                        sb.Append(' ', padding);
                        break;
                    case Align.Center:
                        int padLeft = padding / 2;
                        int padRight = padding - padLeft;
                        sb.Append(' ', padLeft);
                        sb.Append(cell);
                        sb.Append(' ', padRight);
                        break;
                    case Align.Right:
                        sb.Append(' ', padding);
                        sb.Append(cell);
                        break;
                }

                i++;
            }
        }

        private int GetLength(string str) => richTextRegex.Replace(str, "").Length;

        private Align ParseAlignment(string alignment)
        {
            return alignment switch
            {
                "L" => Align.Left,
                "C" => Align.Center,
                "R" => Align.Right,
                _ => Align.Left
            };
        }
        public void Clear() => sb.Clear();

        public override string ToString() => sb.ToString();
    }
}
