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
    public class ScaleWidget : InteractiveWidget
    {
        public ScaleButtonTheme ScaleTheme;
        public ScaleToValue ScaleSize;

        private Material mMaterial;

        private void Awake()
        {
            if (ScaleSize == null)
            {
                ScaleSize = GetComponent<ScaleToValue>();
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (ScaleTheme != null)
            {
                if (ScaleSize != null)
                {
                    ScaleSize.TargetValue = ScaleTheme.GetThemeValue(state);
                    ScaleSize.StartRunning();
                }
                else
                {
                    transform.localScale = ScaleTheme.GetThemeValue(state);
                }
            }
        }
    }
}
