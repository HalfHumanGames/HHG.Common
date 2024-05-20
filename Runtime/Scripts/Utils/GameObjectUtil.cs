using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class GameObjectUtil
    {
        public static GameObject FindOrCreate(string path)
        {
            GameObject current = null;
            foreach (string part in path.Split('/', System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (current == null)
                {
                    current = GameObject.Find(part) ?? new GameObject(part);
                }
                else
                {
                    Transform child = current.transform.Find(part);
                    current = child ? child.gameObject : new GameObject(part) { transform = { parent = current.transform } };
                }
            }
            return current;
        }
    }
}