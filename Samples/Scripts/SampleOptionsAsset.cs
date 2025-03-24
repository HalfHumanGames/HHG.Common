using HHG.Common.Runtime;
using UnityEngine;

namespace HHG.Common.Sample
{
    [CreateAssetMenu(fileName = "Sample Options", menuName = "HHG/Sample/Sample Options")]
    public class SampleOptionsAsset : OptionsAssetBase
    {
        public const string General = nameof(General);
        public const string Graphics = nameof(Graphics);
        public const string Audio = nameof(Audio);

        public int ResolutionWidth => resolutionWidth;
        public int ResolutionHeight => resolutionHeight;

        #region Option Properties

        [OptionsCategory(General)]
        [OptionsToggle] public bool ScreenShake { get => Get(ref screenShake); set => Set(ref screenShake, value); }
        [OptionsToggle] public bool Vibration { get => Get(ref vibration); set => Set(ref vibration, value); }

        [OptionsCategory(Graphics)]
        [OptionsToggle] public bool Blood { get => Get(ref blood); set => Set(ref blood, value); }
        [OptionsToggle] public bool Vignette { get => Get(ref vignette); set => Set(ref vignette, value); }
        [OptionsToggle] public bool Fullscreen { get => Get(ref fullscreen); set => Set(ref fullscreen, value); }
        [OptionsDropdown(nameof(vSyncCountOptions))] public int VSyncCount { get => Get(ref vSyncCount); set => Set(ref vSyncCount, value); }
        [OptionsDropdown(nameof(resolutionOptions))] public int Resolution { get => Get(ref resolution); set => Set(ref resolution, value); }

        [OptionsCategory(Audio)]
        [OptionsSlider] public float MusicVolume { get => Get(ref musicVolume); set => Set(ref musicVolume, value); }
        [OptionsSlider] public float SoundEffectsVolume { get => Get(ref soundEffectsVolume); set => Set(ref soundEffectsVolume, value); }
        [OptionsSlider] public float UIVolume { get => Get(ref uiVolume); set => Set(ref uiVolume, value); }

        #endregion

        #region Option Fields

        [Header(General)]
        [SerializeField] private bool screenShake;
        [SerializeField] private bool vibration;

        [Header(Graphics)]
        [SerializeField] private bool blood;
        [SerializeField] private bool vignette;
        [SerializeField] private bool fullscreen;
        [SerializeField] private int vSyncCount;
        [SerializeField] private int resolution;
        [SerializeField] private int resolutionWidth;
        [SerializeField] private int resolutionHeight;

        [Header(Audio)]
        [SerializeField] private float musicVolume;
        [SerializeField] private float soundEffectsVolume;
        [SerializeField] private float uiVolume;

        #endregion

        private string[] vSyncCountOptions => new string[]
        {
            "Don't Sync",
            "Every V Blank",
            "Every Second V Blank"
        };

        private string[] resolutionOptions => ResolutionUtil.ResolutionsFormatted;

        protected override void OnUpdated()
        {
            ResolutionUtil.SetResolutionIndex(resolution, out resolutionWidth, out resolutionHeight);
        }
    }
}