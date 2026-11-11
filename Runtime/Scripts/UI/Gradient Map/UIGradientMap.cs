using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [ExecuteAlways, RequireComponent(typeof(Graphic))]
    public class UIGradientMap : BaseMeshEffect, IMaterialModifier
    {
        private static readonly int stencilId = Shader.PropertyToID("_Stencil");
        private static readonly int stencilOpId = Shader.PropertyToID("_StencilOp");
        private static readonly int stencilCompId = Shader.PropertyToID("_StencilComp");
        private static readonly int stencilReadMaskId = Shader.PropertyToID("_StencilReadMask");
        private static readonly int stencilWriteMaskId = Shader.PropertyToID("_StencilWriteMask");
        private static readonly int colorMaskId = Shader.PropertyToID("_ColorMask");

        [SerializeField] private GradientMapPalette palette;
        [SerializeField] private int row;

        private GradientMapPalette subscribedPalette;
        private Material stencilMaterial;
        private Coroutine fade;
        private int fromRow;
        private float blend = 1f;

        public void SetRow(int newRow, float duration = 0f)
        {
            if (fade != null)
            {
                StopCoroutine(fade);
                fade = null;
            }

            if (duration > 0f && newRow != row && isActiveAndEnabled && Application.isPlaying)
            {
                fade = StartCoroutine(FadeAsync(newRow, duration));
            }
            else
            {
                row = newRow;
                fromRow = newRow;
                blend = 1f;
                graphic.SetVerticesDirty();
            }
        }

        private IEnumerator FadeAsync(int newRow, float duration)
        {
            // Fade from the row currently on screen, which may itself be mid-fade
            fromRow = blend < .5f ? fromRow : row;
            row = newRow;
            blend = 0f;

            while (blend < 1f)
            {
                blend = Mathf.Min(1f, blend + Time.unscaledDeltaTime / duration);
                graphic.SetVerticesDirty();
                yield return null;
            }

            fromRow = row;
            fade = null;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || palette == null) return;

            Vector4 ramp = new Vector4(palette.GetRowCoordinate(fromRow), palette.GetRowCoordinate(row), blend, 0f);
            UIVertex vertex = default;

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.uv1 = ramp;
                vh.SetUIVertex(vertex, i);
            }
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!IsActive() || palette == null) return baseMaterial;

            Material material = palette.Material;
            Material previous = stencilMaterial;
            stencilMaterial = null;

            // A mask earlier in the modifier chain swapped in a stencil variant, so rebuild it on top of the palette material.
            // Add before releasing the previous one so an unchanged variant is reused instead of recreated.
            if (baseMaterial != null && baseMaterial.HasProperty(stencilId) && baseMaterial.GetInt(stencilId) != 0)
            {
                stencilMaterial = StencilMaterial.Add(
                    material,
                    baseMaterial.GetInt(stencilId),
                    (StencilOp)baseMaterial.GetInt(stencilOpId),
                    (CompareFunction)baseMaterial.GetInt(stencilCompId),
                    (ColorWriteMask)baseMaterial.GetInt(colorMaskId),
                    baseMaterial.GetInt(stencilReadMaskId),
                    baseMaterial.GetInt(stencilWriteMaskId));
            }

            if (previous != null)
            {
                StencilMaterial.Remove(previous);
            }

            return stencilMaterial != null ? stencilMaterial : material;
        }

        protected override void OnEnable()
        {
            fromRow = row;
            blend = 1f;
            Subscribe();
            EnableCanvasChannel();
            base.OnEnable();
            graphic.SetMaterialDirty();
        }

        protected override void OnDisable()
        {
            fade = null;
            Unsubscribe();

            if (stencilMaterial != null)
            {
                StencilMaterial.Remove(stencilMaterial);
                stencilMaterial = null;
            }

            base.OnDisable();
            graphic.SetMaterialDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            fromRow = row;
            blend = 1f;

            if (isActiveAndEnabled)
            {
                Unsubscribe();
                Subscribe();
                EnableCanvasChannel();
                graphic.SetMaterialDirty();
            }

            base.OnValidate();
        }
#endif

        private void Subscribe()
        {
            if (palette != null)
            {
                palette.Changed += Refresh;
                subscribedPalette = palette;
            }
        }

        private void Unsubscribe()
        {
            if (subscribedPalette != null)
            {
                subscribedPalette.Changed -= Refresh;
                subscribedPalette = null;
            }
        }

        private void Refresh()
        {
            if (this == null) return;

            graphic.SetMaterialDirty();
            graphic.SetVerticesDirty();
        }

        // The ramp coordinates travel in TEXCOORD1, which canvases strip unless asked to keep it
        private void EnableCanvasChannel()
        {
            Canvas canvas = graphic.canvas;

            if (canvas == null) return;

            const AdditionalCanvasShaderChannels channel = AdditionalCanvasShaderChannels.TexCoord1;

            // Only assign when missing so the canvas isn't dirtied on every enable
            if ((canvas.additionalShaderChannels & channel) == 0) canvas.additionalShaderChannels |= channel;
            if ((canvas.rootCanvas.additionalShaderChannels & channel) == 0) canvas.rootCanvas.additionalShaderChannels |= channel;
        }
    }
}
