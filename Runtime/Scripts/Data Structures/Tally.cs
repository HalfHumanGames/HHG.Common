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
        private const int min = 33;
        private const int max = 126;
        private const int range = max - min + 1;

        [SerializeField] private string tally;

        public Tally(string data = null)
        {
            tally = data ?? string.Empty;
        }

        public int Get(int index)
        {
            return !string.IsNullOrEmpty(tally) && index < tally.Length ? tally[index] - min : 0;
        }

        public void Set(int index, int value)
        {
            if (value < 0 || value >= range)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and {range - 1}.");
            }

            StringBuilder sb = new StringBuilder(tally ?? string.Empty);

            while (sb.Length <= index)
            {
                sb.Append((char)min);
            }

            sb[index] = (char)(min + value);
            tally = sb.ToString();
        }

        public string ToDebugString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tally.Length; i++)
            {
                sb.AppendLine($"{i}: {Get(i)}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return tally ?? string.Empty;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (string.IsNullOrEmpty(tally))
            {
                yield break;
            }

            for (int i = 0; i < tally.Length; i++)
            {
                yield return Get(i);
            }
        }

        public static implicit operator string(Tally tally) => tally.tally ?? string.Empty;
        public static implicit operator Tally(string value) => new Tally(value);
    }
}
