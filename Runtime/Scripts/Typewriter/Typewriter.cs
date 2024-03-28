using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class Typewriter
    {
        private static readonly char[] punctuation = new char[] { '.', ',', '!', '?', ':', ';', '—' };
        private static float fullPause = .5f;
        private static float quarterPause = .05f;

        public static IEnumerator Typewrite(TextMeshProUGUI text, string line, InputAction action)
        {
            text.text = string.Empty;

            for (int i = 0; i < line.Length; i++)
            {
                text.text = line.Substring(0, i + 1);

                float delay = Array.IndexOf(punctuation, text.text[i]) < 0 ? quarterPause : fullPause;

                if (action.IsPressed())
                {
                    text.text = line;

                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
}