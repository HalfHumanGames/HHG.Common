namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class GZipJsonSerializer : ISerializer
    {
        public byte[] Serialize(object obj)
        {
            return JsonUtil.ToGZipJsonBytes(obj);
        }

        public object Deserialize(byte[] bytes, System.Type type)
        {
            return JsonUtil.FromGZipJsonBytes(bytes, type);
        }

        public void DeserializeOverwrite(byte[] bytes, object obj)
        {
            JsonUtil.FromGZipJsonBytesOverwrite(obj, bytes);
        }
    }
}