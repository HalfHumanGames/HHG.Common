using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [AddComponentMenu("Layout/Diamond Layout Group")]
    public class DiamondLayoutGroup : LayoutGroup
    {
        public enum OrderMode
        {
            CenterOut,
            LeftToRight
        }

        public enum RowStart
        {
            High,
            Low
        }

        [SerializeField] protected Vector2 cellSize = new Vector2(120f, 120f);
        [SerializeField] protected Vector2 spacing = new Vector2(6f, 6f);
        [SerializeField] protected OrderMode order = OrderMode.CenterOut;
        [SerializeField, ShowIf(nameof(order), OrderMode.CenterOut)] protected bool firstGoesRight = true;
        [SerializeField] protected RowStart firstRow = RowStart.High;
        [SerializeField, ShowIf(nameof(order), OrderMode.CenterOut)] protected bool pinCenterSlot = true;

        public Vector2 CellSize { get => cellSize; set => SetProperty(ref cellSize, value); }
        public Vector2 Spacing { get => spacing; set => SetProperty(ref spacing, value); }
        public OrderMode Order { get => order; set => SetProperty(ref order, value); }
        public bool FirstGoesRight { get => firstGoesRight; set => SetProperty(ref firstGoesRight, value); }
        public RowStart FirstRow { get => firstRow; set => SetProperty(ref firstRow, value); }
        public bool PinCenterSlot { get => pinCenterSlot; set => SetProperty(ref pinCenterSlot, value); }

        private Vector2[] positions = new Vector2[0];
        private float minX, maxX, minY, maxY;

        private float GetColumnIndex(int childIndex, int childCount)
        {
            if (order == OrderMode.LeftToRight)
            {
                return childIndex - (childCount - 1) * 0.5f;
            }

            if (childIndex == 0)
            {
                return 0f;
            }

            int steps = (childIndex + 1) / 2;
            bool goesRight = (childIndex % 2 == 1) == firstGoesRight;
            return goesRight ? steps : -steps;
        }

        private void ComputePositions()
        {
            int childCount = rectChildren.Count;
            if (positions.Length != childCount)
            {
                positions = new Vector2[childCount];
            }

            float stepX = cellSize.x * 0.5f + spacing.x;
            float rowOffset = cellSize.y * 0.5f + spacing.y;
            float oddRowSign = firstRow == RowStart.High ? -1f : 1f;

            minX = minY = float.MaxValue;
            maxX = maxY = float.MinValue;

            for (int i = 0; i < childCount; i++)
            {
                float columnIndex = GetColumnIndex(i, childCount);
                bool onOddRow = order == OrderMode.LeftToRight
                    ? (i % 2) == 1
                    : Mathf.RoundToInt(Mathf.Abs(columnIndex)) % 2 == 1;
                float positionX = columnIndex * stepX;
                float positionY = onOddRow ? oddRowSign * rowOffset : 0f;

                positions[i] = new Vector2(positionX, positionY);
                minX = Mathf.Min(minX, positionX); maxX = Mathf.Max(maxX, positionX);
                minY = Mathf.Min(minY, positionY); maxY = Mathf.Max(maxY, positionY);
            }

            if (childCount == 0)
            {
                minX = maxX = minY = maxY = 0f;
            }
        }

        private float contentWidth => (maxX - minX) + cellSize.x;
        private float contentHeight => (maxY - minY) + cellSize.y;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            ComputePositions();
            float width = padding.horizontal + contentWidth;
            SetLayoutInputForAxis(width, width, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float height = padding.vertical + contentHeight;
            SetLayoutInputForAxis(height, height, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            float start;

            if (pinCenterSlot && order == OrderMode.CenterOut)
            {
                start = padding.left + (rectTransform.rect.width - padding.horizontal) * 0.5f - cellSize.x * 0.5f;
            }
            else
            {
                start = GetStartOffset(0, contentWidth) - minX;
            }

            for (int i = 0; i < rectChildren.Count; i++)
            {
                SetChildAlongAxis(rectChildren[i], 0, start + positions[i].x, cellSize.x);
            }
        }

        public override void SetLayoutVertical()
        {
            float top = GetStartOffset(1, contentHeight);
            for (int i = 0; i < rectChildren.Count; i++)
            {
                SetChildAlongAxis(rectChildren[i], 1, top + (maxY - positions[i].y), cellSize.y);
            }
        }
    }
}
