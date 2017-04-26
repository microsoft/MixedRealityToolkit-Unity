// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates the button colors based on the button theme
    /// updates button element position based on CyclePosition
    /// </summary>
    public class PositionWidget : InteractiveWidget
    {
        public PositionButtonTheme PositionTheme;
        public MoveToPosition MovePosition;

        private void Awake()
        {
            if (MovePosition == null)
            {
                MovePosition = GetComponent<MoveToPosition>();
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (PositionTheme != null)
            {
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
}
