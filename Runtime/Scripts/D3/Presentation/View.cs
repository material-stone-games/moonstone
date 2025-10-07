using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Moonstone.D3.Presentation
{
    public class BindAttribute : Attribute
    {
        public Type Type { get; }
        public BindAttribute(Type type) { Type = type; }
    }

    public abstract class View : MonoBehaviour, IView
    {
        private readonly Dictionary<Type, IView> _views = new();
        protected IEventDispatcher eventDispatcher;

        protected virtual void Awake()
        {
            eventDispatcher = Container.Instance?.EventDispatcher;
            BindViews();
        }

        protected void Dispatch(Event @event)
        {
            if (eventDispatcher == null)
                throw new InvalidOperationException("EventDispatcher is not set. Ensure that the View is properly initialized within a Container.");
            eventDispatcher.Dispatch(@event);
        }

        public void BindViews()
        {
            foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                if (Attribute.GetCustomAttribute(field, typeof(BindAttribute)) is BindAttribute bindAttr)
                    if (field.GetValue(this) is View view)
                        _views[bindAttr.Type] = view;
        }

        public T GetView<T>() where T : IView
        {
            var type = typeof(T);
            if (_views.TryGetValue(type, out var view))
                return (T)view;
            throw new KeyNotFoundException($"View of type {type} not found.");
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public virtual void Initialize()
        {
            foreach (var view in GetComponentsInChildren<View>(true))
                if (view != this && view.IsUnityValid())
                    view.Initialize();
        }

        public virtual void Dispose()
        {
            if (IsUnityNull()) return;
            foreach (var view in GetComponentsInChildren<View>(true))
                if (view != this && view.IsUnityValid())
                    view.Dispose();
        }

        private bool IsUnityNull() => this == null;

        private bool IsUnityValid() => !(this == null);
    }
}