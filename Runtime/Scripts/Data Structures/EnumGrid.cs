using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class EnumGrid
    {
        public abstract bool IsInitialized { get; }
        public abstract bool IsValid { get; }
        public abstract void Initialize(int newSize);
        public abstract int GetCellWeak(Vector3Int position);
        public abstract void SetCellWeak(Vector3Int position, int value);
        public abstract Color GetColorWeak(int value);
    }

    public abstract class EnumGrid<T> : EnumGrid, IEnumerable<Vector3Int> where T : Enum
    {
        public override bool IsInitialized => grid != null && grid.Length == size * size;
        public override bool IsValid => size > 0;

        public T this[Vector3Int position]
        {
            get => GetCell(position);
            set => SetCell(position, value);
        }

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

        public T GetCell(Vector3Int position)
        {
            return grid[position.y * size + position.x];
        }

        public void SetCell(Vector3Int position, T value)
        {
            grid[position.y * size + position.x] = value;
        }

        public abstract Color GetColor(T value);

        public override int GetCellWeak(Vector3Int position)
        {
            return Convert.ToInt32(GetCell(position));
        }

        public override void SetCellWeak(Vector3Int position, int value)
        {
            SetCell(position, (T)Enum.ToObject(typeof(T), value));
        }

        public override Color GetColorWeak(int value)
        {
            return GetColor((T)Enum.ToObject(typeof(T), value));
        }

        public IEnumerator<Vector3Int> GetEnumerator()
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    yield return new Vector3Int(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}