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

        public static IEnumerator Typewrite(TMP_Text text, string line, InputAction action = null)
        {
            text.text = string.Empty;

            for (int i = 0; i < line.Length; i++)
            {
                char currentChar = line[i];

                // If we hit an opening tag, skip to the end of the tag
                if (currentChar == '<')
                {
                    int tagEndIndex = line.IndexOf('>', i);
                    if (tagEndIndex != -1)
                    {
                        // Include the entire tag in the output immediately
                        i = tagEndIndex;
                        text.text = line.Substring(0, i + 1);
                        continue; // Continue to next character for tags
                    }
                }

                text.text = line.Substring(0, i + 1);

                float delay = Array.IndexOf(punctuation, currentChar) < 0 ? quarterPause : fullPause;

                if (action != null && action.IsPressed())
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