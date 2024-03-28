using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public static class Locator
    {
        private static Dictionary<SubjectId, object> dict = new Dictionary<SubjectId, object>();

        public static void Register(object obj)
        {
            Register(null, obj);
        }

        public static void Register(object id, object obj)
        {
            SubjectId subjectId = new SubjectId(obj.GetType(), id);
            dict[subjectId] = obj;
        }

        public static void Unregister(object obj)
        {
            Unregister(null, obj);
        }

        public static void Unregister(object id, object obj)
        {
            SubjectId subjectId = new SubjectId(obj.GetType(), id);
            dict.Remove(subjectId);
        }

        public static T Get<T>(object id = null)
        {
            SubjectId subjectId = new SubjectId(typeof(T), id);
            if (dict.ContainsKey(subjectId))
            {
                return (T)dict[subjectId];
            }
            return default;
        }
    }
}