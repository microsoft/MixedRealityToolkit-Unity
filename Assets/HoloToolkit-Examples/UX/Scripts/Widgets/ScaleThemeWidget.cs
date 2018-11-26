// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;
using System;

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
        
        private string mCheckThemeTag = "";

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
            if (mScaleTheme == null)
            {
                SetTheme();
            }

            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            mScaleTheme = GetVector3Theme(ThemeTag);
            mCheckThemeTag = ThemeTag;
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

        private void Update()
        {
            if (!mCheckThemeTag.Equals(ThemeTag))
            {
                SetTheme();
                RefreshIfNeeded();
            }
        }
    }
}
