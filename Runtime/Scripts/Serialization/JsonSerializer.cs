namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize(object value)
        {
            return JsonUtil.ToJsonBytes(value, true);
        }

        public object Deserialize(byte[] bytes, System.Type type)
        {
            return JsonUtil.FromJsonBytes(bytes, type);
        }

        public void DeserializeOverwrite(byte[] bytes, object obj)
        {
            JsonUtil.FromJsonBytesOverwrite(obj, bytes);
        }
    }
}