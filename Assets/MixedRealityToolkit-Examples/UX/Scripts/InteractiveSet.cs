// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.InteractiveElements
{
    public enum SelectionType
    {
        single,
        multiple
    }

    /// <summary>
    /// A controller for managing multiple radial or tab type buttons
    /// </summary>
    public class InteractiveSet : MonoBehaviour
    {
        [Tooltip("Allow single or multiple selection")]
        public SelectionType Type = SelectionType.single;

        [Tooltip("Interactives that will be managed by this controller")]
        public List<InteractiveToggle> Interactives;

        [Tooltip("Currently selected indices or default starting indices")]
        public List<int> SelectedIndices = new List<int>() { 0 };

        [Tooltip("exposed selection changed event")]
        public UnityEvent OnSelectionEvents;

        // list of calls to remove events on recycled buttons
        private Dictionary<InteractiveToggle, UnityAction> Calls = new Dictionary<InteractiveToggle, UnityAction>();

        private bool mHasInit = false;

        private void Start()
        {
            UpdateInteractives();
        }

        public void UpdateInteractives()
        {
            foreach (InteractiveToggle toggle in Calls.Keys)
            {
                toggle.OnSelectEvents.RemoveListener(Calls[toggle]);
            }
            Calls.Clear();
            for (int i = 0; i < Interactives.Count; ++i)
            {
                int itemIndex = i;
                // add selection event handler to each button
                UnityAction call = () => HandleOnSelection(itemIndex);
                Calls.Add(Interactives[i], call);
                Interactives[i].OnSelectEvents.AddListener(call);
                if (Type == SelectionType.single)
                {
                    Interactives[i].AllowDeselect = false;
                }
                Interactives[i].HasSelection = SelectedIndices.Contains(i);
            }
            OnSelectionEvents.Invoke();
        }

        public void RemoveInteractive(int itemIndex)
        {
            Interactives[itemIndex].OnSelectEvents.RemoveListener(() => HandleOnSelection(itemIndex));
            Interactives.RemoveAt(itemIndex);
        }

        /// <summary>
        /// Sets the selected index and selected Interactive
        /// </summary>
        /// <param name="index"></param>
        public void SetSelection(int index)
        {
            if (!isActiveAndEnabled ||
                (index < 0 || Interactives.Count <= index))
            {
                return;
            }

            Interactives[index].OnInputClicked(null);
        }

        /// <summary>
        /// responds to selection events
        /// </summary>
        /// <param name="index"></param>
        private void HandleOnSelection(int index)
        {
            if (Type == SelectionType.single)
            {
                for (int i = 0; i < Interactives.Count; ++i)
                {
                    if (i != index)
                    {
                        Interactives[i].HasSelection = false;
                    }
                }

                if (!mHasInit)
                {
                    Interactives[index].HasSelection = true;
                    mHasInit = true;
                }

                SelectedIndices.Clear();
                SelectedIndices.Add(index);
            }
            else
            {
                Interactives[index].HasSelection = !Interactives[index].HasSelection;
                if (SelectedIndices.Contains(index))
                {
                    SelectedIndices.Remove(index);
                }
                else
                {
                    SelectedIndices.Add(index);
                }
            }
            OnSelectionEvents.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = Interactives.Count - 1; i >= 0; i--)
            {
                RemoveInteractive(i);
            }
        }
    }
}
