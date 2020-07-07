// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Component for handling Eye Focus on <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s.
    /// </summary>
    public abstract class BaseEyeFocusHandler : BaseFocusHandler
    {
        [Tooltip("Configurable duration to trigger an event if a user has been looking at the target for more than this duration.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToTriggerDwellInSec = 5;

        private DateTime dwellTimer;
        private bool isDwelling = false;
        private bool hadFocus = false;

        /// <summary>
        /// Handles highlighting targets when the cursor enters its hit box.
        /// </summary>
        protected virtual void Update()
        {
            if (!HasFocus && hadFocus)
            {
                OnEyeFocusStop();
                isDwelling = false;
                hadFocus = false;
            }
            else if (HasFocus)
            {
                if (!hadFocus)
                {
                    OnEyeFocusStart();
                    dwellTimer = DateTime.UtcNow;
                    hadFocus = true;
                }
                else
                {
                    OnEyeFocusStay();

                    if (!isDwelling && (DateTime.UtcNow - dwellTimer).TotalSeconds > timeToTriggerDwellInSec)
                    {
                        OnEyeFocusDwell();
                        isDwelling = true;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            // If we're the new target object,
            // add the pointer to the list of focusers.
            if (eventData.NewFocusedObject == gameObject && eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes)
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

        /// <summary>
        /// Triggered once the eye gaze ray starts intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStart() { }

        /// <summary>
        /// Triggered while the eye gaze ray is intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStay() { }

        /// <summary>
        /// Triggered once the eye gaze ray stops intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStop() { }

        /// <summary>
        /// Triggered once the eye gaze ray has intersected with this target's collider for a specified amount of time.
        /// </summary>
        protected virtual void OnEyeFocusDwell() { }
    }
}
