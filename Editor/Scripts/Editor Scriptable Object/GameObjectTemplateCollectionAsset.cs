#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CreateAssetMenu(fileName = "Game Object Collection", menuName = "HHG/Template Collections/Game Object Collection")]
    public class GameObjectTemplateCollectionAsset : TemplateCollectionAsset<GameObject>
    {
        public override string defaultPath => "GameObject/HHG";

        protected override void Create(GameObject template)
        {
            PrefabUtility.InstantiatePrefab(template, Selection.activeTransform);
        }
    }
}
#endif