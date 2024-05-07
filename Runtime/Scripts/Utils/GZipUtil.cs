using System.IO;
using System.IO.Compression;

namespace HHG.Common.Runtime
{
    public static class GZipUtil
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return memory.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream memory = new MemoryStream(data))
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    using (MemoryStream decompressedMemoryStream = new MemoryStream())
                    {
                        gzip.CopyTo(decompressedMemoryStream);
                        return decompressedMemoryStream.ToArray();
                    }
                }
            }
        }
    }
}