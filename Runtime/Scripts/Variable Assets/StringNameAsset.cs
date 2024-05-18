using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "String", menuName = "HHG/Assets/Variable/String Name")]
    public class StringNameAsset : VariableAssetBase<string>
    {
        private void OnValidate()
        {
            if (value != name)
            {
                value = name;
            }
        }
    }
}