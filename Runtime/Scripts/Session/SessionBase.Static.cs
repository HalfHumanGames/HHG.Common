using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class SessionBase<TSession, TState, TIO, TSerializer>
    {
        public const string DefaultFileID = "__default";

        public static string FileId { get => Instance.fileId ?? DefaultFileID; set => Instance.fileId = value ?? DefaultFileID; }
        public static bool HasStagedChanges => Instance.mutations.Count > 0;
        public static TState DefaultState => Instance.defaultState;
        public static TState ReadOnlyState => Instance.readOnlyState;

        public static TSession Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TSession>() ?? CreateInstance<TSession>();
                    instance.setup();
                }
                return instance;
            }
        }

        private static TSession instance;

        public static event Action StateUpdated
        {
            add => Instance.Updated += value;
            remove => Instance.Updated -= value;
        }

        public static void Save(string fileId = null) => Instance.save(fileId);
        public static void Load(string fileId = null) => Instance.load(fileId);
        public static void Clear(string fileId = null) => Instance.clear(fileId);
        public static void Save(Action<TState> mutation) => Instance.save(mutation);
        public static void Stage(Action<TState> mutation) => Instance.stage(mutation);
        public static void SaveStagedChanges() => Instance.saveStagedChanges();
        public static void ClearStagedChanges() => Instance.clearStagedChanges();
        public static void OnBeforeClose() => Instance.io.OnBeforeClose();
        public static void OnClose() => Instance.io.OnClose();
        public static void UseDefaultFile() => Instance.useDefaultFile();
        public static void IssueStateUpdated() => Instance.issueStateUpdated();
        public static bool FileExists(string fileId) => Instance.fileExists(fileId);
        public static string GetJson() => Instance.getJson();
        public static string GetFileName(string fileId) => $"{Application.productName.ToLower()}.{typeof(TState).ToString().ToLower()}.{fileId}.dat";
    }
}