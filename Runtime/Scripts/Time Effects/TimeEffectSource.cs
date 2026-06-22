using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class TimeEffectSource : MonoBehaviour
    {
        public bool IsPlaying => TimeEffectManager.HasInstance && TimeEffectManager.Instance.CurrentSource == this;

        [SerializeField] private ConflictMode conflictMode;
        [SerializeReference, ReorderableList, SubclassSelector] private List<TimeEffect> effects = new List<TimeEffect>();

        public void Play()
        {
            TimeEffectManager.Instance.Play(this, effects, conflictMode);
        }

        public void Stop()
        {
            if (TimeEffectManager.HasInstance && TimeEffectManager.Instance.CurrentSource == this)
            {
                TimeEffectManager.Instance.Stop();
            }
        }
    }
}
