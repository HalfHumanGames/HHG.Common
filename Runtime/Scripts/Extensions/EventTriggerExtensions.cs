using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public static class EventTriggerExtensions
    {
        public static EventTrigger.Entry AddTrigger(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener(_ => callback());
            eventTrigger.triggers.Add(entry);
            return entry;
        }

        public static EventTrigger.Entry AddTrigger(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(entry);
            return entry;
        }
    }
}