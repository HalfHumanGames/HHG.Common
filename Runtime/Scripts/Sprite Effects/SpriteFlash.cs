using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SpriteFlash : MonoBehaviour
    {
        private const string flashShaderName = "Sprites/Sprite Flash";

        [SerializeField] private Color color;
        [SerializeField] private float duration;

        private Material material;
        private Shader originalShader;
        private Shader flashShader;
        private Coroutine coroutine;
        private bool hasStarted;

        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material;
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
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(FlashAsync());
        }

        private IEnumerator FlashAsync()
        {
            float time = 0;

            material.shader = flashShader;
            material.SetColor("_FlashColor", color);

            while (time < duration)
            {
                time += Time.deltaTime;
                float perc = time / duration;
                SetFlashAmount(1f - perc);
                yield return null;
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