using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class Session : ScriptableObject
    {
        public class FileHandle : IDisposable
        {
            private Session session;
            private string previousFileId;
            private bool shouldLoadFile;

            public FileHandle()
            {

            }

            public FileHandle(Session session, string fileId, bool loadFile = false)
            {
                Initialize(session, fileId, loadFile);
            }

            public void Initialize(Session newSession, string newFileId, bool newLoadFile = false)
            {
                session = newSession;
                shouldLoadFile = newLoadFile;

                // Get previousFileId before changing it
                previousFileId = session.fileId;
                session.fileId = newFileId;

                if (shouldLoadFile)
                {
                    session.load();
                }
            }

            public void Dispose()
            {
                session.fileId = previousFileId;

                // Load previous file
                if (shouldLoadFile)
                {
                    session.load();
                }

                ObjectPool.Release(this);
            }
        }
    }
}