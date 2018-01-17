// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Tests
{
    /// <summary>
    /// Sample InteractiveWidget for displaying the current ButtonState
    /// </remarks>
    public class InteractiveWidgetStateTest : InteractiveWidget
    {
        public TextMesh TextField;

        private void Start()
        {
            if(TextField == null)
            {
                TextField = GetComponent<TextMesh>();
            }
            
        }
        /// <summary>
        /// Interactive calls this method on state change
        /// </summary>
        /// <param name="state">
        /// Enum containing the following states:
        /// DefaultState: normal state of the button
        /// FocusState: gameObject has gaze
        /// PressState: currently being pressed
        /// SelectedState: selected and has no other interaction
        /// FocusSelected: selected with gaze
        /// PressSelected: selected and pressed
        /// Disabled: button is disabled
        /// DisabledSelected: the button is not interactive, but in it's alternate state (toggle button)
        /// </param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            string stateString = "Default";
            switch (state)
            {
                case Interactive.ButtonStateEnum.Default:
                    stateString = "Default";
                    break;
                case Interactive.ButtonStateEnum.Focus:
                    stateString = "Focus";
                    break;
                case Interactive.ButtonStateEnum.Press:
                    stateString = "Press";
                    break;
                case Interactive.ButtonStateEnum.Selected:
                    stateString = "Selected";
                    break;
                case Interactive.ButtonStateEnum.FocusSelected:
                    stateString = "FocusSelected";
                    break;
                case Interactive.ButtonStateEnum.PressSelected:
                    stateString = "PressSelected";
                    break;
                case Interactive.ButtonStateEnum.Disabled:
                    stateString = "Disabled";
                    break;
                case Interactive.ButtonStateEnum.DisabledSelected:
                    stateString = "DisabledSelected";
                    break;
                default:
                    break;
            }

            TextField.text = stateString;
        }
    }
}
