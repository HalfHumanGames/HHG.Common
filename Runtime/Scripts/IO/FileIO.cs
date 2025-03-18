using System.IO;
using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
	[System.Serializable]
    public class FileIO : IIO
	{
		private string _root;
		private string root
		{
			get
			{
				if (string.IsNullOrEmpty(_root))
				{
					_root = Application.persistentDataPath;
				}
				return _root;
			}
		}

		public FileIO()
		{

		}

		public FileIO(string root)
		{
			_root = root;
		}

		public void Delete(string fileName)
		{
			try
			{
				DirectoryInfo directory = new DirectoryInfo(root);

				foreach (FileInfo file in directory.EnumerateFiles(fileName))
				{
					file.Delete();
				}
			}
			catch (System.Exception e)
			{
                LogExceptionWithDriveInfo(e);
			}
		}

		public bool Exists(string fileName)
		{
			try
			{
				return File.Exists(GetFilePath(fileName));
			}
			catch (System.Exception e)
			{
                LogExceptionWithDriveInfo(e);
				return false;
			}
		}

		public byte[] ReadAllBytes(string fileName)
		{
			try
			{
				return File.ReadAllBytes(GetFilePath(fileName));
			}
			catch (System.Exception e)
			{
                LogExceptionWithDriveInfo(e);
				return new byte[0];
			}
		}

		public void WriteAllBytes(string fileName, byte[] bytes)
		{
			try
			{
				File.WriteAllBytes(GetFilePath(fileName), bytes);
			}
			catch (System.Exception e)
			{
                LogExceptionWithDriveInfo(e);
			}
		}

		public string GetFilePath(string fileName)
		{
			return string.IsNullOrEmpty(root) ? fileName : $"{root}/{fileName}";
		}

		public void OnBeforeClose() { /* Do nothing */ }
		public void OnClose() { /* Do nothing */ }

		private void LogExceptionWithDriveInfo(System.Exception e)
		{
			string driveLetter = Path.GetPathRoot(Application.persistentDataPath);
            
			if (!string.IsNullOrEmpty(driveLetter))
			{
                DriveInfo driveInfo = new DriveInfo(driveLetter);

				if (driveInfo != null)
				{
                    long freeSpaceMb = driveInfo.AvailableFreeSpace / (1024 * 1024);
                    StringBuilder sb = new StringBuilder(e.ToString());
                    sb.AppendLine($"Disk: {driveLetter}");
                    sb.AppendLine($"Available Space: {freeSpaceMb}mb");
                    Debug.LogError(sb.ToString());
					return;
                }
            }

			// Fallback to just logging the exception normally
            Debug.LogException(e);
		}

	}

}