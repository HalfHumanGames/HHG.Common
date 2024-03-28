using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Tilemap))]
    public class SnapToTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private void Awake()
        {
            tilemap ??= GetComponent<Tilemap>();
        }

        private void LateUpdate()
        {
            transform.position = tilemap.WorldToCellWorld(transform.position);
        }
    }
}