using UnityEngine.AddressableAssets;

namespace HHG.Common.Runtime
{
    public static class AddressableExtensions
    {
        public static bool IsValid(this AssetReference reference)
        {
            return reference != null && reference.RuntimeKeyIsValid();
        }
    }
}