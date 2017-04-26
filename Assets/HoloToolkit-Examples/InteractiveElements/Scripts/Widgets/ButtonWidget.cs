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
    public class ButtonWidget : InteractiveWidget
    {
        public ColorButtonTheme ColorTheme;
        public PositionButtonTheme PositionTheme;
        public ScaleButtonTheme ScaleTheme;
        public ColorTransition ColorBlender;
        public MoveToPosition MovePosition;
        public ScaleToValue ScaleSize;

        private Material mMaterial;

        private void Awake()
        {
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }

            if (MovePosition == null)
            {
                MovePosition = GetComponent<MoveToPosition>();
            }

            if (ScaleSize == null)
            {
                ScaleSize = GetComponent<ScaleToValue>();
            }

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                mMaterial = renderer.material;
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (ColorTheme != null)
            {
                if (ColorBlender != null)
                {
                    ColorBlender.StartTransition(ColorTheme.GetThemeValue(state));
                }
                else if(mMaterial != null)
                {
                    mMaterial.color = ColorTheme.GetThemeValue(state);
                }
            }
            
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

        private void OnDestroy()
        {
            if(mMaterial != null)
            {
                GameObject.Destroy(mMaterial);
            }
        }
    }
}
