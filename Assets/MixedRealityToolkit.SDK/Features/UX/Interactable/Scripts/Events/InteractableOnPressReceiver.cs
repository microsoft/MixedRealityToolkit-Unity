// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Basic press event receiver
    /// </summary>
    public class InteractableOnPressReceiver : ReceiverBase
    {

        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Release", Tooltip = "The button is released")]
        public UnityEvent OnRelease = new UnityEvent();

        private enum PressType
        {
            ALL_INTERACTIONS = 0,
            PHYSICAL_ONLY = 1,
            FAR_ONLY = 2
        }

        //This is what i want to do:
        //[SerializeField]
        //[Tooltip("Filter for Near and Far Press")]
        //public PressType pressTypeFilterTest = PressType.ALL_INTERACTIONS;
		
		// This is what it seems like I have to do, but it's broken:
		//[InspectorField(Label = "Press Filter Type", Tooltip = "A index value of the component", Type = InspectorField.FieldTypes.DropdownInt, Options = new string[] { "All Interactions", "Physical Only", "Far Only" })]
        // public int ComponentIndex = 2;
        // public int pressTypeFilter = (int)PressType.ALL_INTERACTIONS;

		// this is what I'm doing right now to at least make it work and test the behavior:
        [InspectorField(Label = "Press Type Filter", Tooltip = "Filter for Near and Far Press", Type = InspectorField.FieldTypes.Int)]
        public int PressTypeFilter = 0;


        private bool hasDown;
        private State lastState;

        private bool isNear = false;

        public InteractableOnPressReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnPress";
        }

        /// <summary>
        /// checks if the received interactable state matches the press filter
        /// </summary>
        /// <returns>true if interactable state matches filter</returns>
        private bool IsFilterValid()
        {
            if (PressTypeFilter == (int)PressType.FAR_ONLY && isNear
                || PressTypeFilter == (int)PressType.PHYSICAL_ONLY && !isNear)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool changed = state.CurrentState() != lastState;

            bool hadDown = hasDown;
            hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;


            if (changed && hasDown != hadDown)
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
            
            lastState = state.CurrentState();
        }
    }
}
