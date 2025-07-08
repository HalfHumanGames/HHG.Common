using UnityEngine;

namespace HHG.Common.Runtime
{
    public struct Link
    {
        public static readonly Link Invalid = new() { Index = -1 };
        public bool IsValid => Index >= 0;
        public int Index;
        public string Id;
        public string Text;
        public Rect Rect;
        public int LineNumber;
        public int Start;
        public int End;
        public int Length;
    }
}