using Moonstone.D3.Domain;

namespace Moonstone.Tests.Runtime
{
    sealed class TestAggregate : Aggregate
    {
        public TestAggregate(string id) : base(id) { }

        public void Raise(IDomainEvent domainEvent)
        {
            AddEvent(domainEvent);
        }
    }
}
