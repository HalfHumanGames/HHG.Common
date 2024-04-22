using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class SortChildrenTool : UnityEditor.Editor
    {
        [MenuItem("GameObject/Tools/Sort Children", false, 0)]
        public static void SortChildren()
        {
            Transform[] children = Selection.activeGameObject.transform.Cast<Transform>().ToArray();
            children = children.OrderBy(x => x.name).ToArray();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }

        [MenuItem("GameObject/Tools/Sort Children", true, 0)]
        public static bool CanSortChildren()
        {
            return Selection.activeGameObject != null;
        }
    }

}