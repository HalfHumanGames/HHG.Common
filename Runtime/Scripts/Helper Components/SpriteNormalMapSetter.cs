using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteNormalMapSetter : MonoBehaviour
    {
        private const string normalMapName = "_NormalMap";
        private static readonly int normalMapID = Shader.PropertyToID(normalMapName);

        private SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock materialPropertyBlock;
        private Sprite lastSprite;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        private void LateUpdate()
        {
            Sprite currentSprite = spriteRenderer.sprite;

            if (currentSprite != lastSprite)
            {
                lastSprite = currentSprite;
                ApplyNormalMap();
            }
        }

        private void ApplyNormalMap()
        {
            int count = spriteRenderer.sprite.GetSecondaryTextureCount();

            if (count == 0) return;

            SecondarySpriteTexture[] secondaries = new SecondarySpriteTexture[count];
            spriteRenderer.sprite.GetSecondaryTextures(secondaries);
            SecondarySpriteTexture nomral = secondaries.FirstOrDefault(s => s.name == normalMapName);
            Texture2D texture = nomral.texture;
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetTexture(normalMapID, texture);
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

}