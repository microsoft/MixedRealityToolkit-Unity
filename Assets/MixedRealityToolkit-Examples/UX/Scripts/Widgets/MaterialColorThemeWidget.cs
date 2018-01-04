// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.Prototyping;
using MixedRealityToolkit.Examples.UX.Themes;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Widgets
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

        private ColorInteractiveTheme mColorTheme;
        private Material mMaterial;

        private string mCheckThemeTag = "";

        void Awake()
        {
            // get the color blender
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
                mMaterial.color = mColorTheme.GetThemeValue(State);
            }
        }

        private void Start()
        {
            if (mColorTheme == null)
            {
                SetTheme();
            }

            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            mColorTheme = GetColorTheme(ThemeTag);
            mCheckThemeTag = ThemeTag;
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

        private void Update()
        {
            if (!mCheckThemeTag.Equals(ThemeTag))
            {
                SetTheme();
                RefreshIfNeeded();
            }
        }

        /// <summary>
        /// Clean up the material is created dynamically
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
