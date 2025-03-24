using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class SaveFileAssetBase : ScriptableSavable
    {
        public string DisplayName { get => Get(ref displayName); set => Set(ref displayName, value); }
        public int DifficultyIndex { get => Get(ref difficultyIndex); set => Set(ref difficultyIndex, value); }
        public SaveFileScene SaveFileScene { get => Get(ref saveFileScene); set => Set(ref saveFileScene, value); }

        public virtual string LastSavedFormatted => $"{LastSaved:g}";
        public virtual string ProgressInfo => string.Empty;

        [SerializeField] private string displayName;
        [SerializeField] private int difficultyIndex;
        [SerializeField] private SaveFileScene saveFileScene;
    }
}