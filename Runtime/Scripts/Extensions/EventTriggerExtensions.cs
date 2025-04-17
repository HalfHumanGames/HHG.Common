using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public static class EventTriggerExtensions
    {
        public static EventTrigger.Entry AddTrigger(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction callback)
        {
            return eventTrigger.AddTrigger(triggerType, _ => callback());
        }

        public static EventTrigger.Entry AddTrigger(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = eventTrigger.triggers.FirstOrDefault(t => t.eventID == triggerType);

            if (entry != null)
            {
                entry.callback.AddListener(callback);
            }
            else
            {
                entry = new EventTrigger.Entry();
                entry.eventID = triggerType;
                entry.callback.AddListener(callback);
                eventTrigger.triggers.Add(entry);
            }

            return entry;
        }

        public static void RemoveTrigger(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
        {
            foreach (EventTrigger.Entry entry in eventTrigger.triggers)
            {
                if (entry.eventID == triggerType)
                {
                    entry.callback.RemoveListener(callback);
                }
            }
        }
    }
}