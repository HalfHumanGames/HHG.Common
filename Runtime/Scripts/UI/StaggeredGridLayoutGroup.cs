using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [AddComponentMenu("Layout/Staggered Grid Layout Group")]
    public class StaggeredGridLayoutGroup : GridLayoutGroup
    {
        public enum RowOffset
        {
            Even,
            Odd
        }

        [SerializeField] protected RowOffset offsetRows = RowOffset.Odd;

        public RowOffset OffsetRows { get => offsetRows; set => SetProperty(ref offsetRows, value); }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SyncConstraintToStartAxis();
        }
#endif

        // FixedColumnCount/FixedRowCount control which axis is capped, independently
        // of StartAxis (which controls fill order), so it's easy to set StartAxis to
        // Vertical while Constraint is still FixedColumnCount and get no extra rows.
        // Keep them in sync so StartAxis alone decides the layout.
        private void SyncConstraintToStartAxis()
        {
            Constraint expected = startAxis == Axis.Horizontal ? Constraint.FixedColumnCount : Constraint.FixedRowCount;

            if (constraint != expected)
            {
                constraint = expected;
            }
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        // Copied from GridLayoutGroup.SetCellsAlongAxis (it's private there, so it
        // can't be reused via inheritance) with a half-cell-width offset added for
        // whichever rows OffsetRows selects.
        private void SetCellsAlongAxis(int axis)
        {
            int rectChildrenCount = rectChildren.Count;
            if (axis == 0)
            {
                for (int i = 0; i < rectChildrenCount; i++)
                {
                    RectTransform rect = rectChildren[i];

                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = cellSize;
                }
                return;
            }

            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            int cellCountX = 1;
            int cellCountY = 1;
            if (constraint == Constraint.FixedColumnCount)
            {
                cellCountX = constraintCount;

                if (rectChildrenCount > cellCountX)
                    cellCountY = rectChildrenCount / cellCountX + (rectChildrenCount % cellCountX > 0 ? 1 : 0);
            }
            else if (constraint == Constraint.FixedRowCount)
            {
                cellCountY = constraintCount;

                if (rectChildrenCount > cellCountY)
                    cellCountX = rectChildrenCount / cellCountY + (rectChildrenCount % cellCountY > 0 ? 1 : 0);
            }
            else
            {
                if (cellSize.x + spacing.x <= 0)
                    cellCountX = int.MaxValue;
                else
                    cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

                if (cellSize.y + spacing.y <= 0)
                    cellCountY = int.MaxValue;
                else
                    cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
            }

            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            if (startAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildrenCount);

                if (constraint == Constraint.FixedRowCount)
                    actualCellCountY = Mathf.Min(cellCountY, rectChildrenCount);
                else
                    actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildrenCount);

                if (constraint == Constraint.FixedColumnCount)
                    actualCellCountX = Mathf.Min(cellCountX, rectChildrenCount);
                else
                    actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
            }

            // Add a half step (cell + spacing) of width so centering accounts for the staggered rows
            Vector2 requiredSpace = new Vector2(
                actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x + (cellSize.x + spacing.x) * 0.5f,
                actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
            );
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
            );

            // Fixes case 1345471 - Makes sure the constraint column / row amount is always respected
            int childrenToMove = 0;
            if (rectChildrenCount > constraintCount && Mathf.CeilToInt((float)rectChildrenCount / (float)cellsPerMainAxis) < constraintCount)
            {
                childrenToMove = constraintCount - Mathf.CeilToInt((float)rectChildrenCount / (float)cellsPerMainAxis);
                childrenToMove += Mathf.FloorToInt((float)childrenToMove / ((float)cellsPerMainAxis - 1));
                if (rectChildrenCount % cellsPerMainAxis == 1)
                    childrenToMove += 1;
            }

            for (int i = 0; i < rectChildrenCount; i++)
            {
                int positionX;
                int positionY;
                if (startAxis == Axis.Horizontal)
                {
                    if (constraint == Constraint.FixedRowCount && rectChildrenCount - i <= childrenToMove)
                    {
                        positionX = 0;
                        positionY = constraintCount - (rectChildrenCount - i);
                    }
                    else
                    {
                        positionX = i % cellsPerMainAxis;
                        positionY = i / cellsPerMainAxis;
                    }
                }
                else
                {
                    if (constraint == Constraint.FixedColumnCount && rectChildrenCount - i <= childrenToMove)
                    {
                        positionX = constraintCount - (rectChildrenCount - i);
                        positionY = 0;
                    }
                    else
                    {
                        positionX = i / cellsPerMainAxis;
                        positionY = i % cellsPerMainAxis;
                    }
                }

                if (cornerX == 1)
                    positionX = actualCellCountX - 1 - positionX;
                if (cornerY == 1)
                    positionY = actualCellCountY - 1 - positionY;

                bool isOffsetRow = offsetRows == RowOffset.Odd ? (positionY % 2 == 1) : (positionY % 2 == 0);
                float rowOffsetX = isOffsetRow ? (cellSize.x + spacing.x) * 0.5f : 0f;

                SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX + rowOffsetX, cellSize[0]);
                SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
            }
        }
    }
}
