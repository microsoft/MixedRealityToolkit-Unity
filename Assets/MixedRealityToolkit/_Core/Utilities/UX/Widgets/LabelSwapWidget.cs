// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX.Widgets
{
    /// <summary>
    /// Swaps the text in the TextMesh based on Interactive state, like "On" and "Off"
    /// </summary>
    public class LabelSwapWidget : InteractiveWidget
    {
        [Tooltip("string for the default state")]
        public string DefaultLabel;

        [Tooltip("string for the selected state")]
        public string SelectedLabel;

        public TextMesh Label;

        /// <summary>
        /// Get the TextMesh
        /// </summary>
        private void Awake()
        {
            if (Label == null)
            {
                Label = GetComponent<TextMesh>();
            }

            if (Label == null)
            {
                Debug.LogError("Textmesh:Label is not set in LabelSwapWidget!");
                Destroy(this);
            }
        }

        /// <summary>
        /// Set the text value
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(ButtonStateEnum state)
        {
            base.SetState(state);

            string label = "";
            if (state == ButtonStateEnum.FocusSelected || state == ButtonStateEnum.Selected || state == ButtonStateEnum.PressSelected || state == ButtonStateEnum.DisabledSelected)
            {
                label = SelectedLabel;
            }
            else
            {
                label = DefaultLabel;
            }

            Label.text = label;

        }
    }
}
