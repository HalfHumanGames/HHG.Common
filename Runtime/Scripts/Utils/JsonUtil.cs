using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class JsonUtil
    {
        public static string ToJson<T>(T value)
        {
            return JsonUtility.ToJson(value);
        }

        public static T FromJson<T>(string json)
        {
            return (T)JsonUtility.FromJson(json, typeof(T));
        }

        public static void FromJsonOverwrite<T>(this T obj, string json)
        {
            JsonUtility.FromJsonOverwrite(json, obj);
        }

        public static byte[] ToJsonBytes<T>(this T value, bool pretty = false)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(value, pretty));
        }

        public static T FromJsonBytes<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes));
        }

        public static void FromJsonBytesOverwrite<T>(this T value, byte[] bytes)
        {
            JsonUtility.FromJsonOverwrite(Encoding.UTF8.GetString(bytes), value);
        }

        public static byte[] ToGZipJsonBytes<T>(this T value)
        {
            return GZipUtil.Compress(Encoding.UTF8.GetBytes(JsonUtility.ToJson(value)));
        }

        public static T FromGZipJsonBytes<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)));
        }

        public static void FromGZipJsonBytesOverwrite<T>(this T value, byte[] bytes)
        {
            JsonUtility.FromJsonOverwrite(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)), value);
        }
    }
}