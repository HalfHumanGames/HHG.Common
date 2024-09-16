using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            transform.forward = transform.position - new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
    }
}