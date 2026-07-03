using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SceneObjectId : MonoBehaviour
    {
        public string Id => id;

        [EditorButton(nameof(Regenerate))]
        [SerializeField, Disable] private string id = System.Guid.NewGuid().ToString();

        [System.NonSerialized] private bool isOriginal;

        private void Awake()
        {
            if (Locator.Contains(id, this))
            {
                Regenerate();
                Locator.Register(id, this);
            }
            else
            {
                isOriginal = true;
                Locator.Register(id, this);
                Locator.Register(name, this);
            }
        }

        private void OnDestroy()
        {
            if (isOriginal)
            {
                Locator.Unregister(id, this);
                Locator.Unregister(name, this);
            }
            else
            {
                Locator.Unregister(id, this);
            }
        }

        public void Regenerate()
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
