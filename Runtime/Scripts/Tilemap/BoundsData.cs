using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class BoundsData : BoundsData<object>
    {
        public BoundsData(BoundsInt bounds, object initial = null) : base(bounds, initial)
        {

        }
    }

    public class BoundsData<T> : ICloneable<BoundsData<T>>
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
            Initialize(bounds, initial);
        }

        public void Initialize(BoundsInt bounds, T initial = default)
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
            return IsInBounds(GetIndex(position));
        }

        public bool IsOutOfBounds(Vector3Int position)
        {
            return IsOutOfBounds(GetIndex(position));
        }

        public T GetData(Vector3Int position)
        {
            return GetData(GetIndex(position));
        }

        public bool DataEquals(Vector3Int position, T value)
        {
            return TryGetData(position, out T data) && data.Equals(value);
        }

        public bool TryGetData(Vector3Int position, out T value)
        {
            return TryGetData(GetIndex(position), out value);
        }

        public void SetData(Vector3Int position, T value)
        {
            SetData(GetIndex(position), value);
        }

        public bool TrySetData(Vector3Int position, T value)
        {
            return TrySetData(GetIndex(position), value);
        }

        public (int x, int y) GetIndex(Vector3Int position)
        {
            return (position.x + offsetX, position.y + offsetY);
        }

        public Vector3Int GetPosition((int x, int y) pos)
        {
            return new Vector3Int(pos.x - offsetX, pos.y - offsetY);
        }

        public bool IsInBounds((int x, int y) pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x <= maxX && pos.y <= maxY;
        }

        public bool IsOutOfBounds((int x, int y) pos)
        {
            return !IsInBounds(pos);
        }

        public T GetData((int x, int y) pos)
        {
            return data[pos.x, pos.y];
        }

        public bool TryGetData((int x, int y) pos, out T value)
        {
            if (IsInBounds(pos))
            {
                value = GetData(pos);
                return value != null;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public void SetData((int x, int y) pos, T value)
        {
            data[pos.x, pos.y] = value;
        }

        public bool TrySetData((int x, int y) pos, T value)
        {
            if (IsInBounds(pos))
            {
                data[pos.x, pos.y] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyDataFrom(BoundsData<T> other, BoundsInt bounds)
        {
            Initialize(bounds);

            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                if (other.TryGetData(position, out T data))
                {
                    if (TrySetData(position, data))
                    {
                        // Do nothing
                    }
                    else
                    {
                        Debug.LogError($"TrySetData failed since position '{position}' is out of bounds '{bounds}'.");
                    }
                }
                else
                {
                    Debug.LogError($"'other.TryGetData' failed since position '{position}' is out of bounds '{bounds}'.");
                }
            }
        }

        public BoundsData<T> Clone()
        {
            BoundsData<T> clone = (BoundsData<T>)MemberwiseClone();
            clone.data = (T[,]) data.Clone();
            return clone;
        }

        public BoundsData<T> Clone(BoundsInt bounds)
        {
            BoundsData<T> data = new BoundsData<T>(bounds);
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                data.SetData(position, GetData(position));
            }
            return data;
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