using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonstone.Arc
{
    public class EventDispatcher
    {
        private static EventDispatcher _instance;
        private static EventDispatcher Instance => _instance ??= new EventDispatcher();

        private readonly Dictionary<Type, object> _eventTables = new();

        private Dictionary<TEnum, List<IEventHandler>> GetEventTable<TEnum>() where TEnum : Enum
        {
            Type eventType = typeof(TEnum);
            if (!_eventTables.ContainsKey(eventType))
                _eventTables[eventType] = new Dictionary<TEnum, List<IEventHandler>>();

            return (Dictionary<TEnum, List<IEventHandler>>)_eventTables[eventType];
        }

        private async Task DispatchInternal<TEnum>(TEnum eventType, params object[] arguments) where TEnum : Enum
        {
            var eventTable = GetEventTable<TEnum>();
            if (eventTable.TryGetValue(eventType, out var handlers))
                foreach (var handler in handlers)
                    await handler.Handle(eventType, arguments);
        }

        public static async Task Dispatch<TEnum>(TEnum eventType, params object[] arguments) where TEnum : Enum
            => await Instance.DispatchInternal(eventType, arguments);

        public static EventRegistration Register(IEventHandler handler) => new(handler, Instance._eventTables);

        public static void Dispose()
        {
            Instance._eventTables.Clear();
            _instance = null;
        }
    }

    public class EventRegistration
    {
        private readonly IEventHandler _handler;
        private readonly Dictionary<Type, object> _eventTables;

        public EventRegistration(IEventHandler handler, Dictionary<Type, object> eventTables)
        {
            _handler = handler;
            _eventTables = eventTables;
        }

        public void For<TEnum>(params TEnum[] eventTypes) where TEnum : Enum
        {
            foreach (var eventType in eventTypes)
            {
                Type type = eventType.GetType();

                if (!_eventTables.ContainsKey(type))
                    _eventTables[type] = new Dictionary<TEnum, List<IEventHandler>>();

                var eventTable = (Dictionary<TEnum, List<IEventHandler>>)_eventTables[type];

                if (!eventTable.ContainsKey(eventType))
                    eventTable[eventType] = new List<IEventHandler>();

                eventTable[eventType].Add(_handler);
            }
        }
    }
}