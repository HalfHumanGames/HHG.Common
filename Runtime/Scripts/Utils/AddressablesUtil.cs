using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace HHG.Common.Runtime
{
    public static class AddressablesUtil
    {
        private static HashSet<object> keys;

        public static bool HasKey(object key)
        {
            if (keys == null)
            {
                Addressables.InitializeAsync().WaitForCompletion(); // Make sure resource locators are loaded
                keys = new HashSet<object>(Addressables.ResourceLocators.SelectMany(locator => locator.Keys));
            }

            return keys.Contains(key);
        }
    }
}