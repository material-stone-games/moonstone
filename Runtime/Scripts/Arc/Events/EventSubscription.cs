using System;

namespace Moonstone.Arc.Events
{
    public sealed class EventSubscription : IDisposable
    {
        private Action _unsubscribe;

        public EventSubscription(Action unsubscribe) => _unsubscribe = unsubscribe;

        public void Dispose()
        {
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }
}
