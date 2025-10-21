using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerAspectAdjuster : MonoBehaviour
    {
        private CanvasScaler canvasScaler;

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            AdjustAspect();
        }

        private void Update()
        {
            AdjustAspect();
        }

        private void AdjustAspect()
        {
            float aspect = (float)Screen.width / Screen.height;
            float targetAspect = 16f / 9f;
            canvasScaler.matchWidthOrHeight = aspect >= targetAspect ? 1f : 0f;
        }
    }
}