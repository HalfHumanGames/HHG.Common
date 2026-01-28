using UnityEngine;
using UnityEngine.U2D.Animation;

namespace HHG.Common.Runtime
{
    // This sorts skinned sprites by offsetting the z position
    // Z sorting is only necessary if using a non-sprite shader
    [ExecuteAlways]
    public class SkinnedSpriteZSorter : MonoBehaviour
    {
        private const float zOffsetMultiplier = -.0001f;

        private readonly int zOffsetPropertyID = Shader.PropertyToID("_ZOffset");

        private MaterialPropertyBlock materialPropertyBlock;

        private void Awake()
        {
            SetZOffsets();
        }

        [ContextMenu("Set Z Offsets")]
        private void SetZOffsets()
        {
            materialPropertyBlock ??= new MaterialPropertyBlock();

            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.GetPropertyBlock(materialPropertyBlock);
                float zOffset = spriteRenderer.sortingOrder * zOffsetMultiplier;
                materialPropertyBlock.SetFloat(zOffsetPropertyID, zOffset);
                spriteRenderer.SetPropertyBlock(materialPropertyBlock);

                if (spriteRenderer.TryGetComponent(out SpriteSkin skin))
                {
                    foreach (Transform boneTransform in skin.boneTransforms)
                    {
                        boneTransform.SetLocalPositionZ(0f);
                    }
                }
            }
        }
    }
}
