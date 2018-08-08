// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// Base Component for handling Focus on <see cref="GameObject"/>s.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class BaseFocusHandler : MonoBehaviour, IMixedRealityFocusHandler
    {
        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        [SerializeField]
        [Tooltip("Is Focus capabilities enabled for this component?")]
        private bool focusEnabled = true;

        /// <inheritdoc />
        public virtual bool FocusEnabled
        {
            get { return focusEnabled; }
            set { focusEnabled = value; }
        }

        private bool hasFocus;

        /// <inheritdoc />
        public bool HasFocus => FocusEnabled && hasFocus;

        /// <inheritdoc />
        public List<IMixedRealityPointer> Focusers { get; } = new List<IMixedRealityPointer>(0);

        /// <inheritdoc />
        public virtual void OnFocusEnter(FocusEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnFocusExit(FocusEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            // If we're the new target object
            // Add the pointer to the list of focusers
            // and update our hasFocus flag if focusing is enabled.
            if (eventData.NewFocusedObject == gameObject)
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
            else if (eventData.OldFocusedObject == gameObject)
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

        /// <inheritdoc />
        public virtual void OnFocusChanged(FocusEventData eventData) { }
    }
}
