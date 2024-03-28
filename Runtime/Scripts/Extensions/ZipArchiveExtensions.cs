using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace HHG.Common.Runtime
{
    public static class ZipArchiveExtensions
    {
        public static void CreateEntryFrom(this ZipArchive archive, string source, string entryName = "", List<ZipArchiveEntry> entries = null)
        {
            string filename = Path.GetFileName(source);

            // Path.Combine uses backslash instead of forwardslash (mac/linux)
            // Use forwardslash to make sure the app runs on all platforms
            string path = string.IsNullOrEmpty(entryName) ? filename : $"{entryName}/{filename}";

            if (File.GetAttributes(source).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(source, path, entries);
            }
            else
            {
                ZipArchiveEntry entry = archive.CreateEntryFromFile(source, path, CompressionLevel.Fastest);
                entries?.Add(entry);
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string directory, string entryName = "", List<ZipArchiveEntry> entries = null)
        {
            IEnumerable<string> files = Directory.GetFiles(directory).Concat(Directory.GetDirectories(directory));

            foreach (string file in files)
            {
                archive.CreateEntryFrom(file, entryName, entries);
            }
        }
    }
}