using UnityEngine;
using UnityEngine.UI;

namespace Moonstone.View.UI
{
    public class ButtonContentGroup
    {
        public Button Button;
        public GameObject Content;
    }

    public class Tab : View
    {
        [SerializeField] Transform buttonContainerTransform;
        [SerializeField] Transform contentContainerTransform;
        [SerializeField] int initialSelectedIndex;

        ButtonContentGroup[] buttonContentGroups;
        ButtonContentGroup selectedGroup;

        void Awake()
        {
            if (buttonContainerTransform.childCount != contentContainerTransform.childCount)
            {
                Debug.LogError("Button and content counts do not match!");
                return;
            }

            buttonContentGroups = new ButtonContentGroup[buttonContainerTransform.childCount];
            for (int i = 0; i < buttonContentGroups.Length; i++)
            {
                var buttonContentGroup = new ButtonContentGroup
                {
                    Button = buttonContainerTransform.GetChild(i).GetComponent<Button>(),
                    Content = contentContainerTransform.GetChild(i).gameObject
                };
                buttonContentGroup.Button.onClick.AddListener(() => SelectTab(buttonContentGroup));
                buttonContentGroups[i] = buttonContentGroup;
            }

            Clear();
            SelectTab(buttonContentGroups[initialSelectedIndex]);
        }

        void Clear()
        {
            foreach (var group in buttonContentGroups)
            {
                group.Content.SetActive(false);
            }
        }

        private void SelectTab(ButtonContentGroup selectedGroup)
        {
            if (this.selectedGroup != null) { this.selectedGroup.Content.SetActive(false); }
            this.selectedGroup = selectedGroup;
            if (this.selectedGroup != null) { this.selectedGroup.Content.SetActive(true); }
        }
    }
}