using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public class SnapToTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Space offsetSpace;
        [SerializeField] private Mode mode;

        private enum Mode
        {
            LateUpdate,
            OnEnable,
            Manual
        }


        private void Awake()
        {
            if (tilemap == null) tilemap = GetComponentInChildren<Tilemap>(true);
            if (tilemap == null) tilemap = FindAnyObjectByType<Tilemap>(FindObjectsInactive.Include);
        }

        private void OnEnable()
        {
            if (mode == Mode.OnEnable) Snap();
        }


        private void LateUpdate()
        {
            // Always snap when in edit mode
            if (!Application.isPlaying || mode == Mode.LateUpdate) Snap();
        }

        public void Snap()
        {
            if (!transform.hasChanged) return;

            transform.position = tilemap.WorldToCellWorld(transform.position);

            if (offsetSpace == Space.Self) transform.localPosition += offset;
            if (offsetSpace == Space.World) transform.position += offset;

            transform.hasChanged = false;
        }
    }
}