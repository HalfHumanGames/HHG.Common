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
        public ActionEvent<LinkSelectable, Link> Clicked => clicked;
        public ActionEvent<LinkSelectable, Link> Highlighted => highlighted;
        public ActionEvent<LinkSelectable, Link> Unhighlighted => unhighlighted;
        public ActionEvent<LinkSelectable, Link> Selected => selected;
        public ActionEvent<LinkSelectable, Link> Deselected => deselected;

        [SerializeField] private string highlightedStyle = "<color=yellow>{0}</color>";
        [SerializeField] private string selectedStyle = "<color=orange>{0}</color>";
        [SerializeField] private Options options;
        [SerializeField] private ActionEvent<LinkSelectable, Link> clicked = new ActionEvent<LinkSelectable, Link>();
        [SerializeField] private ActionEvent<LinkSelectable, Link> highlighted = new ActionEvent<LinkSelectable, Link>();
        [SerializeField] private ActionEvent<LinkSelectable, Link> unhighlighted = new ActionEvent<LinkSelectable, Link>();
        [SerializeField] private ActionEvent<LinkSelectable, Link> selected = new ActionEvent<LinkSelectable, Link>();
        [SerializeField] private ActionEvent<LinkSelectable, Link> deselected = new ActionEvent<LinkSelectable, Link>();
        [SerializeField] private bool logEvents;

        private TMP_Text label;
        private readonly List<Link> allLinks = new List<Link>();
        private Link _highlightedLink = Link.Invalid;
        private Link _selectedLink = Link.Invalid;
        private string originalText;

        private Link highlightedLink
        {
            get => _highlightedLink;
            set
            {
                if (_highlightedLink.IsValid == value.IsValid && _highlightedLink.Id == value.Id)
                {
                    return;
                }

                if (_highlightedLink.IsValid)
                {
                    unhighlighted?.Invoke(this, _highlightedLink);
                }

                _highlightedLink = value;
                ApplyStyles();

                if (_highlightedLink.IsValid)
                {
                    highlighted?.Invoke(this, _highlightedLink);
                }
            }
        }

        private Link selectedLink
        {
            get => _selectedLink;
            set
            {
                if (_selectedLink.IsValid == value.IsValid && _selectedLink.Id == value.Id)
                {
                    return;
                }

                if (_selectedLink.IsValid)
                {
                    deselected?.Invoke(this, _selectedLink);
                }

                _selectedLink = value;
                ApplyStyles();

                if (_selectedLink.IsValid)
                {
                    selected?.Invoke(this, _selectedLink);
                }
            }
        }

        [System.Flags]
        private enum Options
        {
            RememberSelection = 1 << 0,
            All = -1
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
                // Set field to bypass callbacks
                _selectedLink = allLinks[0];
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            DoStateTransition(SelectionState.Selected, false);
            if (!options.HasFlag(Options.RememberSelection)) ForgetSelection();
            selected?.Invoke(this, selectedLink);
            ApplyStyles();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            DoStateTransition(SelectionState.Normal, false);
            deselected?.Invoke(this, selectedLink);
            RemoveStyles();

            // Also clear unhighlighted link
            highlightedLink = Link.Invalid;
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
                clicked?.Invoke(this, selectedLink);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out Link link))
            {
                DoStateTransition(SelectionState.Pressed, false);
                selectedLink = link;
                clicked?.Invoke(this, selectedLink);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out Link hit))
            {
                if (highlightedLink.IsValid && highlightedLink.Id == hit.Id)
                {
                    return;
                }

                highlightedLink = hit;
                DoStateTransition(SelectionState.Highlighted, false);
            }
            else if (highlightedLink.IsValid)
            {
                highlightedLink = Link.Invalid;
                
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

            string text = label.text;
            TMP_TextInfo textInfo = label.textInfo;
            for (int i = 0; i < textInfo.linkCount; i++)
            {
                TMP_LinkInfo link = textInfo.linkInfo[i];
                int firstCharIndex = link.linkTextfirstCharacterIndex;
                int lastCharIndex = firstCharIndex + link.linkTextLength - 1;
                TMP_CharacterInfo firstChar = textInfo.characterInfo[firstCharIndex];
                TMP_CharacterInfo lastChar = textInfo.characterInfo[lastCharIndex];
                Vector3 position = firstChar.bottomLeft;
                Vector3 size = lastChar.topRight - position;
                int linkLength = lastChar.index - firstChar.index + 1;
                int lineNumber = firstChar.lineNumber;

                allLinks.Add(new Link
                {
                    Index = i,
                    Id = link.GetLinkID(),
                    Text = text.Substring(firstChar.index, linkLength),
                    Rect = new Rect(position, size),
                    LineNumber = lineNumber,
                    Start = firstChar.index,
                    End = lastChar.index,
                    Length = linkLength
                });

                // Sort A-Z but we will traverse backwards later
                allLinks.Sort((a, b) => a.Start.CompareTo(b.Start));
            }
        }

        private bool TryFindIntersectingLink(out Link result)
        {
            Vector2 mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(label, mousePosition, null);

            if (linkIndex == -1)
            {
                result = Link.Invalid;
                return false;
            }
            else
            {
                result = allLinks.Find(l => l.Index == linkIndex);
                return result.IsValid;
            }
        }

        private bool MoveHorizontal(int direction)
        {
            if (allLinks.Count == 0) return false;

            Link current = selectedLink;
            Link best = Link.Invalid;
            float bestDistance = float.MaxValue;

            foreach (Link link in allLinks)
            {
                if (link.Index == current.Index) continue;
                if (link.LineNumber != current.LineNumber) continue;

                float dx = link.Rect.center.x - current.Rect.center.x;

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

            Link current = selectedLink;
            Link best = Link.Invalid;
            float bestScore = float.MaxValue;

            foreach (Link link in allLinks)
            {
                if (link.Index == current.Index) continue;

                int dy = link.LineNumber - current.LineNumber;

                if (direction < 0 && dy >= 0) continue;
                if (direction > 0 && dy <= 0) continue;

                float yDist = Mathf.Abs(dy);
                float xDist = Mathf.Abs(link.Rect.center.x - current.Rect.center.x);
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
                Link link = allLinks[i];

                string oldLinkText = link.Text;
                string newLinkText = oldLinkText;

                if (selectedLink.IsValid && link.Index == selectedLink.Index)
                {
                    newLinkText = string.Format(selectedStyle, oldLinkText);
                }
                else if (highlightedLink.IsValid && link.Index == highlightedLink.Index)
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

        private void ForgetSelection()
        {
            if (allLinks.Count > 0)
            {
                _selectedLink = allLinks[0];
            }
        }
    }
}