namespace HHG.Common.Runtime
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
    }
}