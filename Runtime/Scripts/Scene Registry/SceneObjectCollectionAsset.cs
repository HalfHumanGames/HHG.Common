using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Scene Object Collection", menuName = "HHG/Assets/Scene Object Collection")]
    public class SceneObjectCollectionAsset : ScriptableObject, IEnumerable<SceneObjectReference>
    {
        public IReadOnlyList<SceneObjectReference> Items => items;

        [SerializeField] private string registryName;

        [EditorButton(nameof(AddReference), "+")]
        [SerializeField, SceneObjectRegistry] private List<SceneObjectReference> items = new List<SceneObjectReference>();

#if UNITY_EDITOR
        private void AddReference()
        {
            items.Add(default);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public IEnumerator<SceneObjectReference> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
#endif
    }
}
