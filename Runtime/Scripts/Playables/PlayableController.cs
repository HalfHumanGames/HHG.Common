using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class PlayableController : MonoBehaviour
    {
        [SerializeField] private float repeatInterval = 2f;

        private Playable _playable;
        private Playable playable => _playable ??= Playable.Create(gameObject);

        public void PlayAt(Vector3 position)
        {
            transform.position = position;
            Play();
        }

        [ContextMenu("Play Repeatedly")]
        public void PlayRepeatedly()
        {
            PlayRepeatedly(repeatInterval);
        }

        public void PlayRepeatedly(float interval)
        {
            StartCoroutine(PlayRepeatedlyAsync(interval));
        }

        private IEnumerator PlayRepeatedlyAsync(float interval)
        {
            while (true)
            {
                playable.Play();
                yield return new WaitForSeconds(interval);
            }
        }

        [ContextMenu("Play")]
        public void Play()
        {
            playable.Play();
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            playable.Stop();
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            playable.Pause();
        }

        [ContextMenu("Resume")]
        public void Resume()
        {
            playable.Resume();
        }
    }
}