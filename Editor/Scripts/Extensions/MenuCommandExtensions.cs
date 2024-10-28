using UnityEditor;

namespace HHG.Common.Editor
{
    public static class MenuCommandExtensions
    {
        public static bool IsContextActiveObject(this MenuCommand menuCommand)
        {
            return menuCommand.context == Selection.activeObject;
        }
    }
}