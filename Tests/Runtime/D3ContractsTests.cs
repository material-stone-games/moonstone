using System;
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
    }
}
