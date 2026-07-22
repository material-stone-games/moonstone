using System;
using System.Collections.Generic;

namespace Moonstone.D3.Domain
{
    public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    {
        public TId Id { get; }

        protected Entity(TId id)
        {
            if (EqualityComparer<TId>.Default.Equals(id, default))
                throw new ArgumentException("Entity id cannot be the default value.", nameof(id));

            if (id is string text && string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Entity id cannot be empty.", nameof(id));

            Id = id;
        }

        public bool Equals(Entity<TId> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj) => Equals(obj as Entity<TId>);

        public override int GetHashCode()
        {
            unchecked
            {
                return (GetType().GetHashCode() * 397) ^ EqualityComparer<TId>.Default.GetHashCode(Id);
            }
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);
    }
}
