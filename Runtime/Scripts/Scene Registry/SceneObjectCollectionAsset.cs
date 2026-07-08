using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Scene Object Collection", menuName = "HHG/Assets/Scene Object Collection")]
    public class SceneObjectCollectionAsset : ScriptableObject, IEnumerable<SceneObjectReference>
    {
        public IReadOnlyList<SceneObjectReference> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<SceneObjectReference>(items);
                    if (inherit) _items.AddRange(inheritFrom.Items);
                }
                return _items;
            }
        }

        [SerializeField] private string registryName;
        [SerializeField] private bool inherit;
        [SerializeField, ShowIf(nameof(inherit), true)] private SceneObjectCollectionAsset inheritFrom;

        [EditorButton(nameof(AddReference), "+")]
        [SerializeField, SceneObjectRegistry] private List<SceneObjectReference> items = new List<SceneObjectReference>();

        [System.NonSerialized] private List<SceneObjectReference> _items;

        public IEnumerator<SceneObjectReference> GetEnumerator()
        {
            // Use property to get merged items list for collections
            // that also inherit items from other base collections
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        private void AddReference()
        {
            items.Add(default);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
