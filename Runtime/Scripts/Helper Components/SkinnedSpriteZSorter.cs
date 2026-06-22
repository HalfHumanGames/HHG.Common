using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace HHG.Common.Runtime
{
    // This sorts skinned sprites by offsetting the z position
    // Z sorting is only necessary if using a non-sprite shader
    public class SkinnedSpriteZSorter : MonoBehaviour
    {
        private const float zOffsetMultiplier = -.0001f;
        private readonly int zOffsetPropertyID = Shader.PropertyToID("_ZOffset");

        [SerializeField] private Mode mode;

        private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        private List<SpriteSkin> spriteSkins = new List<SpriteSkin>();
        private List<Material> spriteMaterials = new List<Material>();

        private enum Mode
        {
            OnEnable,
            LateUpdate,
            Manual
        }

        private void Awake()
        {
            GetComponentsInChildren(spriteRenderers);
            spriteSkins.AddRange(spriteRenderers.Select(s => s.GetComponent<SpriteSkin>()));
            spriteMaterials.AddRange(spriteRenderers.Select(s => s.material));
        }

        private void OnEnable()
        {
            if (mode == Mode.OnEnable) SetZOffsets();
        }

        private void LateUpdate()
        {
            if (mode == Mode.LateUpdate) SetZOffsets();
        }

        [ContextMenu("Set Z Offsets")]
        public void SetZOffsets()
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                SpriteSkin spriteSkin = spriteSkins[i];
                Material spriteMaterial = spriteMaterials[i];

                float zOffset = spriteRenderer.sortingOrder * zOffsetMultiplier;
                spriteMaterial.SetFloat(zOffsetPropertyID, zOffset);

                if (spriteSkin)
                {
                    foreach (Transform boneTransform in spriteSkin.boneTransforms)
                    {
                        boneTransform.SetLocalPositionZ(0f);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (Material spriteMaterial in spriteMaterials)
            {
                if (spriteMaterial) Destroy(spriteMaterial);
            }
        }
    }
}
