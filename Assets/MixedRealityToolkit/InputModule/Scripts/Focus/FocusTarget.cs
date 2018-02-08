// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.Pointers;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Focus
{
    /// <summary>
    /// Base Component for handling Focus on GameObjects.
    /// </summary>
    public class FocusTarget : MonoBehaviour, IFocusHandler
    {
        [SerializeField]
        [Tooltip("Does this GameObject start with Focus Enabled?")]
        private bool focusEnabled = true;

        public virtual bool FocusEnabled
        {
            get { return focusEnabled; }
            set { focusEnabled = value; }
        }

        private bool hasFocus;

        public bool HasFocus { get { return FocusEnabled && hasFocus; } }

        private readonly List<IPointer> focusers = new List<IPointer>(0);

        public List<IPointer> Focusers { get { return focusers; } }

        public virtual void OnFocusEnter(FocusEventData eventData) { }

        public virtual void OnFocusExit(FocusEventData eventData) { }

        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            // If we're the new target object
            // Add the pointer to the list of focusers
            // and update our hasFocus flag if focusing is enabled.
            if (eventData.NewFocusedObject == this)
            {
                eventData.Pointer.FocusTarget = this;
                Focusers.Add(eventData.Pointer);

                if (focusEnabled)
                {
                    hasFocus = true;
                }
            }
            // If we're the old focused target object
            // update our flag and remove the pointer from our list.
            else if (eventData.OldFocusedObject == this)
            {
                hasFocus = false;

                Focusers.Remove(eventData.Pointer);

                // If there is no new focused target
                // clear the FocusTarget field from the Pointer.
                if (eventData.NewFocusedObject == null)
                {
                    eventData.Pointer.FocusTarget = null;
                }
            }
        }

        public virtual void OnFocusChanged(FocusEventData eventData) { }
    }
}
