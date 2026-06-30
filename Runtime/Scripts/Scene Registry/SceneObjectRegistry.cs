using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common.Runtime
{
    public class SceneObjectRegistry : MonoBehaviour
    {
        [EditorButton(nameof(AddChild), "+")]
        [SerializeField] private string childName = "New Scene Object";

#if UNITY_EDITOR
        private void AddChild()
        {
            GameObject child = new GameObject(childName);
            child.transform.SetParent(transform);
            child.AddComponent<SceneObjectId>();
            Undo.RegisterCreatedObjectUndo(child, "Add Scene Object");
        }
#endif
    }
}
