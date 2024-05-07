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

        protected abstract void save();
        protected abstract void load();
        protected abstract void clear(string fileId = null);
    }

    [ExecuteInEditMode]
    public abstract partial class SessionBase<TSession, TState, TIO, TSerializer> : SessionBase
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

        private event Action<TState> stateUpdated;

        private void setup()
        {
            Application.quitting += onApplicationQuit;
        }

        protected sealed override void save()
        {
            if (state == null)
            {
                Load();
                return;
            }
            mutate(saveFile => saveFile.OnBeforeSave());
            writeToDisk();
            issueStateUpdated();
        }

        protected sealed override void load()
        {
            loadFromDisk();
            mutate(saveFile => saveFile.OnAfterLoad());
            isStagedStateDirty = true;
            issueStateUpdated();
        }

        protected sealed override void clear(string fileId = null)
        {
            if (fileId == null)
            {
                fileId = FileId;
            }

            io.Clear(GetFileName(fileId));

            if (fileId == FileId)
            {
                clearStagedChangesNoCallback();
                state = DefaultState;
                issueStateUpdated();
            }
        }

        private void save(Action<TState> mutation)
        {
            mutate(mutation);
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

        private void mutate(Action<TState> mutation)
        {
            if (state == null)
            {
                loadFromDisk();
            }
            TState temp = state.Clone();
            mutation(temp);
            state = temp;
        }

        private void writeToDisk()
        {
            byte[] bytes = serializer.Serialize(state);
            io.WriteAllBytes(GetFileName(FileId), bytes);
        }

        private void loadFromDisk()
        {
            if (io.Exists(GetFileName(FileId)))
            {
                try
                {
                    byte[] bytes = io.ReadAllBytes(GetFileName(FileId));
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
            stateUpdated?.Invoke(ReadOnlyState);
        }

        private bool fileExists(string fileId)
        {
            return io.Exists(GetFileName(fileId));
        }

        private string getJson()
        {
            return JsonUtility.ToJson(Instance.state);
        }
    }
}