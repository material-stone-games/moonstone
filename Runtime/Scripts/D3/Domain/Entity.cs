using System;

namespace Moonstone.D3.Domain
{
    /// <summary>
    /// 도메인 엔티티 베이스 클래스
    /// </summary>
    public abstract class Entity : IEntity
    {
        public string Id { get; }

        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Entity(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}