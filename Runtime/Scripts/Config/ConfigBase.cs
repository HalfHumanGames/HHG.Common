using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class ConfigBase<T> : ConfigBase where T : class
    {
        public static new T Instance => ConfigBase.Instance as T;
    }

    public abstract class ConfigBase : ScriptableSingleton<ConfigBase>
    {
        public static DifficultyConfig Difficulty => Instance.difficulty;
        public static ScenesConfig Scenes => Instance.scenes;

        [SerializeField] private DifficultyConfig difficulty;
        [SerializeField] private ScenesConfig scenes;
    }
}