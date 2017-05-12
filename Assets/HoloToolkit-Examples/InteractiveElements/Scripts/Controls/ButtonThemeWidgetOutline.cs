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
    public class ButtonThemeWidgetOutline : InteractiveThemeWidget
    {   
        [Tooltip("Button inner material")]
        public Material InnerMaterial;

        [Tooltip("Button outer material")]
        public Material OuterMaterial;

        [Tooltip("tag for the color theme for the inner material")]
        public string InnerColorThemeTag = "innerColorDefault";

        [Tooltip("tag for the color theme for the outer material")]
        public string OuterColorThemeTag = "outerColorDefault";

        [Tooltip("A color tween component : required, but could be on a different object")]
        public ColorTransition ColorBlender;

        private ColorInteractiveTheme mInnterColorTheme;
        private ColorInteractiveTheme mOuterColorTheme;
        
        /// <summary>
        /// set the ColorBlender
        /// </summary>
        private void Awake()
        {
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }

            if (ColorBlender == null)
            {
                Debug.LogError("ColorBlender is not on the gameObject: " + name);
                Destroy(this);
            }
        }

        /// <summary>
        /// get the themes
        /// </summary>
        private void Start()
        {
            if (InnerColorThemeTag != "")
            {
                mInnterColorTheme = GetColorTheme(InnerColorThemeTag);
            }

            if (OuterColorThemeTag != "")
            {
                mOuterColorTheme = GetColorTheme(OuterColorThemeTag);
            }
        }

        /// <summary>
        /// set the colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mInnterColorTheme != null)
            {
                ColorBlender.StartTransition(mInnterColorTheme.GetThemeValue(state), InnerMaterial.name);
            }

            if (mOuterColorTheme != null)
            {
                ColorBlender.StartTransition(mOuterColorTheme.GetThemeValue(state), OuterMaterial.name);
            }
        }
    }
}
