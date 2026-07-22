using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonstone.Arc.Events
{
    public sealed class EventBus : IEventBus, Moonstone.ILifecycleDisposable, IDisposable
    {
        private readonly Dictionary<Type, List<Func<object, Task>>> _typedHandlers = new();
        private bool _isDisposed;

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            return Subscribe<TEvent>(@event =>
            {
                handler(@event);
                return Task.CompletedTask;
            });
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            ThrowIfDisposed();

            var eventType = typeof(TEvent);
            var handlers = GetHandlerTable(eventType);

            Func<object, Task> wrappedHandler = @event => handler((TEvent)@event);

            handlers.Add(wrappedHandler);
            return new EventSubscription(() => handlers.Remove(wrappedHandler));
        }

        public async Task Publish<TEvent>(TEvent @event)
        {
            ThrowIfDisposed();

            var eventType = typeof(TEvent);
            if (!_typedHandlers.TryGetValue(eventType, out var handlers)) return;

            foreach (var handler in handlers.ToArray())
                await handler(@event);
        }

        private List<Func<object, Task>> GetHandlerTable(Type eventType)
        {
            if (!_typedHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Func<object, Task>>();
                _typedHandlers[eventType] = handlers;
            }

            return handlers;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _typedHandlers.Clear();
            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventBus));
        }
    }

}
