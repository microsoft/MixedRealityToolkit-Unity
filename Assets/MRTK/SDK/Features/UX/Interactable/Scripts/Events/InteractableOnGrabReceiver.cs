// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic grab event receiver for detecting grab events (OnGrab, OnRelease) or grab state changes from Interactable
    /// When the grab states change, on or off, these events are triggered
    /// </summary>
    public class InteractableOnGrabReceiver : ReceiverBase
    {
        /// <summary>
        /// Invoked on grab release
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Release", Tooltip = "Grab was released")]
        public UnityEvent OnRelease = new UnityEvent();

        /// <summary>
        /// Invoked on grab start
        /// </summary>
        public UnityEvent OnGrab => uEvent;

        private bool hadGrab;

        /// <summary>
        /// Creates a receiver that raises grab start and end events.
        /// </summary>
        public InteractableOnGrabReceiver(UnityEvent ev) : base(ev, "OnGrab") { }

        /// <summary>
        /// Creates a receiver that raises grab start and end events.
        /// </summary>
        public InteractableOnGrabReceiver() : this(new UnityEvent()) { }

        /// <inheritdoc />
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool hasGrab = state.GetState(InteractableStates.InteractableStateEnum.Grab).Value > 0;

            if (hadGrab != hasGrab)
            {
                if (hasGrab)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnRelease.Invoke();
                }
            }

            hadGrab = hasGrab;
        }
    }
}
