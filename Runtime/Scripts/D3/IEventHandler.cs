using System;
using System.Threading.Tasks;

namespace Moonstone.D3
{
    public interface IEventHandler
    {
        Task Handle(Event @event);
    }

    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : Event
    {
        Task Handle(TEvent @event);

        async Task IEventHandler.Handle(Event @event)
        {
            if (@event is TEvent typedEvent)
                await Handle(typedEvent);
            else
                throw new ArgumentException($"Event is not of type {typeof(TEvent).Name}");
        }
    }
}