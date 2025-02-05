using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public abstract partial class Session<TSession, TState, TIO, TSerializer> : Session, IBindable, IBindableProvider
        where TSession : Session<TSession, TState, TIO, TSerializer>
        where TState : class, ISessionState<TState>, new()
        where TIO : class, IIO, new()
        where TSerializer : ISerializer, new()
    {
        public TState defaultState
        {
            get
            {
                TState state = new TState();
                state.Reset();
                return state;
            }
        }

        public TState readOnlyState
        {
            get
            {
                if (state == null)
                {
                    load();
                }
                if (stagedState == null || isStagedStateDirty)
                {
                    TState temp = state.Clone();
                    foreach (Action<TState> mutation in mutations)
                    {
                        mutation(temp);
                    }
                    stagedState = temp;
                    isStagedStateDirty = false;
                }
                return stagedState;
            }
        }

        public override string fileId
        {
            get => _fileId ?? defaultFileId;
            set
            {
                if (_fileId != value)
                {
                    log($"Changing file id from '{_fileId}' to '{value}'");
                    _fileId = value ?? defaultFileId;
                }
            }
        }

        public override ISessionState readOnlyStateWeak => readOnlyState;
        public override bool hasStagedChanged => mutations.Count > 0;
        public override string defaultFileId => "0";
        public override string tempFileId => "Temp";
        public override string[] fileIds => new string[] { defaultFileId };
        public override bool logsEnabled => false;

        public IBindable Bindable => Instance;

        [SerializeField] private TState state;

        private string _fileId;
        private bool isStagedStateDirty;
        private TState stagedState;
        private TIO io = new TIO();
        private TSerializer serializer = new TSerializer();
        private List<Action<TState>> mutations = new List<Action<TState>>();
        private GetSetMap getterSetterMap;

        public event Action stateUpdated;

        protected override void setup()
        {
            sessions[GetType()] = this;

            Application.quitting += onApplicationQuit;
        }

        protected override void log(string message)
        {
            if (logsEnabled)
            {
                Debug.Log($"{GetType()}: {message}");
            }
        }

        public override void saveWeak(Action<object> mutation) => save(m => mutation((TState)m));
        public void save(Action<TState> mutation)
        {
            log($"Saving mutation for file: {fileId}");

            mutate(FileId, mutation);
            isStagedStateDirty = true;
            save();
        }

        public override void stageWeak(Action<object> mutation) => stage(m => mutation((TState)m));
        public void stage(Action<TState> mutation)
        {
            log($"Staging mutation for file: {fileId}");

            mutations.Add(mutation);
            isStagedStateDirty = true;
            issueStateUpdated();
        }

        public override void save(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            log($"Saving file: {fileId}");

            if (state == null)
            {
                load(fileId);
                return;
            }

            mutate(fileId, saveFile => saveFile.OnBeforeSave());
            writeToDisk(fileId);

            if (fileId == FileId)
            {
                issueStateUpdated();
            }
        }

        public override void load(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            log($"Loading file: {fileId}");

            loadFromDisk(fileId);
            mutate(fileId, saveFile => saveFile.OnAfterLoad());
            isStagedStateDirty = true;

            if (fileId == FileId)
            {
                issueStateUpdated();
            }
        }

        public override void clear(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            log($"Clearing file: {fileId}");

            io.Clear(getFileName(fileId));

            if (fileId == FileId)
            {
                clearStagedChangesNoCallback();
                state = DefaultState;
                issueStateUpdated();
            }
        }

        public override void saveStagedChanges()
        {
            log($"Saving staged changes for file: {fileId}");

            if (HasStagedChanges)
            {
                state = ReadOnlyState;
                clearStagedChangesNoCallback();
                save();
            }
        }

        public override void clearStagedChanges()
        {
            log($"Clearing staged changes for file: {fileId}");

            clearStagedChangesNoCallback();
            issueStateUpdated();
        }

        public override void onBeforeClose()
        {
            io.OnBeforeClose();
        }

        public override void onClose()
        {
            io.OnClose();
        }

        public override void useDefaultFile()
        {
            FileId = DefaultFileId;
        }

        public override void issueStateUpdated()
        {
            stateUpdated?.Invoke();
        }

        public override bool fileExists(string fileId)
        {
            return io.Exists(getFileName(fileId));
        }

        public override string getFileName(string fileId)
        {
            return $"{GetType().ToString().ToLower()}.{fileId.ToLower()}.dat";
        }

        public override string getTempFileId(string fileId)
        {
            return $"{fileId}.{tempFileId}";
        }

        public override string getMostRecentFileId()
        {
            string id = defaultFileId;
            DateTime dt = new DateTime();

            for (int i = 0; i < fileIds.Length; i++)
            {
                if (fileExists(fileIds[i]))
                {
                    using (Handle(fileIds[i], true))
                    {
                        if (readOnlyState.Timestamp > dt)
                        {
                            id = fileIds[i];
                            dt = readOnlyState.Timestamp;
                        }
                    }
                }
            }

            return id;
        }

        public override bool anyFileExists()
        {
            for (int i = 0; i < fileIds.Length; i++)
            {
                if (fileExists(fileIds[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool anyTempFileExists()
        {
            for (int i = 0; i < fileIds.Length; i++)
            {
                if (fileExists(getTempFileId(fileIds[i])))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool tempFileExists(string fileId = null)
        {
            return fileExists(getTempFileId(fileId ?? this.fileId));
        }

        public override string getJson()
        {
            return JsonUtility.ToJson(readOnlyStateWeak);
        }

        public override T GetValue<T>(string name) => TryGetValue(name, out T value) ? value : default;

        public override void SetValue<T>(string name, T value) => TrySetValue(name, value);

        public override bool TryGetValue<T>(string name, out T value)
        {
            getterSetterMap ??= new GetSetMap(this);

            return getterSetterMap.TryGetValue(readOnlyStateWeak, name, out value);
        }

        public override bool TrySetValue<T>(string name, T value)
        {
            getterSetterMap ??= new GetSetMap(this);

            bool success = false;

            stageWeak(state =>
            {
                success = getterSetterMap.TrySetValue(state, name, value);
            });

            _ = readOnlyStateWeak; // Force call mutation

            return success;
        }

        public override FileHandle handle(string fileId, bool loadFile = false)
        {
            FileHandle handle = ObjectPool.Get<FileHandle>();
            handle.Initialize(this, fileId, loadFile);
            return handle;
        }

        private void clearStagedChangesNoCallback()
        {
            mutations.Clear();
            stagedState = null;
            isStagedStateDirty = true;
        }

        private void mutate(string fileId, Action<TState> mutation)
        {
            if (state == null)
            {
                loadFromDisk(fileId);
            }
            TState temp = state.Clone();
            mutation(temp);
            state = temp;
        }

        private void writeToDisk(string fileId)
        {
            byte[] bytes = serializer.Serialize(state);
            io.WriteAllBytes(getFileName(fileId), bytes);
        }

        private void loadFromDisk(string fileId)
        {
            if (io.Exists(getFileName(fileId)))
            {
                try
                {
                    byte[] bytes = io.ReadAllBytes(getFileName(fileId));
                    state = serializer.Deserialize<TState>(bytes);
                }
                catch
                {
                    state = DefaultState;
                }
            }
            else
            {
                state = DefaultState;
            }
        }

        private void onApplicationQuit()
        {
            io.OnBeforeClose();
            io.OnClose();
        }
    }
}