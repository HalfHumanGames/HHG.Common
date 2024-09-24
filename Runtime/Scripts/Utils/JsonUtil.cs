using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class JsonUtil
    {
        public static byte[] ToJson<T>(this T value, bool pretty = false)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(value, pretty));
        }

        public static T FromJson<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes));
        }

        public static byte[] ToGZipJson<T>(this T value)
        {
            return GZipUtil.Compress(Encoding.UTF8.GetBytes(JsonUtility.ToJson(value)));
        }

        public static T FromGZipJson<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(GZipUtil.Decompress(bytes)));
        }
    }
}