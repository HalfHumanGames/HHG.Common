namespace HHG.Common.Runtime
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);
        object Deserialize(byte[] bytes, System.Type type);
        void DeserializeOverwrite(byte[] bytes, object obj);
    }

    public static class ISerializerExtensions
    {
        public static T Deserialize<T>(this ISerializer serializer, byte[] bytes)
        {
            return (T)serializer.Deserialize(bytes, typeof(T));
        }
    }
}