using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class JsonUtil
    {
        public static string ToJson(this object value)
        {
            return JsonUtility.ToJson(value);
        }

        public static T FromJson<T>(string json)
        {
            return (T)JsonUtility.FromJson(json, typeof(T));
        }

        public static object FromJson(string json, System.Type type)
        {
            return JsonUtility.FromJson(json, type);
        }

        public static void FromJsonOverwrite(this object obj, string json)
        {
            JsonUtility.FromJsonOverwrite(json, obj);
        }

        public static void FromJsonOverwrite(this object obj, object other)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(other), obj);
        }

        public static byte[] ToJsonBytes(this object value, bool pretty = false)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(value, pretty));
        }

        public static T FromJsonBytes<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes));
        }

        public static object FromJsonBytes(byte[] bytes, System.Type type)
        {
            return JsonUtility.FromJson(Encoding.UTF8.GetString(bytes), type);
        }

        public static void FromJsonBytesOverwrite(this object value, byte[] bytes)
        {
            JsonUtility.FromJsonOverwrite(Encoding.UTF8.GetString(bytes), value);
        }

        public static byte[] ToGZipJsonBytes(this object value)
        {
            return GZipUtil.Compress(Encoding.UTF8.GetBytes(JsonUtility.ToJson(value)));
        }

        public static T FromGZipJsonBytes<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)));
        }

        public static object FromGZipJsonBytes(byte[] bytes, System.Type type)
        {
            return JsonUtility.FromJson(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)), type);
        }

        public static void FromGZipJsonBytesOverwrite(this object value, byte[] bytes)
        {
            JsonUtility.FromJsonOverwrite(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)), value);
        }
    }
}