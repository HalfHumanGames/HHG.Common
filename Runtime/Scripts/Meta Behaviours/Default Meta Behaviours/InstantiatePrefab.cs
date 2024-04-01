using UnityEngine;

namespace HHG.Common.Runtime
{
    public class InstantiatePrefab : MetaBehaviour
    {
        [SerializeField] private bool parent;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool autoDestroy;
        [SerializeField] private float destroyDelay;

        public override void Awake()
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