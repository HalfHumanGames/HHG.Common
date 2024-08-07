using UnityEngine;
using Action = System.Action;

namespace HHG.Common.Runtime
{
    public class Timer
    {
        private int repetitionCount;
        private float time;
        private float duration;
        private int repeatCount;
        private Action onTimer;
        private Action onTimerDone;

        public Timer(float seconds, Action timerDone) : this(seconds, 0, null, timerDone)
        {
           
        }

        public Timer(float seconds, int repetitions, Action timer, Action timerDone = null)
        {
            repetitionCount = 0;
            time = 0f;
            duration = seconds;
            repeatCount = repetitions;
            onTimer = timer;
            onTimerDone = timerDone;
        }

        public void SetTime(float newTime)
        {
            time = newTime;
        }

        public void Update(float deltaTime)
        {
            if (time > 0)
            {
                time -= deltaTime;

                if (time <= 0)
                {
                    if (repeatCount >= 0 && ++repetitionCount >= repeatCount)
                    {
                        time = 0;
                        onTimer?.Invoke();
                        onTimerDone?.Invoke();
                    }
                    else
                    {
                        time = duration;
                        onTimer?.Invoke();
                    }
                }
            }
        }

        public void Start(bool trigger = false)
        {
            if (trigger)
            {
                onTimer?.Invoke();
            }

            time = duration;
        }

        public void StartRandom(bool trigger = false)
        {
            if (trigger)
            {
                onTimer?.Invoke();
            }

            time = Random.Range(.01f, duration);
        }

        public void Stop()
        {
            time = 0;
        }

        public void Reset(bool start = false)
        {
            repetitionCount = 0;
            time = 0;

            if (start)
            {
                Start();
            }
        }
    }
}