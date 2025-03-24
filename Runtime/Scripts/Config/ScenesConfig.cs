using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ScenesConfig
    {
        public SerializedScene NewGameScene => newGameScene;
        public SerializedScene MidGameScene => midGameScene;
        public SerializedScene HubGameScene => hubGameScene;

        [SerializeField] private SerializedScene newGameScene;
        [SerializeField] private SerializedScene midGameScene;
        [SerializeField] private SerializedScene hubGameScene;

        public SerializedScene GetScene(SaveFileScene scene)
        {
            return scene switch
            {
                SaveFileScene.New => newGameScene,
                SaveFileScene.Mid => midGameScene,
                SaveFileScene.Hub => hubGameScene,

                _ => throw new System.ArgumentException($"Scene unhandled: {scene}", nameof(scene))
            };
        }
    }
}