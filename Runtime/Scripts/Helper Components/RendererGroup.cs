using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RendererGroup : MonoBehaviour
    {
        public void Enable()
        {
            Enable(true);
        }

        public void Disable()
        {
            Enable(false);
        }

        public void Enable(bool enable)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = enable;
            }
        }
    } 
}
