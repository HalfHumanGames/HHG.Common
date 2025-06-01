using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class PlayableController : MonoBehaviour
    {
        [SerializeField] private float repeatInterval = 2f;

        private Playable _playable;
        private Playable playable => _playable ??= Playable.Create(gameObject);

        public CustomYieldInstruction PlayAt(Vector3 position)
        {
            transform.position = position;
            return Play();
        }

        [ContextMenu("Play Repeatedly")]
        public Coroutine PlayRepeatedly()
        {
            return PlayRepeatedly(repeatInterval);
        }

        public Coroutine PlayRepeatedly(float interval)
        {
            return StartCoroutine(PlayRepeatedlyAsync(interval));
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
        public CustomYieldInstruction Play()
        {
            return playable.Play();
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
        public CustomYieldInstruction Resume()
        {
            return playable.Resume();
        }
    }
}