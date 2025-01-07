using UnityEngine;

namespace HHG.Common.Runtime
{
    public class PlayableController : MonoBehaviour
    {
        private Playable playable;

        private void Start()
        {
            playable = Playable.Create(gameObject);
        }

        public void PlayAt(Vector3 position)
        {
            transform.position = position;
            Play();
        }

        public void Play()
        {
            playable.Play();
        }

        public void Stop()
        {
            playable.Stop();
        }

        public void Pause()
        {
            playable.Pause();
        }

        public void Resume()
        {
            playable.Resume();
        }
    }
}