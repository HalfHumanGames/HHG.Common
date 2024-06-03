using System;

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

        public Timer(float seconds = 0f, Action timerDone = null)
        {
            Setup(seconds, timerDone);
        }

        public Timer(float seconds, int repetitions, Action timer, Action timerDone = null)
        {
            Setup(seconds, repetitions, timer, timerDone);
        }

        public void Setup(float seconds, Action timerDone)
        {
            repetitionCount = 0;
            time = 0f;
            duration = seconds;
            repeatCount = 0;
            onTimer = null;
            onTimerDone = timerDone;
        }

        public void Setup(float seconds, int repetitions, Action timer, Action timerDone = null)
        {
            repetitionCount = 0;
            time = 0f;
            duration = seconds;
            repeatCount = repetitions;
            onTimer = timer;
            onTimerDone = timerDone;
        }

        public void Update(float deltaTime)
        {
            if (time > 0)
            {
                time -= deltaTime;

                if (time <= 0)
                {
                    onTimer?.Invoke();

                    if (repeatCount >= 0 && ++repetitionCount >= repeatCount)
                    {
                        time = 0;

                        onTimerDone?.Invoke();
                    }
                    else
                    {
                        time = duration;
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