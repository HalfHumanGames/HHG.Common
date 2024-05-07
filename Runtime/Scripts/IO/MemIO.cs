using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public class MemIO : IIO
	{
		private Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

		public void Clear(string fileName)
		{
			if (files.ContainsKey(fileName))
			{
				files.Remove(fileName);
			}
		}

		public bool Exists(string fileName)
		{
			return files.ContainsKey(fileName);
		}

		public byte[] ReadAllBytes(string fileName)
		{
			if (files.ContainsKey(fileName))
			{
				return files[fileName];
			}
			return new byte[0];
		}

		public void WriteAllBytes(string fileName, byte[] bytes)
		{
			files[fileName] = bytes;
		}

		public void OnBeforeClose() { /* Do nothing */ }
		public void OnClose() { /* Do nothing */ }
	}
}