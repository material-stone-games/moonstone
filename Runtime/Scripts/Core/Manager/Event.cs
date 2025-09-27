using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moonstone.Core.Manager
{
    public class Event : ManagerBase
    {
        readonly Dictionary<Type, object> eventTables = new();

        Dictionary<TEnum, List<IEventListener>> GetTable<TEnum>() where TEnum : Enum
        {
            var type = typeof(TEnum);
            if (!eventTables.ContainsKey(type))
                eventTables[type] = new();
            return (Dictionary<TEnum, List<IEventListener>>)eventTables[type];
        }

        public void AddListener<TEnum>(TEnum type, IEventListener listener) where TEnum : Enum
        {
            var table = GetTable<TEnum>();
            if (!table.ContainsKey(type))
                table[type] = new();
            table[type].Add(listener);
        }

        public void RemoveListener<TEnum>(TEnum type, IEventListener listener) where TEnum : Enum
        {
            var table = GetTable<TEnum>();
            if (!table.ContainsKey(type))
                return;
            table[type].Remove(listener);
        }

        public void Notify<TEnum>(TEnum type, object parameter, Component sender) where TEnum : Enum
        {
            var table = GetTable<TEnum>();
            if (table.ContainsKey(type))
                foreach (var listener in table[type])
                    listener.OnEvent(type, parameter, sender);
        }
    }
}