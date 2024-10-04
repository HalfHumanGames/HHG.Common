namespace HHG.Common.Runtime
{
    public class GZipJsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T value)
        {
            return JsonUtil.ToGZipJsonBytes(value);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonUtil.FromGZipJsonBytes<T>(bytes);
        }
    }
}