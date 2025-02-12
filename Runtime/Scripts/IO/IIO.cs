using System;
using System.Text;

namespace HHG.Common.Runtime
{
    public interface IIO
	{
		byte[] ReadAllBytes(string fileName);
		void WriteAllBytes(string fileName, byte[] bytes);
		bool Exists(string fileName);
		void Clear(string fileName);
		void OnBeforeClose();
		void OnClose();
	}

	public static class IIOX
	{
		public static string ReadAllText(this IIO io, string fileName)
		{
			return Encoding.UTF8.GetString(io.ReadAllBytes(fileName));
		}

		public static string[] ReadAllLines(this IIO io, string fileName)
		{
			return io.ReadAllText(fileName).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}
	}
}