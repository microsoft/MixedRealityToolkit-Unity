// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates the button colors based on the button theme
    /// handles two materials
    /// </summary>
    public class ButtonWidgetOutline : InteractiveWidget
    {
        public Material InnerMaterial;
        public Material OuterMaterial;
        public ColorButtonTheme InnterColorTheme;
        public ColorButtonTheme OuterColorTheme;
        public ColorTransition ColorBlender;

        private void Awake()
        {
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            ColorBlender.StartTransition(InnterColorTheme.GetThemeValue(state), InnerMaterial.name);
            ColorBlender.StartTransition(OuterColorTheme.GetThemeValue(state), OuterMaterial.name);

        }
    }
}
