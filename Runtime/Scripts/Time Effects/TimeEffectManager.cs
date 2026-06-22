using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum ConflictMode
    {
        Ignore,
        Interrupt,
        Queue
    }

    public class TimeEffectManager : MonoBehaviour
    {
        private static TimeEffectManager instance;

        public static bool HasInstance => instance != null;

        public static TimeEffectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("Time Effect Manager");
                    instance = go.AddComponent<TimeEffectManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public TimeEffectSource CurrentSource { get; private set; }
        public bool IsPlaying => coroutine != null;

        private Coroutine coroutine;
        private readonly Queue<TimeEffect> effectQueue = new Queue<TimeEffect>();

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                Time.timeScale = 1f;
            }
        }

        public void Play(TimeEffectSource source, List<TimeEffect> steps, ConflictMode mode)
        {
            if (steps == null || steps.Count == 0) return;

            if (IsPlaying)
            {
                switch (mode)
                {
                    case ConflictMode.Ignore:
                        return;
                    case ConflictMode.Queue:
                        foreach (var step in steps) effectQueue.Enqueue(step);
                        return;
                    case ConflictMode.Interrupt:
                        StopCoroutine(coroutine);
                        effectQueue.Clear();
                        Time.timeScale = 1f;
                        break;
                }
            }

            CurrentSource = source;
            foreach (var step in steps) effectQueue.Enqueue(step);
            coroutine = StartCoroutine(RunSteps());
        }

        public void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            effectQueue.Clear();
            CurrentSource = null;
            Time.timeScale = 1f;
        }

        private IEnumerator RunSteps()
        {
            while (effectQueue.Count > 0)
            {
                yield return effectQueue.Dequeue().Execute();
                Time.timeScale = 1f; // Reset after each effect
            }

            CurrentSource = null;
            coroutine = null;
        }
    }
}
