using System;

namespace HHG.Common.Runtime
{
    public abstract partial class SessionBase<TSession, TState, TIO, TSerializer>
    {
        public static FileHandle Handle(string fileId, bool loadFile = false) => new FileHandle(fileId, loadFile);

        public class FileHandle : IDisposable
        {
            private string previousFileId;
            private bool shouldLoadFile;

            public FileHandle(string fileId, bool loadFile = false)
            {
                previousFileId = FileId;
                shouldLoadFile = loadFile;
                FileId = fileId;

                if (shouldLoadFile)
                {
                    Load();
                }
            }

            public void Dispose()
            {
                FileId = previousFileId;

                // Load previous file
                if (shouldLoadFile)
                {
                    Load();
                }
            }
        }
    }
}