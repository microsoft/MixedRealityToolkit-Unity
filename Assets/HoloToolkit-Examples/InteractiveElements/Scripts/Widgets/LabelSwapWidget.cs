// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class LabelSwapWidget : InteractiveWidget
    {

        public string DefaultLabel;
        public string SelectedLabel;
        public TextMesh Label;

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

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            string label = "";
            if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Selected || state == Interactive.ButtonStateEnum.PressSelected || state == Interactive.ButtonStateEnum.DisabledSelected)
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
