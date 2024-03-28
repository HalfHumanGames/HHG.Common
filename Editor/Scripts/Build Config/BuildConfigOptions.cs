using System;
using UnityEditor;

namespace HHG.Common.Editor
{
    [Serializable]
    public struct BuildConfigOptions
    {
        public bool Enabled;
        public BuildTarget Target;
        public BuildOptions Options;
        public ScriptingImplementation Backend;
    }
}