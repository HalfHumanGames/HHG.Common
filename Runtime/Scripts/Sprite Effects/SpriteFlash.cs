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
            if (hasStarted)
            {
                Flash();
            }
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

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(FlashAsync());
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
                    yield return new WaitForEndOfFrame();
                }

                yield return unscaledTime ? new WaitForSecondsRealtime(loopDelay) : new WaitForSeconds(loopDelay);
            }

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
        }
    }
}