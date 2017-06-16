// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates the button scale based on the button theme
    /// </summary>
    public class ScaleThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultScale";

        [Tooltip("Scale to Value component for animating scale")]
        public ScaleToValue ScaleTweener;

        private Vector3InteractiveTheme mScaleTheme;
        private Material mMaterial;

        /// <summary>
        /// Get Scale to Value
        /// </summary>
        private void Awake()
        {
            if (ScaleTweener == null)
            {
                ScaleTweener = GetComponent<ScaleToValue>();
            }
        }

        /// <summary>
        /// Get the Theme
        /// </summary>
        private void Start()
        {
            mScaleTheme = GetVector3Theme(ThemeTag);
        }

        /// <summary>
        /// Set the Scale
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mScaleTheme != null)
            {
                if (ScaleTweener != null)
                {
                    ScaleTweener.TargetValue = mScaleTheme.GetThemeValue(state);
                    ScaleTweener.StartRunning();
                }
                else
                {
                    transform.localScale = mScaleTheme.GetThemeValue(state);
                }
            }
        }
    }
}
