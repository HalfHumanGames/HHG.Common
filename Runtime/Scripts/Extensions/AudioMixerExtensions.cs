using UnityEngine;
using UnityEngine.Audio;

namespace HHG.Common.Runtime
{
    public static class AudioMixerExtensions
    {
        public static void SetVolumeNormalized(this AudioMixer mixer, string group, float normalized)
        {
            normalized = Mathf.Clamp(normalized, .0001f, 1f);
            float db = Mathf.Log10(normalized) * 20f;
            mixer.SetFloat(group, db);
        }
    }
}