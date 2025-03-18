using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class SerializedAsset<T> where T : Object
    {
        public bool HasAsset => Asset != null;

        public T Asset
        {
            get
            {
                if (asset == null && !string.IsNullOrEmpty(guid))
                {
                    asset = AssetRegistry.GetAsset<T>(guid);
                }

                return asset;
            }
            set
            {
                asset = value;
                guid = AssetRegistry.GetGuid(asset);
            }
        }

        [SerializeField, Guid(nameof(asset)), HideInInspector] protected string guid;
        [SerializeField] protected T asset;

        public SerializedAsset(T asset)
        {
            Asset = asset;
        }

        public static implicit operator SerializedAsset<T>(T asset) => new SerializedAsset<T>(asset);
        public static implicit operator T(SerializedAsset<T> serializedAsset) => serializedAsset != null ? serializedAsset.Asset : null;
    }
}