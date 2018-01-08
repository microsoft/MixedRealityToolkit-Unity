// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HoloToolkit.UI.Keyboard
{
    /// <summary>
    /// This is an input field that overrides getting deselected
    /// </summary>
    public class SliderInputField : InputField
    {
        /// <summary>
        /// Override OnDeselect
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDeselect(BaseEventData eventData)
        {
            // Do nothing for deselection
        }
    }
}
