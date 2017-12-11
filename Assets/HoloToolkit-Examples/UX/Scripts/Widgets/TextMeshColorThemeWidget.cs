// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// An InteractiveThemeWidget for swapping colors on a TextMesh based on Interactive state
    /// </summary>
    public class TextMeshColorThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultColor";

        [Tooltip("A component for color transitions: optional")]
        public ColorTransition ColorBlender;

        private ColorInteractiveTheme mTextColorTheme;
        private TextMesh mTextMesh;

        private string mCheckThemeTag = "";

        void Awake()
        {
            // get the TextMesh
            mTextMesh = GetComponent<TextMesh>();
            if (mTextMesh != null && mTextColorTheme != null)
            {
                mTextMesh.color = mTextColorTheme.GetThemeValue(Interactive.ButtonStateEnum.Default);
            }

            // get the ColorBlender if on self
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }
        }

        private void Start()
        {
            if (mTextColorTheme == null)
            {
                SetTheme();
            }
            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            mTextColorTheme = GetColorTheme(ThemeTag);
            mCheckThemeTag = ThemeTag;
        }

        /// <summary>
        /// Update colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mTextColorTheme != null)
            {
                if (ColorBlender != null)
                {
                    ColorBlender.StartTransition(mTextColorTheme.GetThemeValue(state));
                }
                else if (mTextMesh != null)
                {
                    mTextMesh.color = mTextColorTheme.GetThemeValue(state);
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
