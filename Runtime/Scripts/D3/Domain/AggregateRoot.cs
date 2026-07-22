using System;
using System.Collections.Generic;

namespace Moonstone.D3.Domain
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
    {
        private readonly List<IDomainEvent> _events = new();

        protected AggregateRoot(TId id) : base(id) { }

        protected void AddEvent(IDomainEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _events.Add(@event);
        }

        protected void AddEvents(params IDomainEvent[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                AddEvent(@event);
        }

        public IReadOnlyList<IDomainEvent> GetUncommittedEvents() => new List<IDomainEvent>(_events).AsReadOnly();

        public void ClearEvents() => _events.Clear();
    }
}
