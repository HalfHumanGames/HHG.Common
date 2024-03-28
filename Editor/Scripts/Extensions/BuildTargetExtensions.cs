using System;
using UnityEditor;

namespace HHG.Common.Editor
{
    public static class BuildTargetExtensions
    {
        public static string GetExtension(this BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android: return "aab";
                case BuildTarget.iOS: return "ipa";
                case BuildTarget.StandaloneLinux64: return "x86_64";
                case BuildTarget.StandaloneOSX: return "app";
                case BuildTarget.StandaloneWindows: return "exe";
                case BuildTarget.StandaloneWindows64: return "exe";
            }

            throw new NotImplementedException($"No extensions defined for target: {target}");
        }

        public static string GetDisplayName(this BuildTarget target)
        {
            return target.ToString().
                Replace("OSX", "Mac").
                Replace("Windows", "Win").
                Replace("Standalone", string.Empty);
        }
    }
}
