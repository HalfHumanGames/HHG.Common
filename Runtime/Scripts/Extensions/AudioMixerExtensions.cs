using UnityEngine;
using UnityEngine.Audio;

namespace HHG.Common.Runtime
{
    public static class AudioMixerExtensions
    {
        private const float maxDb = 20f;
        private const float minDb = -80f;

        public static void SetVolumeNormalized(this AudioMixer mixer, string group, float normalized)
        {
            float db = Mathf.Clamp(minDb + (normalized * (maxDb - minDb)), minDb, maxDb);
            mixer.SetFloat(group, db);
        }
    }
}