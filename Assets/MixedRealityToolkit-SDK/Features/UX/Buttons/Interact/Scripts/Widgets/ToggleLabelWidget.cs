// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.Widgets
{
    /// <summary>
    /// An Interactive Widget for changing text based on the selected state
    /// </summary>
    public class ToggleLabelWidget : InteractiveWidget
    {
        public ToggleLabelTheme theme;
        private TextMesh textMesh;
        private Text text;

        private void Awake()
        {
            theme = GetComponentInParent<ToggleLabelTheme>();
            textMesh = GetComponent<TextMesh>();
            text = GetComponent<Text>();
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            if(theme != null)
            {
                string label = InteractiveHost.IsSelected ? theme.Selected : theme.Default;

                if (textMesh != null)
                {
                    textMesh.text = label;
                }
                else if(text != null)
                {
                    text.text = label;
                }
            }
        }

    }
}
