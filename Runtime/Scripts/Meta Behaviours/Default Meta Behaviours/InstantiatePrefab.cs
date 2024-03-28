using UnityEngine;

namespace HHG.Common
{
    public class InstantiatePrefab : MetaBehaviour
    {
        [SerializeField] private bool parent;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool autoDestroy;
        [SerializeField] private float destroyDelay;

        public override void Start()
        {
            GameObject instance = parent ?
                Object.Instantiate(prefab, gameObject.transform) :
                Object.Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);

            if (autoDestroy)
            {
                Object.Destroy(instance, destroyDelay);
            }

            Destroy();
        }
    }
}