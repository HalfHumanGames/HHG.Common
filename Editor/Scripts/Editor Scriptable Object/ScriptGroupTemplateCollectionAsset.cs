using HHG.Common.Runtime;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CreateAssetMenu(fileName = "Script Group Collection", menuName = "HHG/Template Collections/Script Group Collection")]
    public class ScriptGroupTemplateCollectionAsset : TemplateCollectionAsset<TextListAsset>
    {
        protected override string defaultPath => "Assets/Create/HHG/Scripts";

        protected override void Create(TextListAsset template)
        {
            foreach (TextAsset text in template.List)
            {
                ScriptTemplateCollectionAsset.Create<ScriptTemplateCollectionAsset>(text);
            }
        }
    }
}