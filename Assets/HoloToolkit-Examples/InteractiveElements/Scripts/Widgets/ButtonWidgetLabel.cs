// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates the button element postion based on CyclePosition
    /// </summary>
    public class ButtonWidgetLabel : InteractiveWidget
    {
        public ColorButtonTheme ColorTheme;
        public PositionButtonTheme PositionTheme;
        public MoveToPosition MovePosition;
        public LabelTheme ButtonLabels;
        private TextMesh mText;
        
        private void Awake()
        {
            mText = this.gameObject.GetComponent<TextMesh>();
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mText != null)
            {
                if (ColorTheme != null)
                {
                    mText.color = ColorTheme.GetThemeValue(state);
                }

                if (ButtonLabels != null)
                {
                    if (InteractiveHost.IsSelected)
                    {
                        if (ButtonLabels.Selected != "")
                        {
                            mText.text = ButtonLabels.Selected;
                        }
                        else
                        {
                            mText.text = ButtonLabels.Default;
                        }
                    }
                    else
                    {
                        mText.text = ButtonLabels.Default;
                    }
                }
            }

            if (MovePosition != null)
            {
                MovePosition.TargetValue = PositionTheme.GetThemeValue(state);
                MovePosition.StartRunning();
            }
            else
            {
                transform.localPosition = PositionTheme.GetThemeValue(state);
            }
        }
    }
}
