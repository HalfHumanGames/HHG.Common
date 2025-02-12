using System;
using System.IO;
using System.Text;
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
                LogExceptionWithDriveInfo(e);
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
			catch (Exception e)
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
			catch (Exception e)
			{
                LogExceptionWithDriveInfo(e);
			}
		}

		public string GetFilePath(string fileName)
		{
			return string.IsNullOrEmpty(getRoot) ? fileName : $"{getRoot}/{fileName}";
		}

		public void OnBeforeClose() { /* Do nothing */ }
		public void OnClose() { /* Do nothing */ }

		private void LogExceptionWithDriveInfo(Exception e)
		{
			string driveLetter = Path.GetPathRoot(Application.persistentDataPath);
            
			if (!string.IsNullOrEmpty(driveLetter))
			{
                DriveInfo driveInfo = new DriveInfo(driveLetter);

				if (driveInfo != null)
				{
                    long freeSpaceMB = driveInfo.AvailableFreeSpace / (1024 * 1024);

                    StringBuilder sb = new StringBuilder(e.ToString());
                    sb.AppendLine($"Disk: {driveLetter}");
                    sb.AppendLine($"Available Space: {freeSpaceMB}mb");
                    Debug.LogError(sb.ToString());
					return;
                }
            }

			// Fallback to just logging the exception normally
            Debug.LogException(e);
		}

	}

}