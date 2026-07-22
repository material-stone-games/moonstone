using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moonstone.D3.Domain
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public bool Equals(ValueObject other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return ComponentsEqual(GetComponents(), other.GetComponents());
        }

        public override bool Equals(object obj) => Equals(obj as ValueObject);

        public override int GetHashCode()
        {
            unchecked
            {
                return GetComponents()
                    .Select(GetComponentHashCode)
                    .Aggregate(17, (hash, componentHash) => hash * 31 + componentHash);
            }
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);

        private IEnumerable<object> GetComponents()
        {
            return GetEqualityComponents() ?? Enumerable.Empty<object>();
        }

        private static bool ComponentsEqual(object left, object right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            if (left is string || right is string)
                return left.Equals(right);

            if (left is IEnumerable leftItems && right is IEnumerable rightItems)
                return EnumerablesEqual(leftItems, rightItems);

            return left.Equals(right);
        }

        private static bool EnumerablesEqual(IEnumerable left, IEnumerable right)
        {
            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();

            while (true)
            {
                var hasLeft = leftEnumerator.MoveNext();
                var hasRight = rightEnumerator.MoveNext();

                if (hasLeft != hasRight) return false;
                if (!hasLeft) return true;
                if (!ComponentsEqual(leftEnumerator.Current, rightEnumerator.Current)) return false;
            }
        }

        private static int GetComponentHashCode(object component)
        {
            if (component == null) return 0;
            if (component is string) return component.GetHashCode();

            if (component is IEnumerable items)
            {
                unchecked
                {
                    var hash = 17;
                    foreach (var item in items)
                        hash = hash * 31 + GetComponentHashCode(item);
                    return hash;
                }
            }

            return component.GetHashCode();
        }
    }
}
