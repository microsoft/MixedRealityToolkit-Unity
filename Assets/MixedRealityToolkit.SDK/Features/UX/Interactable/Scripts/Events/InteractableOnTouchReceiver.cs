// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic touch event receiver for detecting Physical Touch state changes in the Interactable
    /// When the physical touch states change, these events are triggered.
    /// </summary>
    public class InteractableOnTouchReceiver : ReceiverBase
    {
        /// <summary>
        /// Invoked when touch has left the object
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Touch End", Tooltip = "Touch has left the object")]
        public UnityEvent OnTouchEnd = new UnityEvent();

        /// <summary>
        /// Invoked when touch begins
        /// </summary>
        public UnityEvent OnTouchStart => uEvent;

        private bool hadTouch;

        /// <summary>
        /// Receiver for raising touch begin and end events
        /// </summary>
        public InteractableOnTouchReceiver(UnityEvent ev) : base(ev, "OnTouch") { }

        /// <summary>
        /// Receiver for raising touch begin and end events
        /// </summary>
        public InteractableOnTouchReceiver() : this(new UnityEvent()) { }

        /// <inheritdoc />
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool hadTouch = state.GetState(InteractableStates.InteractableStateEnum.PhysicalTouch).Value > 0;

            if (this.hadTouch != hadTouch)
            {
                if (hadTouch)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnTouchEnd.Invoke();
                }
            }

            this.hadTouch = hadTouch;
        }
    }
}
