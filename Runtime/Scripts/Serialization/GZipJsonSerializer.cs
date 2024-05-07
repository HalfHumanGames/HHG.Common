namespace HHG.Common.Runtime
{
    public class GZipJsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T value)
        {
            return JsonUtil.ToGZipJson(value);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonUtil.FromGZipJson<T>(bytes);
        }
    }
}