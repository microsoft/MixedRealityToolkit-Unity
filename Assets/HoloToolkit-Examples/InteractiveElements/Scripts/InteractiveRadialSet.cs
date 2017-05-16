// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// A controller for managing multiple radial or tab type buttons
    /// </summary>
    public class InteractiveRadialSet : MonoBehaviour
    {
        [Tooltip("Interactives that will be managed by this controller")]
        public InteractiveToggle[] Interactives;

        [Tooltip("Currently selected index or default starting index")]
        public int SelectedIndex = 0;

        [Tooltip("exposed selection changed event")]
        public UnityEvent OnSelectionEvents;

        private bool mHasInit = false;

        private void Start()
        {
            for (int i = 0; i < Interactives.Length; ++i)
            {
                int itemIndex = i;
                // add selection event handler to each button
                Interactives[i].OnSelectEvents.AddListener(() => HandleOnSelection(itemIndex));
                Interactives[i].AllowDeselect = false;
            }

            HandleOnSelection(SelectedIndex);
        }

        /// <summary>
        /// Sets the selected index and selected Interactive
        /// </summary>
        /// <param name="index"></param>
        public void SetSelection(int index)
        {
            if (!isActiveAndEnabled ||
                (index < 0 || Interactives.Length <= index))
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
            for (int i = 0; i < Interactives.Length; ++i)
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

            SelectedIndex = index;

            OnSelectionEvents.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < Interactives.Length; ++i)
            {
                int itemIndex = i;
                Interactives[i].OnSelectEvents.RemoveListener(() => HandleOnSelection(itemIndex));
            }
        }
    }
}
