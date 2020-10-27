// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to control a list of radial type buttons or tabs
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/InteractableToggleCollection")]
    public class InteractableToggleCollection : MonoBehaviour
    {
        [Tooltip("Array of Interactables that will be managed by this controller")]
        [SerializeField, FormerlySerializedAs("ToggleList")]
        private Interactable[] toggleList;

        /// <summary>
        /// Array of Interactables that will be managed by this controller
        /// </summary>
        public Interactable[] ToggleList
        {
            get => toggleList;
            set
            {
                if (value != null && toggleList != value)
                {
                    if (toggleList != null)
                    {
                        // Destroy all listeners on previous toggleList
                        RemoveSelectionListeners();
                    }

                    // Set new list
                    toggleList = value;

                    // Add listeners to new list
                    AddSelectionListeners();

                    int index = Mathf.Clamp(CurrentIndex, 0, toggleList.Length - 1);
                    SetSelection(index, true, true);
                }
            }
        }

        [Tooltip("Currently selected index in the ToggleList, default is 0")]
        [SerializeField]
        private int currentIndex;

        /// <summary>
        /// The current index in the array of interactable toggles
        /// </summary>
        public int CurrentIndex
        {
            get => currentIndex;
            set => SetSelection(value, true, true);
        }

        [Tooltip("This event is triggered when any of the toggles in the ToggleList are selected")]
        /// <summary>
        /// This event is triggered when any of the toggles in the ToggleList are selected
        /// </summary>
        public UnityEvent OnSelectionEvents = new UnityEvent();

        private List<UnityAction> toggleActions = new List<UnityAction>();

        private void Start()
        {
            if (ToggleList != null)
            {
                // If the ToggleList is set before start, then it already has listeners
                // If the ToggleList is populated through the inspector, then it needs listeners
                if (toggleActions.Count == 0)
                {
                    // Add listeners to each toggle in ToggleList
                    AddSelectionListeners();

                    SetSelection(CurrentIndex, true, true);
                }
            }
        }

        /// <summary>
        /// Set the selection of a an element in the toggle collection based on index.
        /// <param name="index">Index of an element in ToggleList</param>
        /// <param name="force">Force selection set</param>
        /// <param name="fireOnClick">The manual trigger of the OnClick event. OnClick event is manually triggered 
        /// when the CurrentIndex is updated via script or inspector</param>
        /// </summary>
        public void SetSelection(int index, bool force = false, bool fireOnClick = false)
        {
            if (index < 0 || ToggleList.Length <= index || ToggleList == null || !isActiveAndEnabled)
            {
                Debug.LogWarning("Index out of range of ToggleList: " + index);
                return;
            }

            if (CurrentIndex != index || force)
            {
                currentIndex = index;

                OnSelection(index);

                if (fireOnClick)
                {
                    // Trigger the OnClick event without checking CanInteract as we did not check when setting the index earlier
                    ToggleList[index].TriggerOnClick(true);
                }
            }
        }

        // Update the visual appearance and set the states of the selected and unselected toggles within 
        // Interactable
        protected virtual void OnSelection(int index, bool force = false)
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                ToggleList[i].IsToggled = (i == index);
            }

            OnSelectionEvents?.Invoke();
        }

        private void AddSelectionListeners()
        {
            // Add listeners to new list
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;

                UnityAction setSelectionAction = () =>
                {
                    SetSelection(itemIndex, true, false);
                };

                toggleActions.Add(setSelectionAction);

                ToggleList[i].OnClick.AddListener(setSelectionAction);
                ToggleList[i].CanDeselect = false;
            }
        }

        private void RemoveSelectionListeners()
        {
            for (int i = 0; i < toggleActions.Count; ++i)
            {
                Interactable toggle = ToggleList[i];
                if (toggle != null)
                {
                    toggle.OnClick.RemoveListener(toggleActions[i]);
                }
            }

            toggleActions.Clear();
        }

        private void OnDestroy()
        {
            RemoveSelectionListeners();
        }
    }
}
