using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.EventManager
{
    public class EventData : UnityEvent<object> { }
    public static class EventManager
    {
        private static Dictionary<eEventType, EventData> _eventDatabase = new Dictionary<eEventType, EventData>();

        public static void Subscribe(eEventType eventType, UnityAction<object> action)
        {
            EventData eventData = null;
            if (_eventDatabase.TryGetValue(eventType, out eventData))
            {
                eventData.AddListener(action);
            }
            else
            {
                eventData = new EventData();
                eventData.AddListener(action);
                _eventDatabase.Add(eventType, eventData);
            }
        }

        public static void Unsubscribe(eEventType eventType, UnityAction<object> action)
        {
            if (_eventDatabase.TryGetValue(eventType, out EventData eventData))
            {
                eventData.RemoveListener(action);
            }
        }

        public static void InvokeEvent(eEventType eventType, object arg = null)
        {
            if (_eventDatabase.TryGetValue(eventType, out EventData eventData))
            {
                eventData.Invoke(arg);
            }
        }
    }
}
