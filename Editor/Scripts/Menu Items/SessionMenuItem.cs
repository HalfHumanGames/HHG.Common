using HHG.Common.Runtime;
using UnityEditor;

namespace HHG.Common.Editor
{
    public static class SessionMenuItem
    {
        [MenuItem("Half Human Games/Session")]
        private static void Open()
        {
            Selection.activeObject = AssetDatabaseUtil.FindAsset<Session>();
        }
    }
}