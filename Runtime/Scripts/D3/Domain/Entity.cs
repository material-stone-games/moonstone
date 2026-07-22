using System;

namespace Moonstone.D3.Domain
{
    public abstract class Entity : Entity<string>, IEntity
    {
        protected Entity() : this(Guid.NewGuid().ToString()) { }

        protected Entity(string id) : base(id ?? throw new ArgumentNullException(nameof(id))) { }
    }
}
