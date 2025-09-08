using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public static class GridLayoutGroupExtensions
    {
        public static IEnumerable<GameObject> GetFirstRowItems(this GridLayoutGroup group)
        {
            var (cols, _) = GetGridDimensions(group);
            return group.GetRowItems(0, cols);
        }

        public static IEnumerable<GameObject> GetLastRowItems(this GridLayoutGroup group)
        {
            var (cols, rows) = GetGridDimensions(group);
            return group.GetRowItems(rows - 1, cols);
        }

        public static IEnumerable<GameObject> GetFirstColumnItems(this GridLayoutGroup group)
        {
            var (cols, rows) = GetGridDimensions(group);
            return group.GetColumnItems(0, cols, rows);
        }

        public static IEnumerable<GameObject> GetLastColumnItems(this GridLayoutGroup group)
        {
            var (cols, rows) = GetGridDimensions(group);
            return group.GetColumnItems(cols - 1, cols, rows);
        }

        public static (int cols, int rows) GetGridDimensions(this GridLayoutGroup group)
        {
            int childCount = group.transform.GetChildren().Count(c => c.gameObject.activeSelf);

            if (group.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                int cols = group.constraintCount;
                int rows = Mathf.CeilToInt((float)childCount / cols);
                return (cols, rows);
            }
            else if (group.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                int rows = group.constraintCount;
                int cols = Mathf.CeilToInt((float)childCount / rows);
                return (cols, rows);
            }
            else // Flexible: Try to infer based on rect size
            {
                var rect = group.GetComponent<RectTransform>().rect;
                int cols = Mathf.FloorToInt((rect.width + group.spacing.x) / (group.cellSize.x + group.spacing.x));
                cols = Mathf.Max(1, cols);
                int rows = Mathf.CeilToInt((float)childCount / cols);
                return (cols, rows);
            }
        }

        public static IEnumerable<GameObject> GetRowItems(this GridLayoutGroup group, int row, int cols)
        {
            int startIndex = row * cols;
            int endIndex = startIndex + cols;
            int activeIdx = 0;

            int total = group.transform.childCount;
            for (int i = 0; i < total; i++)
            {
                GameObject child = group.transform.GetChild(i).gameObject;
                if (!child.activeSelf) continue;

                if (activeIdx >= endIndex) yield break;
                if (activeIdx >= startIndex) yield return child;

                activeIdx++;
            }
        }

        public static IEnumerable<GameObject> GetColumnItems(this GridLayoutGroup group, int col, int cols, int rows)
        {
            int activeIdx = 0;
            int total = group.transform.childCount;

            for (int i = 0; i < total; i++)
            {
                GameObject child = group.transform.GetChild(i).gameObject;
                if (!child.activeSelf) continue;

                int r = activeIdx / cols;
                int c = activeIdx % cols;

                if (r >= rows) yield break;
                if (c == col) yield return child;

                activeIdx++;
            }
        }

    }
}
