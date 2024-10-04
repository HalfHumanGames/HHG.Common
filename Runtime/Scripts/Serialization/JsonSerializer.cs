namespace HHG.Common.Runtime
{
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T value)
        {
            return JsonUtil.ToJsonBytes(value, true);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonUtil.FromJsonBytes<T>(bytes);
        }
    }
}