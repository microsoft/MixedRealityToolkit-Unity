// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.Prototyping;
using MixedRealityToolkit.Examples.UX.Themes;
using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Controls
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

        private ColorInteractiveTheme mInnerColorTheme;
        private ColorInteractiveTheme mOuterColorTheme;

        private string mCheckInnerColorThemeTag = "";
        private string mCheckOuterColorThemeTag = "";

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
            SetTheme();
            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            if (InnerColorThemeTag != "")
            {
                mInnerColorTheme = GetColorTheme(InnerColorThemeTag);
                mCheckInnerColorThemeTag = InnerColorThemeTag;
            }

            if (OuterColorThemeTag != "")
            {
                mOuterColorTheme = GetColorTheme(OuterColorThemeTag);
                mCheckOuterColorThemeTag = OuterColorThemeTag;
            }
        }

        /// <summary>
        /// set the colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mInnerColorTheme != null)
            {
                ColorBlender.StartTransition(mInnerColorTheme.GetThemeValue(state), InnerMaterial.name);
            }

            if (mOuterColorTheme != null)
            {
                ColorBlender.StartTransition(mOuterColorTheme.GetThemeValue(state), OuterMaterial.name);
            }
        }

        private void Update()
        {
            if (!mCheckOuterColorThemeTag.Equals(OuterColorThemeTag) || !mCheckInnerColorThemeTag.Equals(InnerColorThemeTag))
            {
                SetTheme();
                RefreshIfNeeded();
            }
        }
    }
}
