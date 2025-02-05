using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class Session : ScriptableObject
    {
        [ContextMenu("Save")] private void _Save() => save();
        [ContextMenu("Load")] private void _Load() => load();
        [ContextMenu("Clear")] private void _Clear() => clear();

        public abstract string fileId { get; set; }
        public abstract ISessionState readOnlyStateWeak { get; }
        public abstract bool hasStagedChanged { get; }
        public abstract string defaultFileId { get; }
        public abstract string tempFileId { get; }
        public abstract string[] fileIds { get; }
        public abstract bool logsEnabled { get; }

        protected abstract void setup();
        protected abstract void log(string message);

        public abstract void saveWeak(Action<object> mutation);
        public abstract void stageWeak(Action<object> mutation);
        public abstract void save(string fileId = null);
        public abstract void load(string fileId = null);
        public abstract void clear(string fileId = null);
        public abstract void saveStagedChanges();
        public abstract void clearStagedChanges();
        public abstract void onBeforeClose();
        public abstract void onClose();
        public abstract void useDefaultFile();
        public abstract void issueStateUpdated();
        public abstract bool fileExists(string fileId);
        public abstract string getFileName(string fildId);
        public abstract string getTempFileId(string fileId);
        public abstract string getMostRecentFileId();
        public abstract bool anyFileExists();
        public abstract bool anyTempFileExists();
        public abstract bool tempFileExists(string fileId = null);
        public abstract string getJson();
        public abstract T GetValue<T>(string name);
        public abstract void SetValue<T>(string name, T value);
        public abstract bool TryGetValue<T>(string name, out T value);
        public abstract bool TrySetValue<T>(string name, T value);
        public abstract FileHandle handle(string fileId, bool loadFile = false);
    }
}