using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class PlayableController : MonoBehaviour
    {
        public bool IsPlaying => playable.IsPlaying;
        public ActionEvent OnPlay => onPlay;
        public ActionEvent OnStop => onStop;
        public ActionEvent OnPause => onPause;
        public ActionEvent OnResume => onResume;

        [SerializeField] private float repeatInterval = 2f;

        [Header("Events")]
        [SerializeField] private ActionEvent onPlay;
        [SerializeField] private ActionEvent onStop;
        [SerializeField] private ActionEvent onPause;
        [SerializeField] private ActionEvent onResume;

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
                Play();
                yield return new WaitForSeconds(interval);
            }
        }

        [ContextMenu("Play")]
        public CustomYieldInstruction Play()
        {
            CustomYieldInstruction instruction = playable.Play();
            InvokeEvent(onPlay);
            return instruction;
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            playable.Stop();
            InvokeEvent(onStop);
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            playable.Pause();
            InvokeEvent(onPause);
        }

        [ContextMenu("Resume")]
        public CustomYieldInstruction Resume()
        {
            CustomYieldInstruction instruction = playable.Resume();
            InvokeEvent(onResume);
            return instruction;
        }

        private void InvokeEvent(ActionEvent actionEvent)
        {
            // Must be active since starts coroutine
            if (gameObject.activeInHierarchy)
            {
                actionEvent?.Invoke(this);
            }
        }
    }
}