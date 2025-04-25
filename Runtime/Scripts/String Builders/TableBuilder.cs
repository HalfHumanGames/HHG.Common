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

        public enum Align { Left, Right }

        public TableBuilder(params int[] widths) : this(widths, null)
        {
            
        }

        public TableBuilder(int[] widths, Align[] alignments = null)
        {
            this.widths = widths;
            this.alignments = alignments ?? GetDefaultAlignments(widths.Length);
        }

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

                if (alignments[i] == Align.Right)
                {
                    sb.Append(' ', padding);
                    sb.Append(cell);
                }
                else
                {
                    sb.Append(cell);
                    sb.Append(' ', padding);
                }

                i++;
            }
        }

        private int GetLength(string str) => richTextRegex.Replace(str, "").Length;

        private static Align[] GetDefaultAlignments(int count)
        {
            Align[] alignments = new Align[count];
            Array.Fill(alignments, Align.Left, 0, count);
            return alignments;
        }

        public void Clear() => sb.Clear();

        public override string ToString() => sb.ToString();
    }
}
