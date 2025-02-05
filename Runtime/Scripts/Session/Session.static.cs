using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class Session : ScriptableObject
    {
        protected static readonly Dictionary<Type, Session> sessions = new Dictionary<Type, Session>();

        public static TSession Get<TSession>() where TSession : Session
        {
            return Get(typeof(TSession)) as TSession;
        }

        public static Session Get(string typeName)
        {
            return Get(Type.GetType(typeName));
        }

        public static Session Get(Type type)
        {
            if (!sessions.TryGetValue(type, out Session session))
            {
                session = (Session)(FindAnyObjectByType(type) ?? CreateInstance(type));
                session.setup();
                sessions.Add(type, session);
            }

            return session;
        }
    }

    public abstract partial class Session<TSession, TState, TIO, TSerializer>
    {
        private static TSession instance;

        public static TSession Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Get(typeof(TSession)) as TSession;
                }
                return instance;
            }
        }

        public static string FileId
        {
            get => Instance.fileId;
            set => Instance.fileId = value;
        }

        public static ISessionState ReadOnlyStateWeak => Instance.readOnlyStateWeak;
        public static bool HasStagedChanges => Instance.hasStagedChanged;
        public static string DefaultFileId => Instance.defaultFileId;
        public static string TempFileId => Instance.tempFileId;
        public static string[] FileIds => Instance.fileIds;
        public static bool LogsEnabled => Instance.logsEnabled;
        public static TState DefaultState => Instance.defaultState;
        public static TState ReadOnlyState => Instance.readOnlyState;

        public static event Action StateUpdated
        {
            add => Instance.stateUpdated += value;
            remove => Instance.stateUpdated -= value;
        }

        public static void Save(Action<TState> mutation) => Instance.save(mutation);
        public static void Stage(Action<TState> mutation) => Instance.stage(mutation);
        public static void Save(string fileId = null) => Instance.save(fileId);
        public static void Load(string fileId = null) => Instance.load(fileId);
        public static void Clear(string fileId = null) => Instance.clear(fileId);
        public static void SaveStagedChanges() => Instance.saveStagedChanges();
        public static void ClearStagedChanges() => Instance.clearStagedChanges();
        public static void OnBeforeClose() => Instance.onBeforeClose();
        public static void OnClose() => Instance.onClose();
        public static void UseDefaultFile() => Instance.useDefaultFile();
        public static void IssueStateUpdated() => Instance.issueStateUpdated();
        public static bool FileExists(string fileId) => Instance.fileExists(fileId);
        public static string GetFileName(string fildId) => Instance.getFileName(fildId);
        public static string GetTempFileId(string fileId) => Instance.getTempFileId(fileId);
        public static string GetMostRecentFileId() => Instance.getMostRecentFileId();
        public static bool AnyFileExists() => Instance.anyFileExists();
        public static bool AnyTempFileExists() => Instance.anyTempFileExists();
        public static bool TempFileExists(string fileId = null) => Instance.tempFileExists(fileId);
        public static string GetJson() => Instance.getJson();
        public static FileHandle Handle(string fileId, bool loadFile = false) => Instance.handle(fileId, loadFile);
    }
}