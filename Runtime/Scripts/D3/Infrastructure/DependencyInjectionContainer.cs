using UnityEngine;

namespace Moonstone.D3.Infrastructure
{
    public abstract class DependencyInjectionContainer : MonoBehaviour
    {
        [SerializeField] protected Canvas _canvas;
        [SerializeField] private bool _dontDestroyOnLoad;

        void Awake()
        {
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        public abstract void BindObjects();
        public abstract void InitializeObjects();
        public abstract void CreateObjects();
        public abstract void Clear();
    }
}