using UnityEngine;

namespace Dungeonspire
{
    public class UniqueId : MonoBehaviour
    {
        public string Id => id;

        [EditorButton(nameof(NewGuild))]
        [SerializeField, Disable] private string id = System.Guid.NewGuid().ToString();

        public void NewGuild()
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
