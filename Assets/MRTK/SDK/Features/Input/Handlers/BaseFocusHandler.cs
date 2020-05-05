﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Component for handling Focus on <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class BaseFocusHandler : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityFocusChangedHandler
    {
        [SerializeField]
        [Tooltip("Is focus enabled for this component?")]
        private bool focusEnabled = true;

        /// <summary>
        /// Is focus enabled for this <see href="https://docs.unity3d.com/ScriptReference/Component.html">Component</see>?
        /// </summary>
        public virtual bool FocusEnabled
        {
            get { return focusEnabled; }
            set { focusEnabled = value; }
        }

        /// <summary>
        /// Does this object currently have focus by any <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>?
        /// </summary>
        public virtual bool HasFocus => FocusEnabled && Focusers.Count > 0;

        [SerializeField]
        [Tooltip("Is gaze enabled for this component?")]
        private bool gazeEnabled = false;

        /// <summary>
        /// Is gaze enabled for this <see href="https://docs.unity3d.com/ScriptReference/Component.html">Component</see>?
        /// </summary>
        public virtual bool GazeEnabled
        {
            get { return gazeEnabled; }
            set { gazeEnabled = value; }
        }

        /// <summary>
        /// Does this object currently have gaze by any <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>?
        /// </summary>
        public virtual bool HasGaze => GazeEnabled && CoreServices.InputSystem?.GazeProvider.GazeTarget == gameObject;

        /// <summary>
        /// The list of <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>s that are currently focused on this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
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
