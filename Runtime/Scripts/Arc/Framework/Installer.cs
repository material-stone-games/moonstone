using System.Reflection;
using UnityEngine;

namespace Moonstone.Arc.Framework
{
    public class Installer
    {
        private readonly Bootstrapper _bootstrapper;

        public Installer(Bootstrapper bootstrapper) => _bootstrapper = bootstrapper;

        public void Install(GameObject[] rootObjects)
        {
            InjectDependenciesIntoSceneComponents(rootObjects);
            InjectComponentsIntoBootstrapperFields(rootObjects);
        }

        /// <summary>
        /// 씬에 있는 컴포넌트들의 [Inject] 속성이 붙은 필드에 의존성을 주입합니다.
        /// </summary>
        private void InjectDependenciesIntoSceneComponents(GameObject[] rootObjects)
        {
            foreach (var rootObject in rootObjects)
            {
                var components = rootObject.GetComponentsInChildren<Component>(true);
                foreach (var component in components)
                {
                    var componentType = component.GetType();
                    var fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var f in fields)
                    {
                        var injectAttribute = f.GetCustomAttribute<InjectAttribute>();
                        if (injectAttribute == null) continue;

                        var dependency = Container.Resolve(f.FieldType);
                        if (dependency == null) continue;

                        f.SetValue(component, dependency);
                    }
                }
            }
        }

        /// <summary>
        /// Bootstrapper의 필드에 씬에 있는 컴포넌트들을 주입합니다.
        /// </summary>
        private void InjectComponentsIntoBootstrapperFields(GameObject[] rootObjects)
        {
            var bootstrapFields = _bootstrapper.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in bootstrapFields)
            {
                var fieldValue = field.GetValue(_bootstrapper);
                if (fieldValue != null) continue;

                foreach (var rootObject in rootObjects)
                {
                    if (field.FieldType == typeof(GameObject))
                    {
                        field.SetValue(_bootstrapper, rootObject);
                        break;
                    }
                    else if (typeof(Component).IsAssignableFrom(field.FieldType))
                    {
                        var component = rootObject.GetComponentInChildren(field.FieldType, true);
                        if (component == null) continue;

                        field.SetValue(_bootstrapper, component);
                        break;
                    }
                }
            }
        }
    }
}