using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Moonstone.D3.Infrastructure.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<IEventHandler>> _handlers = new();

        /// <summary>
        /// 비동기 이벤트 핸들러 등록
        /// </summary>
        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : Event
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<IEventHandler>();
            _handlers[eventType].Add(handler);
        }

        public void RegisterHandler<TEvent>(Application.IApplicationService service) where TEvent : Event
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<IEventHandler>();
            var methods = service.GetType().GetMethods();
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(Application.HandlerAttribute), false);
                foreach (var attribute in attributes)
                {
                    if (attribute is not Application.HandlerAttribute handlerAttribute) continue;
                    if (handlerAttribute.CommandType != eventType) continue;
                    var handler = new ReflectionEventHandler<TEvent>(service, method);
                    _handlers[eventType].Add(handler);
                }
            }
        }

        /// <summary>
        /// 비동기 이벤트 발행
        /// </summary>
        public async Task Dispatch(Event domainEvent)
        {
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));
            if (domainEvent is EventGroup eventGroup)
            {
                await Task.WhenAll(eventGroup.Events.Select(@event => Dispatch(@event)));
            }
            else
            {
                var eventType = domainEvent.GetType();
                if (!_handlers.ContainsKey(eventType)) return;

                var handlers = _handlers[eventType];
                foreach (var handler in handlers)
                    await handler.Handle(domainEvent);
            }
        }

        /// <summary>
        /// 모든 핸들러 제거
        /// </summary>
        public void ClearHandlers()
        {
            _handlers.Clear();
        }
    }
}