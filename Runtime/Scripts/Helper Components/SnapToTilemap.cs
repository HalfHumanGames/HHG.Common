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

        private void Awake()
        {
            if (tilemap == null) tilemap = GetComponentInChildren<Tilemap>(true);
            if (tilemap == null) tilemap = FindAnyObjectByType<Tilemap>(FindObjectsInactive.Include);
        }


        // Optimize so have mode: OnEnable, LateUpdate, or Manual
        private void LateUpdate()
        {
            transform.position = tilemap.WorldToCellWorld(transform.position);

            if (offsetSpace == Space.Self) transform.localPosition += offset;
            if (offsetSpace == Space.World) transform.position += offset;
        }
    }
}