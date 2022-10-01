// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Enables and disables sub-objects based on the currently
    /// toggled index of a <see cref="ToggleCollection"/>.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Tab View")]
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

        private void OnEnable()
        {
            toggleCollection.OnToggleSelected.AddListener(ChangeTab);
            
            // Ensure the correct view is visible when waking up.
            ChangeTab(CurrentVisibleSectionIndex);
        }

        private void OnDisable()
        {
            toggleCollection.OnToggleSelected.RemoveListener(ChangeTab);
        }

        private void ChangeTab(int index)
        {
            for (int i = 0; i < TabSections.Length; i++)
            {
                SetVisibility(i, i == index);
            }
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

        private void SetVisibility(int index, bool isVisible)
        {
            if (isVisible != TabSections[index].SectionVisibleRoot.activeSelf)
            {
                TabSections[index].SectionVisibleRoot.SetActive(isVisible);
            }
        }
    }
}
