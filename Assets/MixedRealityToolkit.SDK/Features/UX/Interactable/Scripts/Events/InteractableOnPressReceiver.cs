// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Basic press event receiver
    /// </summary>
    public class InteractableOnPressReceiver : ReceiverBase
    {
        /// <summary>
        /// Invoked on pointer release
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Release", Tooltip = "The button is released")]
        public UnityEvent OnRelease = new UnityEvent();

        /// <summary>
        /// Invoked on pointer press
        /// </summary>
        public UnityEvent OnPress => uEvent;

        /// <summary>
        /// Type of valid interaction distances to fire press events
        /// </summary>
        public enum InteractionType
        {
            /// <summary>
            /// Support Near and Far press interactions
            /// </summary>
            NearAndFar = 0,
            /// <summary>
            /// Support Near press interactions only
            /// </summary>
            NearOnly = 1,
            /// <summary>
            /// Support Far press interactions only
            /// </summary>
            FarOnly = 2
        }

        /// <summary>
        /// Specify whether press event is for near or far interaction
        /// </summary>
        [InspectorField(Label = "Interaction Filter", 
            Tooltip = "Specify whether press event is for near or far interaction", 
            Type = InspectorField.FieldTypes.DropdownInt, Options = new string[] { "Near and Far", "Near Only", "Far Only" })]
        public int InteractionFilter = (int)InteractionType.NearAndFar;

        private bool hasDown;

        private bool isNear = false;

        /// <summary>
        /// Receiver that raises press and release unity events
        /// </summary>
        public InteractableOnPressReceiver(UnityEvent ev) : base(ev, "OnPress") { }

        /// <summary>
        /// Receiver that raises press and release unity events
        /// </summary>
        public InteractableOnPressReceiver() : this(new UnityEvent()) { }

        /// <summary>
        /// checks if the received interactable state matches the press filter
        /// </summary>
        /// <returns>true if interactable state matches filter</returns>
        private bool IsFilterValid()
        {
            if (InteractionFilter == (int)InteractionType.FarOnly && isNear
                || InteractionFilter == (int)InteractionType.NearOnly && !isNear)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <inheritdoc />
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool hadDown = hasDown;
            hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;


            if (hasDown != hadDown)
            {
                if (hasDown)
                {
                    isNear = state.GetState(InteractableStates.InteractableStateEnum.PhysicalTouch).Value > 0;
                    if (IsFilterValid())
                    {
                        uEvent.Invoke();
                    }
                }
                else
                {
                    if (IsFilterValid())
                    {
                        OnRelease.Invoke();
                    }
                }
            }
        }
    }
}
