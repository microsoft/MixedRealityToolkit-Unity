// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to control a list of radial type buttons or tabs
    /// </summary>
    public class InteractableToggleCollection : MonoBehaviour
    {
        [Tooltip("Array of Interactables that will be managed by this controller")]
        public Interactable[] ToggleList;


        [Tooltip("Currently selected index in the ToggleList, default is 0")]
        [SerializeField]
        private int currentIndex;

        /// <summary>
        /// The current index in the array of interactable toggles
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                SetSelection(value, false, true);
            }
        }

        [Tooltip("This event is triggered when any of the toggles in the ToggleList are selected")]
        public UnityEvent OnSelectionEvents = new UnityEvent();

        private void Start()
        {
            if (ToggleList != null)
            {
                // Add listeners to each toggle in ToggleList
                for (int i = 0; i < ToggleList.Length; ++i)
                {
                    int itemIndex = i;
                    ToggleList[i].OnClick.AddListener(() => SetSelection(itemIndex, true, false));
                    ToggleList[i].CanDeselect = false;
                }

                SetSelection(CurrentIndex, true, true);
            }
        }

        /// <summary>
        /// Set the selection of a an element in the toggle collection based on index.
        /// <param name="index">Index of an element in ToggleList</param>
        /// <param name="force">Force update currentIndex</param>
        /// <param name="fireOnClick">The manual trigger of the OnClick event. OnClick event is manually triggered 
        /// when the CurrentIndex is updated via script or inspector</param>
        /// </summary>
        public void SetSelection(int index, bool force = false, bool fireOnClick = false)
        {
            if (index < 0 || ToggleList.Length <= index)
            {
                Debug.LogWarning("Index out of range of ToggleList: " + index);
                return;
            }

            if (CurrentIndex != index || force)
            {
                currentIndex = index;

                UpdateToggleVisual(index);
            }

            if (fireOnClick)
            {
                ToggleList[index].TriggerOnClick();
            }
        }

        // Update the visual appearance and set the states of the selected and unselected toggles within 
        // Interactable
        protected virtual void UpdateToggleVisual(int index)
        {
            int length = ToggleList.Length;
            for (int i = 0; i < length; ++i)
            {
                if (i != index)
                {
                    ToggleList[i].IsToggled = false;
                }
                else
                {
                    ToggleList[i].IsToggled = true;
                }
            }

            OnSelectionEvents.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;
                ToggleList[i].OnClick.RemoveListener(() => SetSelection(itemIndex, true, false));
            }
        }
    }
}
