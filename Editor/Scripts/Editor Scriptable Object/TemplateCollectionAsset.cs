#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CreateAssetMenu(fileName = "Template Collection", menuName = "HHG/Editor Assets/Template Collection")]
    public class TemplateCollectionAsset : ScriptableObject
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private string path = "GameObject/HHG/";
        [SerializeField] private List<GameObject> templates = new List<GameObject>();

        private void Awake()
        {
            EditorApplication.delayCall += VerifyTemplates;
        }

        public void VerifyTemplates()
        {
            foreach (GameObject prefab in templates)
            {
                VerifyTemplate(prefab);
            }
        }

        private void VerifyTemplate(GameObject template)
        {
            string fullPath = $"{path}/{template.name}";
            var exists = MenuTool.MenuItemExists(fullPath);

            if (!exists && enabled)
            {
                MenuTool.AddMenuItem(fullPath, string.Empty, false, 0, () => InstantiateTemplate(template), () => true);
            }
            else if (exists && !enabled)
            {
                MenuTool.RemoveMenuItem(fullPath);
            }
        }

        private static void InstantiateTemplate(GameObject template)
        {
            PrefabUtility.InstantiatePrefab(template, Selection.activeTransform);
        }

        private void OnValidate()
        {
            EditorApplication.delayCall += VerifyTemplates;
        }
    }
}
#endif