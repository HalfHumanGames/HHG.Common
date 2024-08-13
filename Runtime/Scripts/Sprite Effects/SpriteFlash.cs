using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SpriteFlash : MonoBehaviour
    {
        private static readonly int flashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int flashAmount = Shader.PropertyToID("_FlashAmount");

        [SerializeField] private Color color = Color.white;
        [SerializeField] private float duration = 1f;
        [SerializeField] private int loops;
        [SerializeField] private float loopDelay;
        [SerializeField] private float cooldown;
        [SerializeField] private bool unscaledTime;

        private Renderer flashRenderer;
        private MaterialPropertyBlock block;
        private Coroutine coroutine;

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            flashRenderer = GetComponent<Renderer>();
            flashRenderer.GetPropertyBlock(block);
            SetFlashAmount(0);
        }

        private void OnEnable()
        {
            // Do not auto-flash in OnEnable
            // Explicitly call Flash manually
        }

        private void OnDisable()
        {
            Cleanup();
        }

        [ContextMenu("Flash")]
        public void Flash()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (coroutine == null)
            {
                coroutine = StartCoroutine(FlashAsync());
            }  
        }

        private IEnumerator FlashAsync()
        {
            block.SetColor(flashColor, color);

            for (int i = 0; loops < 0 || i <= loops; i++)
            {
                float time = 0;

                while (time < duration)
                {
                    time += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    float perc = time / duration;
                    SetFlashAmount(1f - perc);
                    yield return WaitFor.EndOfFrame;
                }

                yield return unscaledTime ? new WaitForSecondsRealtime(loopDelay) : new WaitForSeconds(loopDelay);
            }

            yield return unscaledTime ? new WaitForSecondsRealtime(cooldown) : new WaitForSeconds(cooldown);

            Cleanup();
        }

        private void SetFlashAmount(float amount)
        {
            block.SetFloat(flashAmount, amount);
            flashRenderer.SetPropertyBlock(block);
        }

        private void Cleanup()
        {
            StopAllCoroutines();
            SetFlashAmount(0);
            coroutine = null;
        }
    }
}