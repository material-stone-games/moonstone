using System;

namespace Moonstone.D3.Domain
{
    public abstract class Aggregate : AggregateRoot<string>, IAggregate
    {
        protected Aggregate() : this(Guid.NewGuid().ToString()) { }

        protected Aggregate(string id) : base(id ?? throw new ArgumentNullException(nameof(id))) { }
    }
}
