using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Scene Object Collection", menuName = "HHG/Assets/Scene Object Collection")]
    public class SceneObjectCollectionAsset : ScriptableObject
    {
        public IReadOnlyList<SceneObjectReference> References => references;

        [SerializeField] private string registryName;

        [EditorButton(nameof(AddReference), "+")]
        [SerializeField, SceneObjectRegistry] private List<SceneObjectReference> references = new List<SceneObjectReference>();

#if UNITY_EDITOR
        private void AddReference()
        {
            references.Add(default);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
