using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
	public class PlayerPrefsIO : IIO
	{
		public void Clear(string fileName)
		{
			try
			{
				PlayerPrefs.DeleteKey(fileName);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public bool Exists(string fileName)
		{
			try
			{
				return PlayerPrefs.HasKey(fileName);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return false;
			}
		}

		public byte[] ReadAllBytes(string fileName)
		{
			try
			{
				string base64Tex = PlayerPrefs.GetString(fileName, null);
				return Convert.FromBase64String(base64Tex);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return new byte[0];
			}
		}

		public void WriteAllBytes(string fileName, byte[] bytes)
		{
			try
			{
				string base64Tex = Convert.ToBase64String(bytes);
				PlayerPrefs.SetString(fileName, base64Tex);
				PlayerPrefs.Save();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void OnBeforeClose() { /* Do nothing */ }
		public void OnClose() { /* Do nothing */ }
	}

}