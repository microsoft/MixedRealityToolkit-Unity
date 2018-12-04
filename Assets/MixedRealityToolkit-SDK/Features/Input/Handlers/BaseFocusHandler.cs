// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
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
        [SerializeField]
        [Tooltip("Is focus enabled for this component?")]
        private bool focusEnabled = true;

        /// <summary>
        /// Is focus enabled for this <see cref="Component"/>?
        /// </summary>
        public virtual bool FocusEnabled
        {
            get { return focusEnabled; }
            set { focusEnabled = value; }
        }

        /// <summary>
        /// Does this object currently have focus by any <see cref="IMixedRealityPointer"/>?
        /// </summary>
        public virtual bool HasFocus => FocusEnabled && Focusers.Count > 0;

        /// <summary>
        /// The list of <see cref="IMixedRealityPointer"/>s that are currently focused on this <see cref="GameObject"/>
        /// </summary>
        public List<IMixedRealityPointer> Focusers { get; } = new List<IMixedRealityPointer>(0);

        /// <inheritdoc />
        public virtual void OnFocusEnter(FocusEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnFocusExit(FocusEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            // If we're the new target object,
            // add the pointer to the list of focusers.
            if (eventData.NewFocusedObject == gameObject)
            {
                eventData.Pointer.FocusTarget = this;
                Focusers.Add(eventData.Pointer);
            }
            // If we're the old focused target object,
            // remove the pointer from our list.
            else if (eventData.OldFocusedObject == gameObject)
            {
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
