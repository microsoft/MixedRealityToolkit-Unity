// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class TextMeshColorWidget : InteractiveWidget
    {
        public ColorButtonTheme TextColorTheme;
        public ColorTransition ColorBlender;

        private TextMesh mTextMesh;

        void Awake()
        {
            mTextMesh = GetComponent<TextMesh>();
            if (mTextMesh != null && TextColorTheme != null)
            {
                mTextMesh.color = TextColorTheme.GetThemeValue(Interactive.ButtonStateEnum.Default);
            }

            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (TextColorTheme != null)
            {
                if (ColorBlender != null)
                {
                    ColorBlender.StartTransition(TextColorTheme.GetThemeValue(state));
                }
                else if (mTextMesh != null)
                {
                    mTextMesh.color = TextColorTheme.GetThemeValue(state);
                }
            }
        }
    }
}
