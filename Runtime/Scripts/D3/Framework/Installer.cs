using System.Reflection;
using Moonstone.D3.Infrastructure;
using UnityEngine;

namespace Moonstone.D3.Framework
{
    public class Installer
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly Container _container;
        private readonly Bootstrap _bootstrap;

        public Installer(IEventDispatcher eventDispatcher, Container container, Bootstrap bootstrap)
        {
            _eventDispatcher = eventDispatcher;
            _container = container;
            _bootstrap = bootstrap;
        }

        public void Install(GameObject[] rootObjects)
        {
            foreach (var rootObject in rootObjects)
            {
                var views = rootObject.GetComponentsInChildren<Presentation.View>(true);
                foreach (var view in views)
                {
                    var field = typeof(Presentation.View).GetField("eventDispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
                    field?.SetValue(view, _eventDispatcher);
                }

                var components = rootObject.GetComponentsInChildren<Component>(true);
                foreach (var component in components)
                {
                    var componentType = component.GetType();
                    var fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var f in fields)
                    {
                        var injectAttribute = f.GetCustomAttribute<InjectAttribute>();
                        if (injectAttribute != null)
                        {
                            var dependency = _container.Resolve(f.FieldType);
                            if (dependency != null)
                            {
                                f.SetValue(component, dependency);
                            }
                        }
                    }
                }
            }

            var bootstrapFields = _bootstrap.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in bootstrapFields)
            {
                var fieldValue = field.GetValue(_bootstrap);
                if (fieldValue == null)
                {
                    foreach (var rootObject in rootObjects)
                    {
                        if (field.FieldType == typeof(GameObject))
                        {
                            field.SetValue(_bootstrap, rootObject);
                            break;
                        }
                        else if (typeof(Component).IsAssignableFrom(field.FieldType))
                        {
                            var component = rootObject.GetComponentInChildren(field.FieldType, true);
                            if (component != null)
                            {
                                field.SetValue(_bootstrap, component);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}