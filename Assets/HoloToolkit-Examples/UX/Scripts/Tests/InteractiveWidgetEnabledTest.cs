// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Sample InteractiveWidget for displaying text indicating if the button is enabled or disabled
    /// </remarks>
    public class InteractiveWidgetEnabledTest : InteractiveWidget
    {
        public TextMesh TextField;
        private bool mIsEnabled = true;
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
            switch (state)
            {
                case Interactive.ButtonStateEnum.Default:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.Focus:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.Press:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.Selected:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.FocusSelected:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.PressSelected:
                    mIsEnabled = true;
                    break;
                case Interactive.ButtonStateEnum.Disabled:
                    mIsEnabled = false;
                    break;
                case Interactive.ButtonStateEnum.DisabledSelected:
                    mIsEnabled = false;
                    break;
                default:
                    break;
            }

            TextField.text = mIsEnabled ? "(Enabled)" : "(Disabled)";
        }
    }
}
