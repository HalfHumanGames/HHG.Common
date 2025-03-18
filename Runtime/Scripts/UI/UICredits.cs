//using System;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.InputSystem;

//namespace HHG.Common.Runtime
//{
//    public class UICredits : MonoBehaviour
//    {
//        [SerializeField] private float normalSpeed = 50f;
//        [SerializeField] private float fastForwardSpeed = 250f;
//        [SerializeField] private TextAsset credits;
//        [SerializeField] private TextMeshProUGUI sectionLabel;
//        [SerializeField] private TextMeshProUGUI credit1Label;
//        [SerializeField] private RectTransform credit2Labels;
//        [SerializeField] private ActionEvent onDone;

//        private RectTransform rect;
//        private RectTransform canvasRect;

//        private void Awake()
//        {
//            rect = GetComponent<RectTransform>();
//            canvasRect = rect.root.GetComponent<RectTransform>();

//            foreach (TextMeshProUGUI label in GetComponentsInChildren<TextMeshProUGUI>(true))
//            {
//                label.text = string.Empty;
//            }

//            string[] lines = credits.text.Split(Environment.NewLine);

//            List<TextMeshProUGUI> currentLabels = new List<TextMeshProUGUI>();

//            foreach (string line in lines)
//            {
//                if (string.IsNullOrEmpty(line))
//                {
//                    continue;
//                }

//                if (line[0] == '#')
//                {
//                    string header = line.Substring(1).Trim();
//                    TextMeshProUGUI label = Instantiate(sectionLabel, transform);
//                    label.text = header;

//                    currentLabels.Clear();
//                }
//                else
//                {
//                    string[] split = line.Split('|');

//                    if (split.Length == 1)
//                    {
//                        if (currentLabels.Count == 0)
//                        {
//                            Instantiate(credit1Label, transform).GetComponentsInChildren(true, currentLabels);
//                        }
//                        currentLabels[0].text += split[0] + Environment.NewLine;
//                    }
//                    else
//                    {
//                        if (currentLabels.Count == 0)
//                        {
//                            Instantiate(credit2Labels, transform).GetComponentsInChildren(true, currentLabels);
//                        }
//                        currentLabels[0].text += split[0] + Environment.NewLine;
//                        currentLabels[1].text += split[1] + Environment.NewLine;
//                    }
//                }
//            }

//            // Hide the template label game objects
//            sectionLabel.gameObject.SetActive(false);
//            credit1Label.gameObject.SetActive(false);
//            credit2Labels.gameObject.SetActive(false);
//        }

//        private void Start()
//        {
//            // Do after awake to make sure these gets set correctly
//            rect.RebuildLayout();
//            rect.SetAnchoredPosY(-canvasRect.sizeDelta.y);
//        }

//        private void Update()
//        {
//            float y = rect.anchoredPosition.y;

//            if (y < rect.sizeDelta.y)
//            {
//                bool fastForward = Keyboard.current.anyKey.isPressed;
//                float speed = fastForward ? fastForwardSpeed : normalSpeed;
//                rect.SetAnchoredPosY(y + speed * Time.deltaTime);
//            }
//            else
//            {
//                enabled = false;
//                onDone?.Invoke(this);
//            }
//        }
//    }
//}