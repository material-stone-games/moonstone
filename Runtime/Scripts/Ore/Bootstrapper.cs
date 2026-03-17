using System.Collections;
using UnityEngine;

namespace Moonstone.Ore
{
    public abstract class Bootstrapper : Local.Entity
    {
        protected override void OnInitialize()
        {
            StartCoroutine(InitializeRoutine());
        }

        private IEnumerator InitializeRoutine()
        {
            CreateObjects();
            InitializeObjects();
            BindObjects();
            SetUpObjects();
            yield break;
        }

        protected abstract void CreateObjects();

        protected abstract void BindObjects();

        protected abstract void InitializeObjects();

        protected abstract void SetUpObjects();
    }
}