using System;

namespace Moonstone.D3.Domain
{
    public sealed class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string message) : base(message) { }

        public EntityNotFoundException(Type entityType, object id)
            : base($"{entityType?.Name ?? "Entity"} with id '{id}' was not found.")
        {
        }
    }
}
