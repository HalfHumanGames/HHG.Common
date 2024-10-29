using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UICarousel : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = .25f;
        [SerializeField] private SpriteListAsset spriteList;
        [SerializeField] private Image image;
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Button buttonRight;
        [SerializeField] private Button buttonJump;
        [SerializeField] private Button buttonContinue;
        [SerializeField] private ActionEvent onContinue;

        private List<Button> buttons = new List<Button>();
        private Coroutine coroutine;
        private int current;

        private void Awake()
        {
            buttonLeft.onClick.AddListener(PageLeft);
            buttonRight.onClick.AddListener(PageRight);
            buttonContinue.onClick.AddListener(Continue);
            buttonContinue.gameObject.SetActive(false);
            image.sprite = spriteList.Value[current];

            for (int i = 0; i < spriteList.Value.Count; i++)
            {
                int index = i;
                Button button = Instantiate(buttonJump, buttonJump.transform.parent);
                button.onClick.AddListener(() => PageTo(index));
                button.interactable = i == 0;
                buttons.Add(button);
            }

            // Update button visibility after all buttons are created
            UpdateButtonVisibility();
            Destroy(buttonJump.gameObject);
        }

        public void PageLeft()
        {
            current = Mathf.Clamp(current - 1, 0, spriteList.Value.Count - 1);
            Refresh();
        }

        public void PageRight()
        {
            current = Mathf.Clamp(current + 1, 0, spriteList.Value.Count - 1);
            Refresh();
        }

        public void PageTo(int index)
        {
            if (current != index)
            {
                current = Mathf.Clamp(index, 0, spriteList.Value.Count - 1);
                Refresh();
            }
        }

        public void Continue()
        {
            onContinue?.Invoke(this);
        }

        public void Refresh()
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(RefreshAsync());
            }
        }

        private IEnumerator RefreshAsync()
        {
            yield return TweenUtil.TweenAsync(() => image.color, value => image.color = value, Color.white.WithA(0f), fadeDuration, TimeScale.Unscaled, EaseUtil.InOutQuad);
            image.sprite = spriteList.Value[current];
            UpdateButtonVisibility(); // Do in the middle of the fade animation
            yield return TweenUtil.TweenAsync(() => image.color, value => image.color = value, Color.white.WithA(1f), fadeDuration, TimeScale.Unscaled, EaseUtil.InOutQuad);
            CoroutineUtil.StopAndNullifyCoroutine(ref coroutine);
        }

        private void UpdateButtonVisibility()
        {
            buttons[current].interactable = true;
            buttonLeft.gameObject.SetActive(current > 0);
            buttonRight.gameObject.SetActive(current < spriteList.Value.Count - 1);

            if (current == spriteList.Value.Count - 1)
            {
                buttonContinue.gameObject.SetActive(true);
            }
        }
    }
}