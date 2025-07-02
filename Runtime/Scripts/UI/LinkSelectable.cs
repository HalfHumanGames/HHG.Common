using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class LinkSelectable : Selectable, ISubmitHandler, IPointerClickHandler, IPointerMoveHandler
    {
        public ActionEvent<LinkSelectable, string> Clicked => clicked;
        public ActionEvent<LinkSelectable, string> Highlighted => highlighted;
        public ActionEvent<LinkSelectable, string> Unhighlighted => unhighlighted;
        public ActionEvent<LinkSelectable, string> Selected => selected;
        public ActionEvent<LinkSelectable, string> Deselected => deselected;

        [Header("Styles")]
        [SerializeField] private string highlightedStyle = "<color=yellow>{0}</color>";
        [SerializeField] private string selectedStyle = "<color=orange>{0}</color>";

        [Header("Events")]
        [SerializeField] private bool logEvents;
        [SerializeField] private ActionEvent<LinkSelectable, string> clicked = new ActionEvent<LinkSelectable, string>();
        [SerializeField] private ActionEvent<LinkSelectable, string> highlighted = new ActionEvent<LinkSelectable, string>();
        [SerializeField] private ActionEvent<LinkSelectable, string> unhighlighted = new ActionEvent<LinkSelectable, string>();
        [SerializeField] private ActionEvent<LinkSelectable, string> selected = new ActionEvent<LinkSelectable, string>();
        [SerializeField] private ActionEvent<LinkSelectable, string> deselected = new ActionEvent<LinkSelectable, string>();

        private TMP_Text label;
        private readonly List<LinkData> allLinks = new List<LinkData>();
        private LinkData _highlightedLink = LinkData.Invalid;
        private LinkData _selectedLink = LinkData.Invalid;
        private string originalText;

        private LinkData highlightedLink
        {
            get => _highlightedLink;
            set
            {
                if (_highlightedLink.IsValid == value.IsValid && _highlightedLink.LinkID == value.LinkID)
                {
                    return;
                }

                if (_highlightedLink.IsValid)
                {
                    unhighlighted?.Invoke(this, _highlightedLink.LinkID);
                }

                _highlightedLink = value;
                ApplyStyles();

                if (_highlightedLink.IsValid)
                {
                    highlighted?.Invoke(this, _highlightedLink.LinkID);
                }
            }
        }

        private LinkData selectedLink
        {
            get => _selectedLink;
            set
            {
                if (_selectedLink.IsValid == value.IsValid && _selectedLink.LinkID == value.LinkID)
                {
                    return;
                }

                if (_selectedLink.IsValid)
                {
                    deselected?.Invoke(this, _selectedLink.LinkID);
                }

                _selectedLink = value;
                ApplyStyles();

                if (_selectedLink.IsValid)
                {
                    selected?.Invoke(this, _selectedLink.LinkID);
                }
            }
        }

        private struct LinkData
        {
            public static readonly LinkData Invalid = new() { LinkIndex = -1 };
            public bool IsValid => LinkIndex >= 0;
            public int LinkIndex;
            public string LinkID;
            public string LinkText;
            public Vector2 Center;
            public int LineNumber;
            public int Start;
            public int End;
            public int Length;
        }

        protected override void Awake()
        {
            base.Awake();
            label = GetComponent<TMP_Text>();

            if (logEvents)
            {
                clicked.AddListener((invoker, linkID) => Debug.Log($"Clicked: {linkID}"));
                highlighted.AddListener((invoker, linkID) => Debug.Log($"Highlighted: {linkID}"));
                unhighlighted.AddListener((invoker, linkID) => Debug.Log($"Unhighlighted: {linkID}"));
                selected.AddListener((invoker, linkID) => Debug.Log($"Selected: {linkID}"));
                deselected.AddListener((invoker, linkID) => Debug.Log($"Deselected: {linkID}"));
            }
        }

        protected override void Start()
        {
            base.Start();
            CaptureOriginalText();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveStyles();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveStyles();
        }

        public void CaptureOriginalText()
        {
            originalText = label.text;
            label.ForceMeshUpdate();
            BuildLinkData();

            if (!selectedLink.IsValid && allLinks.Count > 0)
            {
                selectedLink = allLinks[0];
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            DoStateTransition(SelectionState.Selected, false);
            selected?.Invoke(this, selectedLink.LinkID);
            ApplyStyles();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            DoStateTransition(SelectionState.Normal, false);
            deselected?.Invoke(this, selectedLink.LinkID);
            RemoveStyles();

            // Also clear unhighlighted link
            highlightedLink = LinkData.Invalid;
        }

        public override void OnMove(AxisEventData eventData)
        {
            bool moved = false;
            Vector2 move = eventData.moveVector;

            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                moved = MoveHorizontal(move.x > 0 ? 1 : -1);
            }
            else
            {
                moved = MoveVertical(move.y > 0 ? -1 : 1);
            }

            if (moved)
            {
                eventData.Use();
            }
            else
            {
                base.OnMove(eventData);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (selectedLink.IsValid)
            {
                DoStateTransition(SelectionState.Pressed, false);
                clicked?.Invoke(this, selectedLink.LinkID);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out LinkData link))
            {
                DoStateTransition(SelectionState.Pressed, false);
                selectedLink = link;
                clicked?.Invoke(this, selectedLink.LinkID);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out LinkData hit))
            {
                if (highlightedLink.IsValid && highlightedLink.LinkID == hit.LinkID)
                {
                    return;
                }

                highlightedLink = hit;
                DoStateTransition(SelectionState.Highlighted, false);
            }
            else if (highlightedLink.IsValid)
            {
                highlightedLink = LinkData.Invalid;
                
                if (EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    DoStateTransition(SelectionState.Selected, false);
                }
                else
                {
                    DoStateTransition(SelectionState.Normal, false);
                }
            }
        }

        private void BuildLinkData()
        {
            allLinks.Clear();

            TMP_TextInfo textInfo = label.textInfo;
            for (int i = 0; i < textInfo.linkCount; i++)
            {
                TMP_LinkInfo link = textInfo.linkInfo[i];
                int firstCharIndex = link.linkTextfirstCharacterIndex;
                int lastCharIndex = firstCharIndex + link.linkTextLength - 1;
                TMP_CharacterInfo firstChar = textInfo.characterInfo[firstCharIndex];
                TMP_CharacterInfo lastChar = textInfo.characterInfo[lastCharIndex];
                Vector2 position = (firstChar.bottomLeft + lastChar.topRight) / 2f;
                int lineNumber = firstChar.lineNumber;

                allLinks.Add(new LinkData
                {
                    LinkIndex = i,
                    LinkID = link.GetLinkID(),
                    LinkText = link.GetLinkText(),
                    Center = position,
                    LineNumber = lineNumber,
                    Start = firstChar.index,
                    End = lastChar.index,
                    Length = lastChar.index - firstChar.index + 1
                });

                // Sort A-Z but we will traverse backwards later
                allLinks.Sort((a, b) => a.Start.CompareTo(b.Start));
            }
        }

        private bool TryFindIntersectingLink(out LinkData result)
        {
            Vector2 mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(label, mousePosition, null);

            if (linkIndex == -1)
            {
                result = LinkData.Invalid;
                return false;
            }
            else
            {
                result = allLinks.Find(l => l.LinkIndex == linkIndex);
                return result.IsValid;
            }
        }

        private bool MoveHorizontal(int direction)
        {
            if (allLinks.Count == 0) return false;

            LinkData current = selectedLink;
            LinkData best = LinkData.Invalid;
            float bestDistance = float.MaxValue;

            foreach (LinkData link in allLinks)
            {
                if (link.LinkIndex == current.LinkIndex) continue;
                if (link.LineNumber != current.LineNumber) continue;

                float dx = link.Center.x - current.Center.x;

                if (direction > 0 && dx <= 0) continue;
                if (direction < 0 && dx >= 0) continue;

                float dist = Mathf.Abs(dx);

                if (dist < bestDistance)
                {
                    best = link;
                    bestDistance = dist;
                }
            }

            if (best.IsValid)
            {
                selectedLink = best;
                return true;
            }

            return false;
        }

        private bool MoveVertical(int direction)
        {
            if (allLinks.Count == 0) return false;

            LinkData current = selectedLink;
            LinkData best = LinkData.Invalid;
            float bestScore = float.MaxValue;

            foreach (LinkData link in allLinks)
            {
                if (link.LinkIndex == current.LinkIndex) continue;

                int dy = link.LineNumber - current.LineNumber;

                if (direction < 0 && dy >= 0) continue;
                if (direction > 0 && dy <= 0) continue;

                float yDist = Mathf.Abs(dy);
                float xDist = Mathf.Abs(link.Center.x - current.Center.x);
                float score = yDist * 1000f + xDist;

                if (score < bestScore)
                {
                    best = link;
                    bestScore = score;
                }
            }

            if (best.IsValid)
            {
                selectedLink = best;
                return true;
            }

            return false;
        }

        private void ApplyStyles()
        {
            StringBuilder sb = new StringBuilder(originalText);

            // allLinks is sorted A-Z, but we want to traverse backwards
            for (int i = allLinks.Count - 1; i >= 0; i--)
            {
                LinkData link = allLinks[i];

                string oldLinkText = link.LinkText;
                string newLinkText = oldLinkText;

                if (selectedLink.IsValid && link.LinkIndex == selectedLink.LinkIndex)
                {
                    newLinkText = string.Format(selectedStyle, oldLinkText);
                }
                else if (highlightedLink.IsValid && link.LinkIndex == highlightedLink.LinkIndex)
                {
                    newLinkText = string.Format(highlightedStyle, oldLinkText);
                }
                else
                {
                    continue;
                }

                sb.Remove(link.Start, link.Length);
                sb.Insert(link.Start, newLinkText);
            }

            SetText(sb.ToString());
        }

        private void RemoveStyles()
        {
            SetText(originalText);
        }

        private void SetText(string text)
        {
            label.text = text;
            label.ForceMeshUpdate(true, true);
        }
    }
}