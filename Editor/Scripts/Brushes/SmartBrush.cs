using HHG.Common.Runtime;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Editor
{
    [CustomGridBrush(true, true, true, "Smart Brush")]
    public class SmartBrush : GridBrush
    {
        public bool Log;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindPaintTarget(brushTarget);
            }
            
            if (Log)
            {
                Debug.Log(position);
            }

            base.Paint(gridLayout, brushTarget, position);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (Event.current.type == EventType.MouseDown && IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindTopmostPaintTarget(brushTarget, position);
            }

            base.Erase(gridLayout, brushTarget, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds)
        {
            if ((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindPaintTarget(brushTarget);
            }

            base.BoxFill(gridLayout, brushTarget, bounds);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds)
        {
            if (Event.current.type == EventType.MouseDown && IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindTopmostPaintTarget(brushTarget, bounds);
            }

            base.BoxErase(gridLayout, brushTarget, bounds);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindPaintTarget(brushTarget);
            }

            base.FloodFill(gridLayout, brushTarget, position);
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds, Vector3Int pickStart)
        {
            if (IsBrushTargetSceneTilemap(brushTarget))
            {
                GridPaintingState.scenePaintTarget = brushTarget = FindTopmostPaintTarget(brushTarget, bounds);
            }

            base.Pick(gridLayout, brushTarget, bounds, pickStart);
        }

        private GameObject FindPaintTarget(GameObject brushTarget)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                BrushCell cell = cells[i];

                if (cell != null && cell.tile is ISmartTile smartTile)
                {
                    if (smartTile.TargetTilemapLayer == null || string.IsNullOrEmpty(smartTile.TargetTilemapLayer.name))
                    {
                        continue;
                    }

                    Transform target = brushTarget.transform.parent.Find(smartTile.TargetTilemapLayer.name);

                    if (target == null)
                    {
                        target = GameObject.Find(smartTile.TargetTilemapLayer.name)?.transform;
                    }

                    if (target != null && target.TryGetComponent<Tilemap>(out _))
                    {
                        return target.gameObject;
                    }
                }
            }

            return brushTarget;
        }

        private GameObject FindTopmostPaintTarget(GameObject brushTarget, Vector3Int position)
        {
            Transform parent = brushTarget.transform.parent;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                if (parent.GetChild(i).TryGetComponent(out Tilemap tilemap))
                {
                    if (tilemap.GetTile(position) != null)
                    {
                        return tilemap.gameObject;
                    }
                }
            }

            return brushTarget;
        }

        private GameObject FindTopmostPaintTarget(GameObject brushTarget, BoundsInt bounds)
        {
            Transform parent = brushTarget.transform.parent;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                foreach (Vector3Int position in bounds.allPositionsWithin)
                {
                    if (parent.GetChild(i).TryGetComponent(out Tilemap tilemap))
                    {
                        if (tilemap.GetTile(position) != null)
                        {
                            return tilemap.gameObject;
                        }
                    }
                }
            }

            return brushTarget;
        }

        private bool IsBrushTargetSceneTilemap(GameObject brushTarget)
        {
            return brushTarget.scene.name != "Preview Scene";
        }

        [CustomEditor(typeof(SmartBrush))]
        public class SmartBrushEditor : GridBrushEditor
        {

        }
    }
}