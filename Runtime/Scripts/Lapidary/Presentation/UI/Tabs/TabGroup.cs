using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Moonstone.Lapidary.Presentation.UI.Tabs
{
    [DisallowMultipleComponent]
    public class TabGroup : MonoBehaviour
    {
        [Header("Items")]
        [SerializeField] TabItem[] items = Array.Empty<TabItem>();

        [Header("Selection")]
        [SerializeField] int initialSelectedIndex = default;
        [SerializeField] bool disableSelectedButton = true;

        UnityAction[] selectActions = Array.Empty<UnityAction>();
        int selectedIndex = -1;

        public event Action<int, TabItem> SelectionChanged;

        public IReadOnlyList<TabItem> Items => items ?? Array.Empty<TabItem>();
        public int SelectedIndex => selectedIndex;
        public TabItem SelectedItem => IsValidIndex(selectedIndex) ? items[selectedIndex] : null;

        protected virtual void OnValidate()
        {
            items ??= Array.Empty<TabItem>();

            if (initialSelectedIndex < 0)
                initialSelectedIndex = 0;
        }

        protected virtual void Awake()
        {
            if (!ValidateItems())
                return;

            BindButtons();
            SetAllContentsActive(false);

            if (!Select(initialSelectedIndex))
                SelectFirstInteractable();
        }

        protected virtual void OnDestroy()
        {
            UnbindButtons();
        }

        public bool Select(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (items == null)
                return false;

            for (var i = 0; i < items.Length; i++)
            {
                if (string.Equals(items[i].Id, id, StringComparison.Ordinal))
                    return Select(i);
            }

            return false;
        }

        public bool Select(int index)
        {
            if (!IsValidIndex(index))
            {
                Debug.LogError($"Tab index is out of range: {index}", this);
                return false;
            }

            var item = items[index];
            if (!item.Interactable)
                return false;

            if (selectedIndex == index)
                return true;

            if (IsValidIndex(selectedIndex))
                items[selectedIndex].Content.SetActive(false);

            selectedIndex = index;
            item.Content.SetActive(true);
            RefreshButtonStates();
            SelectionChanged?.Invoke(selectedIndex, item);
            return true;
        }

        bool ValidateItems()
        {
            if (items == null || items.Length == 0)
            {
                Debug.LogError("Tab items are empty.", this);
                return false;
            }

            for (var i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    Debug.LogError($"Tab item is missing: {i}", this);
                    return false;
                }

                if (items[i].Button == null)
                {
                    Debug.LogError($"Tab button is missing: {i}", this);
                    return false;
                }

                if (items[i].Content == null)
                {
                    Debug.LogError($"Tab content is missing: {i}", this);
                    return false;
                }
            }

            return true;
        }

        bool SelectFirstInteractable()
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i].Interactable)
                    return Select(i);
            }

            Debug.LogError("Tab group has no interactable items.", this);
            return false;
        }

        void BindButtons()
        {
            selectActions = new UnityAction[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var index = i;
                selectActions[i] = () => Select(index);
                items[i].Button.onClick.AddListener(selectActions[i]);
            }
        }

        void UnbindButtons()
        {
            if (items == null || selectActions == null)
                return;

            var count = Math.Min(items.Length, selectActions.Length);
            for (var i = 0; i < count; i++)
            {
                if (items[i]?.Button != null && selectActions[i] != null)
                    items[i].Button.onClick.RemoveListener(selectActions[i]);
            }
        }

        void SetAllContentsActive(bool active)
        {
            foreach (var item in items)
                item.Content.SetActive(active);
        }

        void RefreshButtonStates()
        {
            for (var i = 0; i < items.Length; i++)
                items[i].Button.interactable = items[i].Interactable && (!disableSelectedButton || i != selectedIndex);
        }

        bool IsValidIndex(int index)
            => items != null && index >= 0 && index < items.Length;
    }
}
