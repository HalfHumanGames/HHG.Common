using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CreateAssetMenu(fileName = "Script Collection", menuName = "HHG/Template Collections/Script Collection")]
    public class ScriptTemplateCollectionAsset : TemplateCollectionAsset<TextAsset>
    {
        private static Dictionary<string, string> _keywords;
        private static Dictionary<string, string> keywords
        {
            get
            {
                if (_keywords == null)
                {
                    _keywords = new Dictionary<string, string>()
                    {
                        { "game", Application.productName },
                        { "namespace", !string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace) ? EditorSettings.projectGenerationRootNamespace : Application.productName }
                    };
                }

                return _keywords;
            }
        }

        protected override string defaultPath => "Assets/Create/HHG/Scripts";

        protected override void Create(TextAsset template)
        {
            string folder = AssetDatabaseUtil.GetProjectFolderAssetPath();
            string path = $"{folder}/{template.name}.cs";
            string contents = Format(template.text, keywords);
            File.WriteAllText(path, contents);
            AssetDatabase.Refresh();
        }

        private static string Format(string contents, IDictionary<string, string> variables)
        {
            foreach (KeyValuePair<string, string> kvpair in variables)
            {
                contents = Regex.Replace(contents, $"{{{{{kvpair.Key}}}}}", kvpair.Value);
            }

            return contents;
        }
    }
}