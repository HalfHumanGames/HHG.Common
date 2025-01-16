using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class EnumGrid
    {
        public abstract bool IsInitialized { get; }
        public abstract void Initialize(int newSize);
        public abstract int GetCellWeak(int x, int y);
        public abstract void SetCellWeak(int x, int y, int value);
        public abstract Color GetColorWeak(int value);
    }

    public abstract class EnumGrid<T> : EnumGrid where T : Enum
    {
        public override bool IsInitialized => grid != null && grid.Length == size * size;

        [SerializeField] private int size;
        [SerializeField] private T[] grid;

        public EnumGrid(int size)
        {
            Initialize(size);
        }

        public override void Initialize(int newSize)
        {
            size = newSize;

            if (grid == null)
            {
                grid = new T[size * size];
            }
            else
            {
                Array.Resize(ref grid, size * size);
            }
        }

        public T GetCell(int x, int y)
        {
            return grid[y * size + x];
        }

        public void SetCell(int x, int y, T value)
        {
            grid[y * size + x] = value;
        }

        public abstract Color GetColor(T value);

        public override int GetCellWeak(int x, int y)
        {
            return Convert.ToInt32(GetCell(x, y));
        }

        public override void SetCellWeak(int x, int y, int value)
        {
            SetCell(x, y, (T)Enum.ToObject(typeof(T), value));
        }

        public override Color GetColorWeak(int value)
        {
            return GetColor((T)Enum.ToObject(typeof(T), value));
        }
    }
}