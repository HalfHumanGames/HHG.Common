using System;
using System.IO;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FileIO : IIO
	{
		private string root;
		private string getRoot
		{
			get
			{
				if (root == null)
				{
					root = Application.persistentDataPath;
				}
				return root;
			}
		}

		public FileIO()
		{

		}

		public FileIO(string root)
		{
			this.root = root;
		}

		public void Clear(string fileName)
		{
			try
			{
				DirectoryInfo directory = new DirectoryInfo(getRoot);

				foreach (FileInfo file in directory.EnumerateFiles(fileName))
				{
					file.Delete();
				}
			}
			catch (Exception e)
			{
                Debug.LogException(e);
			}
		}

		public bool Exists(string fileName)
		{
			try
			{
				return File.Exists(GetFilePath(fileName));
			}
			catch (Exception e)
			{
                Debug.LogException(e);
				return false;
			}
		}

		public byte[] ReadAllBytes(string fileName)
		{
			try
			{
				return File.ReadAllBytes(GetFilePath(fileName));
			}
			catch (Exception e)
			{
                Debug.LogException(e);
				return new byte[0];
			}
		}

		public void WriteAllBytes(string fileName, byte[] bytes)
		{
			try
			{
				File.WriteAllBytes(GetFilePath(fileName), bytes);
			}
			catch (Exception e)
			{
                Debug.LogException(e);
			}
		}

		public string GetFilePath(string fileName)
		{
			return string.IsNullOrEmpty(getRoot) ? fileName : $"{getRoot}/{fileName}";
		}

		public void OnBeforeClose() { /* Do nothing */ }
		public void OnClose() { /* Do nothing */ }

	}

}