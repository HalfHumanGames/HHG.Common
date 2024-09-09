using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
        }
    }
}