using System;
using System.Collections.Generic;

namespace Moonstone.D3.Domain
{
    /// <summary>
    /// 도메인 애그리거트 루트 베이스 클래스
    /// </summary>
    public abstract class Aggregate : Entity, IAggregate
    {
        private readonly List<D3.Event> _events = new();

        public Aggregate() : base() { }
        public Aggregate(string id) : base(id) { }

        public void AddEvent(D3.Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            _events.Add(@event);
        }

        public void AddEvent(params D3.Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            _events.Add(new EventGroup(events));
        }

        public IReadOnlyList<D3.Event> GetUncommittedEvents() => new List<D3.Event>(_events).AsReadOnly();
        public void ClearEvents() => _events.Clear();
    }
}