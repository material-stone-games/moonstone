using System.Collections.Generic;
using Moonstone.D3.Domain;

namespace Moonstone.Tests.Runtime
{
    sealed class TestValueObject : ValueObject
    {
        private readonly string _key;
        private readonly int[] _values;

        public TestValueObject(string key, int[] values)
        {
            _key = key;
            _values = values;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _key;
            yield return _values;
        }
    }
}
