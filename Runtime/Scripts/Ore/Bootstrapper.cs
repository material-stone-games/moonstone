using System.Collections;
using UnityEngine;

namespace Moonstone.Ore
{
    public abstract class Bootstrapper : Local.Entity
    {
        protected override void OnInitialize()
        {
            BindObjects();
            InitializeObjects();
        }

        private void Start()
        {
            CreateObjects();
            PrepareObjects();
        }

        protected abstract void BindObjects();

        protected abstract void InitializeObjects();

        protected abstract void CreateObjects();

        protected abstract void PrepareObjects();
    }
}