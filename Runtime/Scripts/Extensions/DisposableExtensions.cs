using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class DisposableExtensions
    {
        private static readonly HashSet<Tracked> tracked = new HashSet<Tracked>();

        static DisposableExtensions()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            using (Pool.GetList(out List<Tracked> copy))
            {
                copy.AddRange(tracked);

                foreach (IDisposable disposable in copy)
                {
                    disposable.Dispose();
                }

                tracked.Clear();
            }
        }

        public static IDisposable DisposeOnSceneUnloaded(this IDisposable disposable)
        {
            Tracked track = Pool.Get<Tracked>();
            track.Initialize(disposable);
            tracked.Add(track);
            return track;
        }

        private class Tracked : IDisposable
        {
            private IDisposable disposable;
            private bool disposed;

            public void Initialize(IDisposable newDisposable)
            {
                disposable = newDisposable;
                disposed = false;
            }

            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                disposable?.Dispose();
                tracked.Remove(this);
                Pool.Release(this);
            }
        }
    }
}