using UnityEngine;

namespace HHG.Common.Runtime
{
    public class GuidAttribute : PropertyAttribute
    {
        public string Path => path;

        private string path;

        public GuidAttribute(string path = null)
        {
            this.path = path;
        }
    }
}