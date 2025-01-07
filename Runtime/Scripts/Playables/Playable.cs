using System;
using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public partial class Playable
    {
        private readonly Action play;
        private readonly Action stop;
        private readonly Action pause;
        private readonly Action resume;
        private readonly List<Playable> children = new List<Playable>();

        public event Action Played;
        public event Action Stopped;
        public event Action Paused;
        public event Action Resumed;

        public Playable()
        {

        }

        public Playable(Action play, Action stop = null, Action pause = null, Action resume = null)
        {
            this.play = play;
            this.stop = stop;
            this.pause = pause;
            this.resume = resume;
        }

        public void Play()
        {
            play?.Invoke();
            Played?.Invoke();

            foreach (Playable child in children)
            {
                child.Play();
            }
        }

        public void Stop()
        {
            stop?.Invoke();
            Stopped?.Invoke();

            foreach (Playable child in children)
            {
                child.Stop();
            }
        }

        public void Pause()
        {
            pause?.Invoke();
            Paused?.Invoke();

            foreach (Playable child in children)
            {
                child.Pause();
            }
        }

        public void Resume()
        {
            resume?.Invoke();
            Resumed?.Invoke();

            foreach (Playable child in children)
            {
                child.Resume();
            }
        }

        public void Add(Playable child)
        {
            if (child != null && !children.Contains(child))
            {
                children.Add(child);
            }
        }

        public void Remove(Playable child)
        {
            if (child != null && children.Contains(child))
            {
                children.Remove(child);
            }
        }

        public void AddRange(IEnumerable<Playable> childrenToAdd)
        {
            foreach (Playable child in childrenToAdd)
            {
                Add(child);
            }
        }

        public void RemoveRange(IEnumerable<Playable> childrenToRemove)
        {
            foreach (Playable child in childrenToRemove)
            {
                Remove(child);
            }
        }

        public void Clear()
        {
            children.Clear();
        }
    }
}
