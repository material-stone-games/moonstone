using System;

namespace Moonstone.D3.Domain
{
    public interface IDomainEvent
    {
        DateTime OccurredAt { get; }
    }
}
