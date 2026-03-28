using System;
using System.Collections.Generic;

namespace Events
{
    public static class EventBus
    {
        private static Dictionary<Type, Delegate> _eventTable = new();

        public static void Subscribe<T>(Action<T> listener) where T : IEvent
        {
            var type = typeof(T);
            
            if (_eventTable.TryGetValue(type, out Delegate existing))
                _eventTable[type] = Delegate.Combine(existing, listener);
            else
                _eventTable.Add(type, listener);
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : IEvent
        {
            var type = typeof(T);
            if (_eventTable.TryGetValue(type, out Delegate existing))
            {
                var currentDel = Delegate.Remove(existing, listener);
                
                if (currentDel == null)
                    _eventTable.Remove(type);
                else
                    _eventTable[type] = currentDel;
            }
        }

        public static void Publish<T>(T eventData) where T : IEvent
        {
            var type = typeof(T);
            
            if (_eventTable.TryGetValue(type, out Delegate del))
                ((Action<T>)del)?.Invoke(eventData);
        }

        public static void Clear()
        {
            _eventTable.Clear();
        }
    }
}