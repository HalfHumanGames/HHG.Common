using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
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