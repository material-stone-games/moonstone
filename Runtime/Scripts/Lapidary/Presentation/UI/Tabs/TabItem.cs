using System;
using UnityEngine;
using UnityEngine.UI;

namespace Moonstone.Lapidary.Presentation.UI.Tabs
{
    [Serializable]
    public sealed class TabItem
    {
        [SerializeField] string id = default;
        [SerializeField] Button button = default;
        [SerializeField] GameObject content = default;
        [SerializeField] bool interactable = true;

        public string Id => id;
        public Button Button => button;
        public GameObject Content => content;
        public bool Interactable => interactable;
    }
}
