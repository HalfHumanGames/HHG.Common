using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.Audio;

namespace HHG.Common.Sample
{
    public class SampleOptionsManager : OptionsManagerBase<SampleOptionsAsset>
    {
        [SerializeField] protected AudioMixer mixer;

        protected override void OnOptionsUpdated(SampleOptionsAsset optionsAsset)
        {
            // General
            if (Camera.main)
            {
                Camera.main.SetLayerCulling("Blood", optionsAsset.Blood);
                Camera.main.SetLayerCulling("Vignette", optionsAsset.Vignette);
            }

            // Graphics
            QualitySettings.vSyncCount = optionsAsset.VSyncCount;
            Screen.SetResolution(optionsAsset.ResolutionWidth, optionsAsset.ResolutionHeight, optionsAsset.Fullscreen);

            // Audio
            mixer.SetVolumeNormalized("Music Volume", optionsAsset.MusicVolume);
            mixer.SetVolumeNormalized("Sound Effects Volume", optionsAsset.SoundEffectsVolume);
            mixer.SetVolumeNormalized("Sound Effects Priority Volume", optionsAsset.SoundEffectsVolume);
            mixer.SetVolumeNormalized("UI Volume", optionsAsset.UIVolume);
        }
    }
}