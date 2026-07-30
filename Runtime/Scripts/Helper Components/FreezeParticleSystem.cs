using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FreezeParticleSystem : MonoBehaviour
    {
        [SerializeField] private bool changeSortingLayer;
        [SerializeField, ShowIf(nameof(changeSortingLayer), true)] private string sortingLayerName;
        [SerializeField, ShowIf(nameof(changeSortingLayer), true)] private int sortingOrder;

        private ParticleSystem particles;

        private void Awake()
        {
            particles = GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            float totalDuration = particles.main.duration;
            if (particles.time >= totalDuration)
            {
                if (changeSortingLayer)
                {
                    ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
                    renderer.sortingLayerName = sortingLayerName;
                    renderer.sortingOrder = sortingOrder;
                }

                particles.time = totalDuration - .01f;
                particles.Pause();
            }
        }
    }

}