using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class BoundsData<T>
    {
        public int Width => width;
        public int Height => height;

        private int width;
        private int height;
        private int offsetX;
        private int offsetY;
        private int maxX;
        private int maxY;
        private T[,] data;

        public T this[Vector3Int position]
        {
            get => GetData(position);
            set => SetData(position, value);
        }
        
        public BoundsData(BoundsInt bounds, T initial = default)
        {
            width = bounds.size.x;
            height = bounds.size.y;
            offsetX = -bounds.min.x;
            offsetY = -bounds.min.y;
            maxX = width - 1;
            maxY = height - 1;
            data = new T[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    data[x, y] = initial;
                }
            }
        }

        public bool IsInBounds(Vector3Int position)
        {
            (int x, int y) = GetIndex(position);
            return IsInBounds(x, y);
        }

        public bool IsOutOfBounds(Vector3Int position)
        {
            (int x, int y) = GetIndex(position);
            return IsOutOfBounds(x, y);
        }

        public T GetData(Vector3Int position)
        {
            (int x, int y) = GetIndex(position);
            return GetData(x, y);
        }

        public bool DataEquals(Vector3Int position, T value)
        {
            return TryGetData(position, out T data) && data.Equals(value);
        }

        public bool TryGetData(Vector3Int position, out T value)
        {
            (int x, int y) = GetIndex(position);
            return TryGetData(x, y, out value);
        }

        public void SetData(Vector3Int position, T value)
        {
            (int x, int y) = GetIndex(position);
            SetData(x, y, value);
        }

        public (int x, int y) GetIndex(Vector3Int position)
        {
            return (position.x + offsetX, position.y + offsetY);
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x <= maxX && y <= maxY;
        }

        public bool IsOutOfBounds(int x, int y)
        {
            return !IsInBounds(x, y);
        }

        public T GetData(int x, int y)
        {
            return data[x, y];
        }

        public bool TryGetData(int x, int y, out T value)
        {
            if (IsInBounds(x, y))
            {
                value = GetData(x, y);
                return value != null;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public void SetData(int x, int y, T value)
        {
            data[x, y] = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    sb.Append(data[x, y]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}