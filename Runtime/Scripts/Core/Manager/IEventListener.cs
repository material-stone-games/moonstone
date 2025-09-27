using System;
using UnityEngine;

namespace Moonstone.Core.Manager
{
    public interface IEventListener
    {
        void OnEvent<TEnum>(TEnum type, object parameter, Component sender) where TEnum : Enum;
    }
}