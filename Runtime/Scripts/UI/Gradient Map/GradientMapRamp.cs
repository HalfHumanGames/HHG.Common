using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class GradientMapRamp
    {
        public Gradient Gradient => gradient;

        [SerializeField] private string name;
        [SerializeField] private Gradient gradient = new Gradient();
    }
}
