using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class EnumGrid<T> : IEnumerable<KeyValuePair<Vector3Int, T>> where T : Enum
    {
        public bool IsValid => sprite != null;

        public T this[Vector3Int position]
        {
            get => grid[position];
            set => grid[position] = value;
        }

        [SerializeField, AssetPreview] private Sprite sprite;

        private Dictionary<Vector3Int, T> _grid;
        private Dictionary<Vector3Int, T> grid
        {
            get
            {
                if (_grid == null && sprite != null)
                {
                    _grid = new Dictionary<Vector3Int, T>();

                    Color[,] pixels = sprite.GetPixels();

                    pixels.ForEach((x, y, color) =>
                    {
                        if (color != Color.clear)
                        {
                            Vector3Int position = new Vector3Int(x, y);
                            T value = GetValue(color);
                            _grid[position] = value;
                        }
                    });
                }

                return _grid;
            }
        }

        public abstract Color GetColor(T value);
        public abstract T GetValue(Color color);

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public IEnumerator<KeyValuePair<Vector3Int, T>> GetEnumerator()
        {
            return grid.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}