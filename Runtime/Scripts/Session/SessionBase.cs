using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class SessionBase : ScriptableObject
    {
        [ContextMenu("Save")] private void _Save() => save();
        [ContextMenu("Load")] private void _Load() => load();
        [ContextMenu("Clear")] private void _Clear() => clear();

        protected abstract void save(string fileId = null);
        protected abstract void load(string fileId = null);
        protected abstract void clear(string fileId = null);
    }

    [ExecuteInEditMode]
    public abstract partial class SessionBase<TSession, TState, TIO, TSerializer> : SessionBase, IBindable, IBindableProvider
        where TSession : SessionBase<TSession, TState, TIO, TSerializer>
        where TState : class, ISessionState<TState>, new()
        where TIO : class, IIO, new()
        where TSerializer : ISerializer, new()
    {
        [SerializeField] private TState state;

        private TState stagedState;
        private TIO io = new TIO();
        private TSerializer serializer = new TSerializer();
        private List<Action<TState>> mutations = new List<Action<TState>>();
        private bool isStagedStateDirty;
        private string fileId = DefaultFileID;
        private GetSetMap getterSetterMap = new GetSetMap(typeof(TState));

        private TState defaultState
        {
            get
            {
                TState state = new TState();
                state.Reset();
                return state;
            }
        }

        private TState readOnlyState
        {
            get
            {
                if (state == null)
                {
                    Load();
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

        public event Action Updated;

        private void setup()
        {
            Application.quitting += onApplicationQuit;
        }

        protected sealed override void save(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            if (state == null)
            {
                Load(fileId);
                return;
            }

            mutate(fileId, saveFile => saveFile.OnBeforeSave());
            writeToDisk(fileId);

            if (fileId == FileId)
            {
                issueStateUpdated();
            }
        }

        protected sealed override void load(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            loadFromDisk(fileId);
            mutate(fileId, saveFile => saveFile.OnAfterLoad());
            isStagedStateDirty = true;

            if (fileId == FileId)
            {
                issueStateUpdated();
            }
        }

        protected sealed override void clear(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            io.Clear(getFileName(fileId));

            if (fileId == FileId)
            {
                clearStagedChangesNoCallback();
                state = DefaultState;
                issueStateUpdated();
            }
        }

        private void save(Action<TState> mutation)
        {
            mutate(FileId, mutation);
            isStagedStateDirty = true;
            Save();
        }

        private void stage(Action<TState> mutation)
        {
            mutations.Add(mutation);
            isStagedStateDirty = true;
            issueStateUpdated();
        }

        private void saveStagedChanges()
        {
            if (!HasStagedChanges) return;
            state = ReadOnlyState;
            clearStagedChangesNoCallback();
            Save();
        }

        private void clearStagedChanges()
        {
            clearStagedChangesNoCallback();
            issueStateUpdated();
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

        private void useDefaultFile()
        {
            fileId = DefaultFileID;
        }

        private void issueStateUpdated()
        {
            Updated?.Invoke();
        }

        private bool fileExists(string fileId)
        {
            return io.Exists(getFileName(fileId));
        }

        public string getFileName(string fileId)
        {
            return $"{GetType().ToString().ToLower()}.{fileId.ToLower()}.dat";
        }


        private string getJson()
        {
            return JsonUtility.ToJson(Instance.state);
        }

        public IBindable Bindable => instance;

        public bool TryGetValue<T>(string name, out T value)
        {
            return getterSetterMap.TryGetValue(readOnlyState, name, out value);
        }

        public bool TrySetValue<T>(string name, T value)
        {
            bool success = false;

            stage(state =>
            {
                success = getterSetterMap.TrySetValue(state, name, value);
            });

            _ = ReadOnlyState; // Force call mutation

            return success;
        }
    }
}