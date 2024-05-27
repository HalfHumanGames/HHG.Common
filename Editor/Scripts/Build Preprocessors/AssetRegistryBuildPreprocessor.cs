using HHG.Common.Runtime;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HHG.Common.Editor
{
    public class AssetRegistryBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            AssetRegistry.EditorLoad();
        }
    }
}