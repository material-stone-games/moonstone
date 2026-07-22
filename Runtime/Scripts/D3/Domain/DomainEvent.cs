using System;

namespace Moonstone.D3.Domain
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredAt { get; }

        protected DomainEvent() : this(DateTime.UtcNow) { }

        protected DomainEvent(DateTime occurredAt)
        {
            OccurredAt = occurredAt;
        }
    }
}
