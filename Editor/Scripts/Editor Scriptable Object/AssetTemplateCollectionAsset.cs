using System.IO;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CreateAssetMenu(fileName = "Asset Collection", menuName = "HHG/Template Collections/Asset Collection")]
    public class AssetTemplateCollectionAsset : TemplateCollectionAsset<Object>
    {
        protected override string defaultPath => $"Assets/Create/HHG/";

        protected override void Create(Object template)
        {
            string folder = AssetDatabaseUtil.GetProjectFolderAbsolutePath();
            string sourcePath = AssetDatabaseUtil.GetAbsolutePath(template);
            string extension = Path.GetExtension(sourcePath);
            string destinationPath = $"{folder}/{template.name}{extension}";
            
            File.Copy(sourcePath, destinationPath, overwrite: true);
            AssetDatabase.Refresh();
        }
    }
}