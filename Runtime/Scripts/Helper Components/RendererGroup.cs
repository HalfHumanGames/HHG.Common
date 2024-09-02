using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RendererGroup : MonoBehaviour
    {
        private List<Renderer> renderers = new List<Renderer>();

        private void Awake()
        {
            GetComponentsInChildren(true, renderers);
        }

        public void OnEnable()
        {
            SetRenderersEnabled(true);
        }

        public void OnDisable()
        {
            SetRenderersEnabled(false);
        }

        private void SetRenderersEnabled(bool enable)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = enable;
            }
        }

        private void OnTransformChildrenChanged()
        {
            SetRenderersEnabled(enabled);
        }
    } 
}
