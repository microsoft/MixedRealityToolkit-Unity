// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to control a list of radial type buttons or tabs
    /// </summary>
    public class InteractableToggleCollection : MonoBehaviour
    {
        [Tooltip("Interactables that will be managed by this controller")]
        public Interactable[] ToggleList;

        [Tooltip("Currently selected index or default starting index")]
        public int CurrentIndex;

        [Tooltip("exposed selection changed event")]
        public UnityEvent OnSelectionEvents;

        private void OnEnable()
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;
                // add selection event handler to each button
                ToggleList[i].OnClick.AddListener(() => OnSelection(itemIndex));
                ToggleList[i].CanDeselect = false;
            }
            
            OnSelection(CurrentIndex, true);
        }

        /// <summary>
        /// Sets the selected index and selected Interactive
        /// </summary>
        /// <param name="index"></param>
        public void SetSelection(int index)
        {
            if (!isActiveAndEnabled ||
                (index < 0 || ToggleList.Length <= index))
            {
                return;
            }

            ToggleList[index].OnPointerClicked(null);
        }

        /// <summary>
        /// Set the toggle state of each button based on the selected item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="force"></param>
        protected virtual void OnSelection(int index, bool force = false)
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                if (i != index)
                {
                    ToggleList[i].SetDimensionIndex(0);
                }
            }

            CurrentIndex = index;

            if (force)
            {
                ToggleList[index].SetDimensionIndex(1);
            }
            else
            {
                OnSelectionEvents.Invoke();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;
                ToggleList[i].OnClick.RemoveListener(() => OnSelection(itemIndex));
            }
        }
    }
}
