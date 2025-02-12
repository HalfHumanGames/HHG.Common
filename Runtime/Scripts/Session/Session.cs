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

        public sealed override string fileId
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

        public sealed override string tempFileId => getTempFileId(fileId);
        public sealed override ISessionState readOnlyStateWeak => readOnlyState;
        public sealed override bool hasStagedChanged => mutations.Count > 0;

        // Only these properties can be overwritten in a subclass
        public override string defaultFileId => "0";
        public override string tempFileTag => "Temp";
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

        protected sealed override void setup()
        {
            Application.quitting += onApplicationQuit;
        }

        protected sealed override void log(string message)
        {
            if (logsEnabled)
            {
                Debug.Log($"{GetType()}: {message}");
            }
        }

        public sealed override void saveWeak(Action<object> mutation) => save(m => mutation((TState)m));
        public void save(Action<TState> mutation)
        {
            log($"Saving mutation for file: {fileId}");

            mutate(FileId, mutation);
            isStagedStateDirty = true;
            save();
        }

        public sealed override void stageWeak(Action<object> mutation) => stage(m => mutation((TState)m));
        public void stage(Action<TState> mutation)
        {
            log($"Staging mutation for file: {fileId}");

            mutations.Add(mutation);
            isStagedStateDirty = true;
            issueStateUpdated();
        }

        public sealed override void save(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = this.fileId;
            }

            log($"Saving file: {fileId}");

            if (state == null)
            {
                load(fileId);
                return;
            }

            mutate(fileId, state => state?.OnBeforeSave());
            writeToDisk(fileId);

            if (fileId == this.fileId)
            {
                issueStateUpdated();
            }
        }

        public sealed override void load(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = this.fileId;
            }

            log($"Loading file: {fileId}");

            loadFromDisk(fileId);
            mutate(fileId, state => state?.OnAfterLoad());
            isStagedStateDirty = true;

            if (fileId == this.fileId)
            {
                issueStateUpdated();
            }
        }

        public sealed override void clear(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = this.fileId;
            }

            log($"Clearing file: {fileId}");

            io.Clear(getFileName(fileId));

            if (fileId == this.fileId)
            {
                clearStagedChangesNoCallback();
                state = DefaultState;
                issueStateUpdated();
            }
        }

        public sealed override void saveStagedChanges()
        {
            log($"Saving staged changes for file: {fileId}");

            if (HasStagedChanges)
            {
                state = ReadOnlyState;
                clearStagedChangesNoCallback();
                save();
            }
        }

        public sealed override void clearStagedChanges()
        {
            log($"Clearing staged changes for file: {fileId}");

            clearStagedChangesNoCallback();
            issueStateUpdated();
        }

        public sealed override void onBeforeClose()
        {
            io.OnBeforeClose();
        }

        public sealed override void onClose()
        {
            io.OnClose();
        }

        public sealed override void useDefaultFile()
        {
            this.fileId = DefaultFileId;
        }

        public sealed override void issueStateUpdated()
        {
            stateUpdated?.Invoke();
        }

        public sealed override bool fileExists(string fileId)
        {
            return io.Exists(getFileName(fileId));
        }

        public sealed override string getFileName(string fileId)
        {
            return $"{GetType().ToString().ToLower()}.{fileId.ToLower()}.dat";
        }

        public sealed override string getTempFileId(string fileId)
        {
            return $"{fileId}.{tempFileTag}";
        }

        public sealed override string getMostRecentFileId()
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

        public sealed override bool anyFileExists()
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

        public sealed override bool anyTempFileExists()
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

        public sealed override bool tempFileExists(string fileId = null)
        {
            return fileExists(getTempFileId(fileId ?? this.fileId));
        }

        public sealed override string getJson()
        {
            return JsonUtility.ToJson(readOnlyStateWeak);
        }

        public sealed override T GetValue<T>(string name) => TryGetValue(name, out T value) ? value : default;

        public sealed override void SetValue<T>(string name, T value) => TrySetValue(name, value);

        public sealed override bool TryGetValue<T>(string name, out T value)
        {
            getterSetterMap ??= new GetSetMap(typeof(TState));

            return getterSetterMap.TryGetValue(readOnlyStateWeak, name, out value);
        }

        public sealed override bool TrySetValue<T>(string name, T value)
        {
            getterSetterMap ??= new GetSetMap(typeof(TState));

            bool success = false;

            stage(state =>
            {
                success = getterSetterMap.TrySetValue(state, name, value);
            });

            _ = readOnlyStateWeak; // Force call mutation

            return success;
        }

        public sealed override FileHandle handle(string fileId, bool loadFile = false)
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
                    state = defaultState;
                }
            }
            else
            {
                state = defaultState;
            }
        }

        private void onApplicationQuit()
        {
            io.OnBeforeClose();
            io.OnClose();
        }
    }
}