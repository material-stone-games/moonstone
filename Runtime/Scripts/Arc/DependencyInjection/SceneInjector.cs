using System.Collections.Generic;
using UnityEngine;

namespace Moonstone.Arc.DependencyInjection
{
    public class SceneInjector
    {
        private readonly Bootstrapper _bootstrapper;
        private readonly IResolver _resolver;

        public SceneInjector(Bootstrapper bootstrapper, IResolver resolver)
        {
            _bootstrapper = bootstrapper;
            _resolver = resolver;
        }

        public void Inject(GameObject[] rootObjects)
        {
            var injected = new HashSet<Component>();

            InjectDependenciesIntoBootstrapper(injected);
            InjectDependenciesIntoSceneComponents(rootObjects, injected);
        }

        private void InjectDependenciesIntoBootstrapper(HashSet<Component> injected)
        {
            InjectComponent(_bootstrapper, injected);

            foreach (var component in _bootstrapper.GetComponentsInChildren<Component>(true))
                InjectComponent(component, injected);
        }

        private void InjectDependenciesIntoSceneComponents(GameObject[] rootObjects, HashSet<Component> injected)
        {
            foreach (var rootObject in rootObjects)
            {
                if (rootObject == null) continue;

                foreach (var component in rootObject.GetComponentsInChildren<Component>(true))
                    InjectComponent(component, injected);
            }
        }

        private void InjectComponent(Component component, HashSet<Component> injected)
        {
            if (component == null || !injected.Add(component)) return;

            DependencyInjector.Inject(component, _resolver);
        }
    }
}
