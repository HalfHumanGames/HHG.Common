using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MetaDataExtensions
    {
        public static void SetMetaData(this GameObject gameObject, object key, object value)
        {
            gameObject.GetOrAddComponent<MetaData>().Set(key, value);
        }

        public static TValue GetMetaData<TValue>(this GameObject gameObject, object key)
        {
            return gameObject.GetOrAddComponent<MetaData>(add: false).Get<TValue>(key);
        }

        public static bool TryGetMetaData<TValue>(this GameObject gameObject, object key, out TValue value)
        {
            MetaData metaData = gameObject.GetOrAddComponent<MetaData>(add: false);

            if (metaData != null)
            {
                return metaData.TryGet(key, out value);
            }

            value = default;
            return false;
        }

        public static bool HasMetaData(this GameObject gameObject, object key)
        {
            MetaData metaData = gameObject.GetOrAddComponent<MetaData>(add: false);
            return metaData != null && metaData.Has(key);
        }

        public static bool RemoveMetaData(this GameObject gameObject, object key)
        {
            MetaData metaData = gameObject.GetOrAddComponent<MetaData>(add: false);
            return metaData != null && metaData.Remove(key);
        }

        public static void SetMetaData(this Component component, object key, object value)
        {
            component.gameObject.SetMetaData(key, value);
        }

        public static TValue GetMetaData<TValue>(this Component component, object key)
        {
            return component.gameObject.GetMetaData<TValue>(key);
        }

        public static bool TryGetMetaData<TValue>(this Component component, object key, out TValue value)
        {
            return component.gameObject.TryGetMetaData(key, out value);
        }

        public static bool HasMetaData(this Component component, object key)
        {
            return component.gameObject.HasMetaData(key);
        }

        public static bool RemoveMetaData(this Component component, object key)
        {
            return component.gameObject.RemoveMetaData(key);
        }
    }
}
