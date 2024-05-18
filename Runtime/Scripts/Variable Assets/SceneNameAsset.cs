using UnityEditor;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Scene Name", menuName = "HHG/Assets/Variable/Scene Name")]
    public class SceneNameAsset : VariableAssetBase<string>
    {
#if UNITY_EDITOR
        [Dropdown(typeof(SceneAsset))]
#endif
        [SerializeField] private Object scene;

        private void OnValidate()
        {
            if (scene != null && value != scene.name)
            {
                value = scene.name;
            }
        }
    }
}