// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic focus event receiver
    /// </summary>
    public class InteractableOnFocusReceiver : ReceiverBase
    {
        /// <summary>
        /// Creates receiver that raises focus enter and exit unity events
        /// </summary>
        public InteractableOnFocusReceiver() : this(new UnityEvent()) { }

        /// <summary>
        /// Creates receiver that raises focus enter and exit unity events
        /// </summary>
        public InteractableOnFocusReceiver(UnityEvent ev) : base(ev, "OnFocusOn") { }

        /// <summary>
        /// Raised when focus has left the object
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Focus Off", Tooltip = "Focus has left the object")]
        public UnityEvent OnFocusOff = new UnityEvent();

        /// <summary>
        /// Raised when focus has entered the object
        /// </summary>
        public UnityEvent OnFocusOn => uEvent;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool hasFocus = state.GetState(InteractableStates.InteractableStateEnum.Focus).Value > 0;

            if (hadFocus != hasFocus)
            {
                if (hasFocus)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnFocusOff.Invoke();
                }
            }

            hadFocus = hasFocus;
        }
    }
}
