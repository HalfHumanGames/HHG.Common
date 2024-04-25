using UnityEditor;

namespace HHG.Common.Editor
{
    public static class ReserializeAssetsTool
    {
        [MenuItem("Half Human Games/Tools/Reserialize Assets")]
        public static void ReserializeAssets()
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}