using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class PlayableController : MonoBehaviour
    {
        public bool IsPlaying => playable.IsPlaying;
        public ActionEvent<PlayableController> OnPlay => onPlay;
        public ActionEvent<PlayableController> OnPlayComplete => onPlayComplete;
        public ActionEvent<PlayableController> OnStop => onStop;
        public ActionEvent<PlayableController> OnPause => onPause;
        public ActionEvent<PlayableController> OnResume => onResume;

        [EditorButton(nameof(Play), PositionType = ButtonPositionType.Above)]
        [EditorButton(nameof(PlayRepeatedly), PositionType = ButtonPositionType.Above)]
        [EditorButton(nameof(Stop), PositionType = ButtonPositionType.Above)]
        [EditorButton(nameof(Pause), PositionType = ButtonPositionType.Above)]
        [EditorButton(nameof(Resume), PositionType = ButtonPositionType.Above)]

        [SerializeField] private float repeatInterval = 2f;

        [Header("Events")]
        [SerializeField] private ActionEvent<PlayableController> onPlay;
        [SerializeField] private ActionEvent<PlayableController> onPlayComplete;
        [SerializeField] private ActionEvent<PlayableController> onStop;
        [SerializeField] private ActionEvent<PlayableController> onPause;
        [SerializeField] private ActionEvent<PlayableController> onResume;

        private Playable _playable;
        private Playable playable => _playable ??= Playable.Create(gameObject);

        public Coroutine Play()
        {
            return Play(0f);
        }

        public Coroutine Play(float delay)
        {
            return StartCoroutine(PlayAsync(delay));
        }

        public Coroutine PlayAt(Vector3 position, float delay = 0f)
        {
            return StartCoroutine(PlayAtAsync(position, delay));
        }

        public Coroutine PlayRepeatedly()
        {
            return PlayRepeatedly(repeatInterval);
        }

        public Coroutine PlayRepeatedly(float interval)
        {
            return StartCoroutine(PlayRepeatedlyAsync(interval));
        }

        public IEnumerator PlayAsync(float delay = 0f)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);
            RaiseEvent(onPlay);
            yield return playable.Play();
            RaiseEvent(onPlayComplete);
        }

        public IEnumerator PlayAtAsync(Vector3 position, float delay = 0f)
        {
            transform.position = position;
            return PlayAsync(delay);
        }

        private IEnumerator PlayRepeatedlyAsync(float interval)
        {
            while (true)
            {
                yield return PlayAsync();
                yield return new WaitForSeconds(interval);
            }
        }

        public void Stop()
        {
            RaiseEvent(onStop);
            StopAllCoroutines();
            playable.Stop();
        }

        public void Pause()
        {
            RaiseEvent(onPause);
            playable.Pause();
        }

        public IEnumerator Resume()
        {
            RaiseEvent(onResume);
            yield return playable.Resume();
        }

        private void RaiseEvent(ActionEvent<PlayableController> actionEvent)
        {
            // Must be active since starts coroutine
            if (gameObject.activeInHierarchy)
            {
                actionEvent?.Invoke(this);
            }
        }
    }
}
