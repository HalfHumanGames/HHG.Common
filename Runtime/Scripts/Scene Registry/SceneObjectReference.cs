using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class SceneObjectReference
    {
        public GameObject GameObject
        {
            get
            {
                if (gameObject == null && Locator.TryGet(id, out SceneObjectId sceneObjectId))
                {
                    gameObject = sceneObjectId.gameObject;
                }

                return gameObject;
            }
        }

        [SerializeField] private string id;

        private GameObject gameObject;

        public static implicit operator string(SceneObjectReference reference) => reference.id;
        public static implicit operator SceneObjectReference(string id) => new SceneObjectReference { id = id };
    }
}
