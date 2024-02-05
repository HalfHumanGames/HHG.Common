#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HHG.Common
{
	public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
	{
		private static string resourcePath => $"{typeof(T)}";
		private static string assetPath => $"Assets/Resources/{resourcePath}.asset";

		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null || instance.Equals(null))
				{
					instance = Resources.Load<T>(resourcePath);
					if (instance == null)
					{
						Debug.LogWarning($"{typeof(T)} asset not found at '{assetPath}' so a new one has been created.");
						instance = CreateInstance<T>();
#if UNITY_EDITOR
						AssetDatabase.CreateAsset(instance, assetPath);
#endif
					}
				}
				return instance;
			}
		}
	}

}
