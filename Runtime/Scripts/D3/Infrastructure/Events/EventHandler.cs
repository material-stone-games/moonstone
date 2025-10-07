using System.Reflection;
using System.Threading.Tasks;

namespace Moonstone.D3.Infrastructure.Events
{
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : Event
    {
        protected readonly IEventDispatcher _eventDispatcher;

        public EventHandler(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        protected async Task Dispatch(Event @event) => await _eventDispatcher.Dispatch(@event);

        public abstract Task Handle(TEvent @event);
    }

    public class ReflectionEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : Event
    {
        private readonly object _target;
        private readonly MethodInfo _method;

        public ReflectionEventHandler(object target, MethodInfo method)
        {
            _target = target;
            _method = method;
        }

        public async Task Handle(TEvent domainEvent)
        {
            await (Task)_method.Invoke(_target, new object[] { domainEvent });
        }
    }
}