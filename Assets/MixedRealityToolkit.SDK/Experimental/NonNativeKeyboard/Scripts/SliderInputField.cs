// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This is an input field that overrides getting deselected
    /// </summary>
    public class SliderInputField : TMP_InputField
    {
        /// <summary>
        /// Override OnDeselect
        /// </summary>
        public override void OnDeselect(BaseEventData eventData)
        {
            // Do nothing for deselection
        }
    }
}
