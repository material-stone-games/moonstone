using System;

namespace Moonstone.D3
{
    /// <summary>
    /// 이벤트 베이스 클래스
    /// </summary>
    public abstract class Event
    {
        public DateTime OccurredAt { get; }

        public Event()
        {
            OccurredAt = DateTime.UtcNow;
        }
    }

    public class EventGroup : Event
    {
        public Event[] Events { get; }

        public EventGroup(params Event[] events)
        {
            Events = events ?? throw new ArgumentNullException(nameof(events));
        }
    }
}