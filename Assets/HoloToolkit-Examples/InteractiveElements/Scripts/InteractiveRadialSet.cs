// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.InteractiveElements
{

    public class InteractiveRadialSet : MonoBehaviour
    {

        public InteractiveToggle[] Interactives;
        public int SelectedIndex = 0;

        public UnityEvent OnSelectionEvents;

        private bool mHasInit = false;

        private void Awake()
        {
            for (int i = 0; i < Interactives.Length; ++i)
            {
                int itemIndex = i;
                Interactives[i].OnSelectEvents.AddListener(() => HandleOnSelection(itemIndex));
                Interactives[i].AllowDeselect = false;
            }

            HandleOnSelection(SelectedIndex);
        }

        public void SetSelection(int index)
        {
            if (!isActiveAndEnabled ||
                (index < 0 || Interactives.Length <= index))
            {
                return;
            }

            Interactives[index].OnInputClicked(null);
        }

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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
