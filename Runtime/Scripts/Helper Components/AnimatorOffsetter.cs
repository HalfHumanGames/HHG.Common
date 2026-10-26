using UnityEngine;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorOffsetter : MonoBehaviour
    {
        [SerializeField] private int layer;

        private void Start()
        {
            GetComponent<Animator>().Offset(layer);
        }
    }
}
