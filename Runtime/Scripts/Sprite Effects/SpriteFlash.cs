using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SpriteFlash : MonoBehaviour
    {
        private const string flashShaderName = "Sprites/Flash";

        [SerializeField] private Color color = Color.white;
        [SerializeField] private float duration = 1f;
        [SerializeField] private int loops;
        [SerializeField] private float loopDelay;
        [SerializeField] private float cooldown;
        [SerializeField] private bool unscaledTime;

        private Material material;
        private Shader originalShader;
        private Shader flashShader;
        private Coroutine coroutine;
        private bool hasStarted;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
            originalShader = material.shader;
            flashShader = Shader.Find(flashShaderName);
        }

        private void Start()
        {
            hasStarted = true;
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

            material.shader = flashShader;
            material.SetColor("_FlashColor", color);

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
            material.SetFloat("_FlashAmount", amount);
        }

        private void Cleanup()
        {
            StopAllCoroutines();
            SetFlashAmount(0);
            material.shader = originalShader;
            coroutine = null;
        }
    }
}