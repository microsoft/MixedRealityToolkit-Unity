// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
