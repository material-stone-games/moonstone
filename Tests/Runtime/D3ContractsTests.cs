using System;
using System.Collections.Generic;
using Moonstone.D3.Application;
using Moonstone.D3.Domain;
using NUnit.Framework;

namespace Moonstone.Tests.Runtime
{
    public sealed class D3ContractsTests
    {
        [Test]
        public void EntityRejectsDefaultAndEmptyIds()
        {
            Assert.Throws<ArgumentException>(() => new TestEntity(null));
            Assert.Throws<ArgumentException>(() => new TestEntity(""));
            Assert.Throws<ArgumentException>(() => new TestEntity(" "));
        }

        [Test]
        public void EntityEqualityUsesRuntimeTypeAndId()
        {
            Assert.That(new TestEntity("id-1"), Is.EqualTo(new TestEntity("id-1")));
            Assert.That(new TestEntity("id-1"), Is.Not.EqualTo(new OtherTestEntity("id-1")));
            Assert.That(new TestEntity("id-1"), Is.Not.EqualTo(new TestEntity("id-2")));
        }

        [Test]
        public void ValueObjectComparesNestedCollectionsByValue()
        {
            var left = new TestValueObject("key", new[] { 1, 2, 3 });
            var right = new TestValueObject("key", new[] { 1, 2, 3 });
            var different = new TestValueObject("key", new[] { 1, 3, 2 });

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
            Assert.That(left, Is.Not.EqualTo(different));
        }

        [Test]
        public void AggregateTracksAndClearsDomainEvents()
        {
            var aggregate = new TestAggregate("aggregate-1");
            var domainEvent = new TestDomainEvent();

            aggregate.Raise(domainEvent);

            Assert.That(aggregate.GetUncommittedEvents(), Is.EqualTo(new[] { domainEvent }));

            aggregate.ClearEvents();

            Assert.That(aggregate.GetUncommittedEvents(), Is.Empty);
        }

        [Test]
        public void ResultCarriesErrorsAndErrorCode()
        {
            var result = Result.Failure(new[] { "first", "second" }, "validation");

            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.ErrorCode, Is.EqualTo("validation"));
            Assert.That(result.ErrorMessage, Is.EqualTo("first"));
            Assert.That(result.Errors, Is.EqualTo(new[] { "first", "second" }));
        }

        private sealed class TestEntity : Entity<string>
        {
            public TestEntity(string id) : base(id) { }
        }

        private sealed class OtherTestEntity : Entity<string>
        {
            public OtherTestEntity(string id) : base(id) { }
        }

        private sealed class TestAggregate : Aggregate
        {
            public TestAggregate(string id) : base(id) { }

            public void Raise(IDomainEvent domainEvent)
            {
                AddEvent(domainEvent);
            }
        }

        private sealed class TestDomainEvent : DomainEvent { }

        private sealed class TestValueObject : ValueObject
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
}
