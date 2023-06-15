// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// ToggleCollection groups a list of StatefulInteractables and correlates their
    /// toggle states. When any one of the StatefulInteractables are toggled, all other
    /// interactables controlled by this script will be de-toggled.
    /// A custom list of interactables can be set; if none is set at edit-time, all direct
    /// StatefulInteractable children will be added.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Toggle Collection")]
    public class ToggleCollection : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Array of StatefulInteractable toggles that will be managed by this controller.")]
        private List<StatefulInteractable> toggles = new List<StatefulInteractable>();

        /// <summary>
        /// List of StatefulInteractable toggles that will be managed by this controller.
        /// </summary>
        public List<StatefulInteractable> Toggles
        {
            get => toggles;
            set
            {
                if (value != null && toggles != value)
                {
                    if (toggles != null)
                    {
                        // Destroy all listeners on previous toggleList
                        RemoveSelectionListeners();
                    }

                    // Set new list
                    toggles = value;

                    // Add listeners to new list
                    AddSelectionListeners();

                    int index = Mathf.Clamp(CurrentIndex, 0, toggles.Count - 1);
                    SetSelection(index);
                }
            }
        }

        [SerializeField]
        [Tooltip("Should the user be allowed to un-toggle the toggles?")]
        private bool allowSwitchOff = false;

        /// <summary>
        /// Get or set if the user is allowed to un-toggle the toggle buttons.
        /// </summary>
        public bool AllowSwitchOff
        {
            get => allowSwitchOff;
            set
            {
                allowSwitchOff = value;
                foreach (var toggle in Toggles)
                {
                    if (toggle == null) { continue; }
                    toggle.ToggleMode = value ? StatefulInteractable.ToggleType.Toggle : StatefulInteractable.ToggleType.OneWayToggle;
                }
            }
        }

        [SerializeField]
        [Tooltip("Currently selected index in the ToggleCollection, default is 0")]
        private int currentIndex;

        /// <summary>
        /// The current index in the array of stateful toggles
        /// </summary>
        public int CurrentIndex
        {
            get => currentIndex;
            set => SetSelection(value);
        }

        [Tooltip("This event is triggered when any of the toggles in the ToggleCollection are selected. The event data is the index of the toggle button with in the ToggleCollection.")]
        [SerializeField]
        private ToggleSelectedEvent onToggleSelected = new ToggleSelectedEvent();

        /// <summary>
        /// This event is triggered when any of the toggles in the <see cref="ToggleCollection"/> are selected. The event data is the index of the toggle button within the <see cref="ToggleCollection"/>.
        /// </summary>
        public ToggleSelectedEvent OnToggleSelected
        {
            get => onToggleSelected;
        }

        // List of the actions for the toggles in ToggleCollection.
        private List<UnityAction<float>> toggleActions = new List<UnityAction<float>>();

        private void OnValidate()
        {
            // Refresh this when modified, so that the desired changes are propagated
            // to all the managed interactables.
            AllowSwitchOff = allowSwitchOff;
        }

        private void Start()
        {
            // If we don't already have any toggles listed, we scan for toggles
            // in our direct children.
            if (toggles == null || toggles.Count == 0)
            {
                // Make sure our toggles are not null.
                toggles ??= new List<StatefulInteractable>();

                // Find some toggles!
                foreach (Transform child in transform)
                {
                    var interactable = child.GetComponent<StatefulInteractable>();

                    // If the interactable is some kind of toggle...
                    if (interactable != null && interactable.ToggleMode != StatefulInteractable.ToggleType.Button)
                    {
                        toggles.Add(interactable);
                    }
                }
            }
            if (Toggles != null && toggleActions.Count == 0)
            {
                // Add listeners to each toggle in ToggleCollection.
                AddSelectionListeners();

                // Force set initial selection in the toggle collection at start
                if (CurrentIndex >= 0 && CurrentIndex < Toggles.Count)
                {
                    SetSelection(CurrentIndex, true);
                    Toggles[CurrentIndex].ForceSetToggled(true);
                }
            }

            // Initialize the interactables with the proper allow-toggle-off setting.
            AllowSwitchOff = allowSwitchOff;
        }

        /// <summary>
        /// Set the selection of a an element in the toggle collection based on index.
        /// <param name="index">Index of an element in the <see cref="ToggleCollection"/>.</param>
        /// <param name="force">Force selection to be set.</param>
        /// </summary>
        public void SetSelection(int index, bool force = false)
        {
            if (index < 0 || Toggles.Count <= index || Toggles == null || !isActiveAndEnabled)
            {
                Debug.LogWarning("Index out of range of ToggleCollection: " + index);
                return;
            }

            if (CurrentIndex != index || force)
            {
                currentIndex = index;

                OnSelection(index);
            }
        }

        protected virtual void OnSelection(int index)
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i] == null) { continue; }

                if (index != i)
                {
                    Toggles[i].ForceSetToggled(false);
                }
                else
                {
                    Toggles[i].ForceSetToggled(true);
                }
            }

            OnToggleSelected?.Invoke(index);
        }

        private void AddSelectionListeners()
        {
            // Add listeners to new list
            for (int i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i] == null) { continue; }
                
                int itemIndex = i;
                UnityAction<float> setSelectionAction = (_) => SetSelection(itemIndex);

                toggleActions.Add(setSelectionAction);

                Toggles[i].IsToggled.OnEntered.AddListener(setSelectionAction);
                Toggles[i].ToggleMode = allowSwitchOff ? StatefulInteractable.ToggleType.Toggle : StatefulInteractable.ToggleType.OneWayToggle;
            }
        }

        private void RemoveSelectionListeners()
        {
            for (int i = 0; i < toggleActions.Count; i++)
            {
                if (Toggles[i] == null) { continue; }
                
                Toggles[i].IsToggled.OnEntered.RemoveListener(toggleActions[i]);
            }

            toggleActions.Clear();
        }

        private void OnDestroy()
        {
            RemoveSelectionListeners();
        }
    }
}
