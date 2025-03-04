using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [Serializable]
    public struct Tally : IEnumerable<int>
    {
        [SerializeField] private string tally;

        public Tally(string tally = null)
        {
            this.tally = tally ?? string.Empty;
        }

        public int Get(int index)
        {
            tally ??= string.Empty;

            return index < tally.Length ? tally[index] : 0;
        }

        public void Set(int index, int value)
        {
            tally ??= string.Empty;

            if (value < 0 || value > char.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and {char.MaxValue}.");
            }

            StringBuilder sb = new StringBuilder(tally);

            while (sb.Length <= index)
            {
                sb.Append('\0');
            }

            sb[index] = (char)value;
            tally = sb.ToString();
        }

        public override string ToString() => tally ?? string.Empty;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator()
        {
            tally ??= string.Empty;

            for (int i = 0; i < tally.Length; i++)
            {
                yield return tally[i];
            }
        }

        public static implicit operator string(Tally tally)
        {
            return tally.tally ?? string.Empty;;
        }
        public static implicit operator Tally(string value)
        {
            return new Tally(value);
        }
    }
}