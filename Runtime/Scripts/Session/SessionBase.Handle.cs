using System;

namespace HHG.Common.Runtime
{
    public abstract partial class SessionBase<TSession, TState, TIO, TSerializer>
    {
        public static FileHandle Handle(string fileId) => new FileHandle(fileId);

        public class FileHandle : IDisposable
        {
            private string previousFileId;

            public FileHandle(string fileId)
            {
                previousFileId = FileId;
                FileId = fileId;
            }

            public void Dispose()
            {
                FileId = previousFileId;
            }
        }
    }
}