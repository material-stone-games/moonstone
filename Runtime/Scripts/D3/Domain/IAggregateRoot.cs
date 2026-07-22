using System.Collections.Generic;

namespace Moonstone.D3.Domain
{
    public interface IAggregateRoot<out TId> : IEntity<TId>
    {
        IReadOnlyList<IDomainEvent> GetUncommittedEvents();
        void ClearEvents();
    }
}
