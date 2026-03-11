using System;
using System.Collections.Generic;

namespace AcceleracersCCG.Infrastructure
{
    /// <summary>
    /// Simple publish/subscribe event bus for decoupling game logic from UI.
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();
            _subscribers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public void Publish<T>(T gameEvent) where T : GameEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                // Copy to avoid modification during iteration
                var snapshot = new List<Delegate>(list);
                foreach (var handler in snapshot)
                {
                    ((Action<T>)handler)(gameEvent);
                }
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}
