// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Collection of toggles that control the visibility of associated game objects.
    /// </summary>
    public class TabView : MonoBehaviour
    {
        [Tooltip("Toggle collection for this Tab View.")]
        [SerializeField]
        private ToggleCollection toggleCollection;

        /// <summary>
        /// Toggle collection for this Tab View.
        /// </summary>
        public ToggleCollection ToggleCollection
        {
            get => toggleCollection;
            set => toggleCollection = value;
        }

        [SerializeField]
        [Tooltip("Tab sections array, one tab section maps to one item in the toggle collection. ")]
        private TabSection[] tabSections;

        /// <summary>
        /// Tab sections array, one tab section maps to one item in the toggle collection. 
        /// </summary>
        public TabSection[] TabSections
        {
            get => tabSections;
            set => tabSections = value;
        }

        /// <summary>
        /// The index of the current active tab/toggle collection item
        /// </summary>
        public int CurrentVisibleSectionIndex
        {
            get => toggleCollection.CurrentIndex;
            set => toggleCollection.CurrentIndex = value;
        }

        private void Start()
        {
            toggleCollection.OnToggleSelected.AddListener((index) =>
            {
                SetIsVisible(index, true);

                for (int i = 0; i < TabSections.Length; i++)
                {
                    if (i != index)
                    {
                        SetIsVisible(i, false);
                    }
                }
            });
        }

        /// <summary>
        /// Set a tab active based on the tab section's label name.
        /// </summary>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>True if the section name was set to active</returns>
        public bool ForceSetTabActiveByLabel(string sectionName)
        {
            for (int i = 0; i < TabSections.Length; i++)
            {
                if (TabSections[i].SectionName == sectionName)
                {
                    ToggleCollection.CurrentIndex = i;
                    return true;
                }
            }

            Debug.LogError($"The section name {sectionName} entered does not exist in the TabSections array.");
            return false;
        }

        /// <summary>
        /// Check if a section is visible based on the name. 
        /// </summary>
        /// <param name="sectionName">The name of section</param>
        /// <returns>True if the section is visible</returns>
        public bool IsSectionVisible(string sectionName)
        {
            for (int i = 0; i < TabSections.Length; i++)
            {
                if (TabSections[i].SectionName == sectionName && i == CurrentVisibleSectionIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Set the next tab active in the toggle collection.
        /// </summary>
        public void IncreaseIndex()
        {
            int nextIndex = CurrentVisibleSectionIndex + 1;

            if (nextIndex < TabSections.Length)
            {
                CurrentVisibleSectionIndex = nextIndex;
            }
        }

        /// <summary>
        /// Set the previous tab active in the toggle collection.
        /// </summary>
        public void DecreaseIndex()
        {
            int nextIndex = CurrentVisibleSectionIndex - 1;

            if (nextIndex >= 0)
            {
                CurrentVisibleSectionIndex = nextIndex;
            }
        }

        private void SetIsVisible(int index, bool isVisible)
        {
            if (isVisible != TabSections[index].SectionVisibleRoot.activeSelf)
            {
                TabSections[index].SectionVisibleRoot.SetActive(isVisible);
            }
        }
    }
}
