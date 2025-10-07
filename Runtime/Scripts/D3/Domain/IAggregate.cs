using System.Collections.Generic;

namespace Moonstone.D3.Domain
{
    public interface IAggregate : IEntity
    {
        void AddEvent(D3.Event @event);
        IReadOnlyList<D3.Event> GetUncommittedEvents();
        void ClearEvents();
    }
}