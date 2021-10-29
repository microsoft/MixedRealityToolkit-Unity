﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Basic hold event receiver
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/InteractableOnHoldReceiver")]
    public class InteractableOnHoldReceiver : ReceiverBase
    {
        /// <summary>
        /// The amount of time to press before triggering event
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Float, Label = "Hold Time", Tooltip = "The amount of time to press before triggering event")]
        public float HoldTime = 1f;

        /// <summary>
        /// Should this event suppress the OnClick Event? Only relevant for touch interaction.
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Bool, Label = "Suppress Click Event", Tooltip = "Should this event suppress the OnClick Event? Only relevant for touch interaction.")]
        public bool SuppressClick = false;

        private float clickTimer = 0;

        private bool hasDown;

        /// <summary>
        /// Invoked when interactable has been pressed for HoldTime
        /// </summary>
        public UnityEvent OnHold => uEvent;

        /// <summary>
        /// Creates receiver that raises OnHold events
        /// </summary>
        public InteractableOnHoldReceiver(UnityEvent ev) : base(ev, "OnHold") { }

        /// <summary>
        /// Creates receiver that raises OnHold events
        /// </summary>
        public InteractableOnHoldReceiver() : this(new UnityEvent()) { }

        /// <inheritdoc />
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            if (state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0 && !hasDown)
            {
                hasDown = true;
                clickTimer = 0;
            }
            else if (state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value < 1)
            {
                hasDown = false;
            }

            if (hasDown && clickTimer < HoldTime)
            {
                clickTimer += Time.deltaTime;

                if (clickTimer >= HoldTime)
                {
                    uEvent.Invoke();
                }
            }
        }
    }
}
