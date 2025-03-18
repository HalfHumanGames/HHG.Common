using System.Text;

namespace HHG.Common.Runtime
{
    public interface IIO
	{
		byte[] ReadAllBytes(string fileName);
		void WriteAllBytes(string fileName, byte[] bytes);
		bool Exists(string fileName);
		void Delete(string fileName);
	}

	public static class IIOX
	{
		public static string ReadAllText(this IIO io, string fileName)
		{
			return Encoding.UTF8.GetString(io.ReadAllBytes(fileName));
		}

		public static void WriteAllText(this IIO io, string fileName, string text)
        {
            io.WriteAllBytes(fileName, Encoding.UTF8.GetBytes(text));
        }
    }
}