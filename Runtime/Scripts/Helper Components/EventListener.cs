using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class EventListener :
        MonoBehaviour,
        IBeginDragHandler,
        ICancelHandler,
        IDeselectHandler,
        IDragHandler,
        IDropHandler,
        IEndDragHandler,
        IInitializePotentialDragHandler,
        IMoveHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerMoveHandler,
        IPointerUpHandler,
        IScrollHandler,
        ISelectHandler,
        ISubmitHandler,
        IUpdateSelectedHandler
    {
        [Flags]
        private enum Events
        {
            Awake = 1 << 0,
            Start = 1 << 1,
            Enable = 1 << 2,
            //Disable = 1 << 3, // Cannot start coroutines on disable, for reuse
            //Destroy = 1 << 4, // Cannot start coroutines on destroy, for reuse
            BeginDrag = 1 << 5,
            Cancel = 1 << 6,
            Deselect = 1 << 7,
            Drag = 1 << 8,
            Drop = 1 << 9,
            EndDrag = 1 << 10,
            InitializePotentialDrag = 1 << 11,
            Move = 1 << 12,
            PointerClick = 1 << 13,
            PointerDown = 1 << 14,
            PointerEnter = 1 << 15,
            PointerExit = 1 << 16,
            PointerMove = 1 << 17,
            PointerUp = 1 << 18,
            Scroll = 1 << 19,
            Select = 1 << 20,
            Submit = 1 << 21,
            UpdateSelected = 1 << 22,
            All = -1
        }

        private bool listenToAwake => events.HasFlag(Events.Awake);
        private bool listenToStart => events.HasFlag(Events.Start);
        private bool listenToEnable => events.HasFlag(Events.Enable);
        private bool listenToBeginDrag => events.HasFlag(Events.BeginDrag);
        private bool listenToCancel => events.HasFlag(Events.Cancel);
        private bool listenToDeselect => events.HasFlag(Events.Deselect);
        private bool listenToDrag => events.HasFlag(Events.Drag);
        private bool listenToDrop => events.HasFlag(Events.Drop);
        private bool listenToEndDrag => events.HasFlag(Events.EndDrag);
        private bool listenToInitializePotentialDrag => events.HasFlag(Events.InitializePotentialDrag);
        private bool listenToMove => events.HasFlag(Events.Move);
        private bool listenToPointerClick => events.HasFlag(Events.PointerClick);
        private bool listenToPointerDown => events.HasFlag(Events.PointerDown);
        private bool listenToPointerEnter => events.HasFlag(Events.PointerEnter);
        private bool listenToPointerExit => events.HasFlag(Events.PointerExit);
        private bool listenToPointerMove => events.HasFlag(Events.PointerMove);
        private bool listenToPointerUp => events.HasFlag(Events.PointerUp);
        private bool listenToScroll => events.HasFlag(Events.Scroll);
        private bool listenToSelect => events.HasFlag(Events.Select);
        private bool listenToSubmit => events.HasFlag(Events.Submit);
        private bool listenToUpdateSelected => events.HasFlag(Events.UpdateSelected);

        [SerializeField] private Events events = Events.All;

        // MonoBehaviour Events
        [SerializeField, Unfold, ShowIf(nameof(listenToAwake), true)] private ActionEvent onAwake = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToStart), true)] private ActionEvent onStart = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToEnable), true)] private ActionEvent onEnable = new ActionEvent();

        // UI Events
        [SerializeField, Unfold, ShowIf(nameof(listenToBeginDrag), true)] private ActionEvent onBeginDrag = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToCancel), true)] private ActionEvent onCancel = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToDeselect), true)] private ActionEvent onDeselect = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToDrag), true)] private ActionEvent onDrag = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToDrop), true)] private ActionEvent onDrop = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToEndDrag), true)] private ActionEvent onEndDrag = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToInitializePotentialDrag), true)] private ActionEvent onInitializePotentialDrag = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToMove), true)] private ActionEvent onMove = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerClick), true)] private ActionEvent onPointerClick = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerDown), true)] private ActionEvent onPointerDown = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerEnter), true)] private ActionEvent onPointerEnter = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerExit), true)] private ActionEvent onPointerExit = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerMove), true)] private ActionEvent onPointerMove = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToPointerUp), true)] private ActionEvent onPointerUp = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToScroll), true)] private ActionEvent onScroll = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToSelect), true)] private ActionEvent onSelect = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToSubmit), true)] private ActionEvent onSubmit = new ActionEvent();
        [SerializeField, Unfold, ShowIf(nameof(listenToUpdateSelected), true)] private ActionEvent onUpdateSelected = new ActionEvent();

        private void Awake()
        {
            if (listenToAwake) onAwake?.Invoke(this);
        }

        private void Start()
        {
            if (listenToStart) onStart?.Invoke(this);
        }

        private void OnEnable()
        {
            if (listenToEnable) onEnable?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (listenToBeginDrag) onBeginDrag?.Invoke(this);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (listenToCancel) onCancel?.Invoke(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (listenToDeselect) onDeselect?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (listenToDrag) onDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (listenToDrop) onDrop?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (listenToEndDrag) onEndDrag?.Invoke(this);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (listenToInitializePotentialDrag) onInitializePotentialDrag?.Invoke(this);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (listenToMove) onMove?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (listenToPointerClick) onPointerClick?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (listenToPointerDown) onPointerDown?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (listenToPointerEnter) onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (listenToPointerExit) onPointerExit?.Invoke(this);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (listenToPointerMove) onPointerMove?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (listenToPointerUp) onPointerUp?.Invoke(this);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (listenToScroll) onScroll?.Invoke(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (listenToSelect) onSelect?.Invoke(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (listenToSubmit) onSubmit?.Invoke(this);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (listenToUpdateSelected) onUpdateSelected?.Invoke(this);
        }
    }
}