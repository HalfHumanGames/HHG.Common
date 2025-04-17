using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class SaveFileAssetBase : ScriptableSavable
    {
        public static string[] SaveFileNames => saveFileNames ??= Enumerable.Range(0, 10).Select(i => $"Save Slot {i}").ToArray();
        private static string[] saveFileNames;

        public string DisplayName { get => Get(ref displayName); set => Set(ref displayName, value); }
        public int DifficultyIndex { get => Get(ref difficultyIndex); set => Set(ref difficultyIndex, value); }
        public SaveFileScene SaveFileScene { get => Get(ref saveFileScene); set => Set(ref saveFileScene, value); }

        public virtual string LastSavedFormatted => $"{LastSaved:g}";
        public virtual string ProgressInfo => string.Empty;

        public override string DefaultFileName => "Save Slot 0";
        
        [SerializeField] private string displayName;
        [SerializeField] private int difficultyIndex;
        [SerializeField] private SaveFileScene saveFileScene;

        public bool AnyExists()
        {
            return AnyExists(SaveFileNames);
        }

        public bool LoadLastSaved()
        {
            return LoadLastSaved(SaveFileNames);
        }
    }
}