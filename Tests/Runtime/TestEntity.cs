using Moonstone.D3.Domain;

namespace Moonstone.Tests.Runtime
{
    sealed class TestEntity : Entity<string>
    {
        public TestEntity(string id) : base(id) { }
    }
}
