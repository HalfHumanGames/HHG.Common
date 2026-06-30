using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SceneObjectRegistryAttribute : PropertyAttribute
    {
        public string RegistryName { get; }

        public SceneObjectRegistryAttribute(string registryName)
        {
            RegistryName = registryName;
        }

        public SceneObjectRegistryAttribute() { }
    }
}
