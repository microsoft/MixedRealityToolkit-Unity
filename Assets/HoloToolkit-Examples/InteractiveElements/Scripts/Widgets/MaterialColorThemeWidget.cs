// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Changes the color of a material based on the Interactive state and the assigned theme
    /// </summary>
    public class MaterialColorThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultColor";

        [Tooltip("A component for color transitions: optional")]
        public ColorTransition ColorBlender;

        protected ColorInteractiveTheme mColorTheme;
        protected Material mMaterial;

        void Awake()
        {
            // get the color tweener
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }

            // get the renderer and material
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                mMaterial = renderer.material;
            }

            if (mMaterial != null && mColorTheme != null)
            {
                mMaterial.color = mColorTheme.GetThemeValue(Interactive.ButtonStateEnum.Default);
            }
        }

        private void Start()
        {
            mColorTheme = GetColorTheme(ThemeTag);
        }

        /// <summary>
        /// Set or fade the colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mColorTheme != null)
            {
                if (ColorBlender != null)
                {
                    ColorBlender.StartTransition(mColorTheme.GetThemeValue(state));
                }
                else if (mMaterial != null)
                {
                    mMaterial.color = mColorTheme.GetThemeValue(state);
                }
            }
        }

        /// <summary>
        /// Clean up the materal is created dynamically
        /// </summary>
        private void OnDestroy()
        {
            if (mMaterial != null)
            {
                GameObject.Destroy(mMaterial);
            }
        }
    }
}
