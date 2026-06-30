using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SceneObjectId : MonoBehaviour
    {
        public string Id => id;

        [EditorButton(nameof(Regenerate))]
        [SerializeField, Disable] private string id = System.Guid.NewGuid().ToString();

        private void Awake()
        {
            Locator.Register(id, this);
            Locator.Register(name, this);
        }

        private void OnDestroy()
        {
            Locator.Unregister(id);
            Locator.Unregister(name);
        }

        public void Regenerate()
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
