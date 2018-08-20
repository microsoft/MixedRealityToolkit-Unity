// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using HoloToolkit.Unity;
using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// Changes the color of a material based on the Interactive state and the assigned theme
    /// </summary>
    public class ColorThemeWidget : InteractiveThemeWidget
    {
        [HideInInspector]
        public string ShaderPropertyName = ColorAbstraction.DefaultColor;

        private BlendColor mTransition;

        protected ColorInteractiveTheme mColorTheme;
        protected ColorAbstraction mColorAbstraction;

		protected BlendColors colors;							 
        protected bool inited = false;

        private void Awake()
        {
            // set up the color abstraction layer
            mColorAbstraction = new ColorAbstraction(gameObject, ShaderPropertyName);

            mTransition = GetComponent<BlendColor>();
        }

        public override void SetTheme()
        {
            mColorTheme = InteractiveThemeManager.GetColorTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mColorTheme != null;
        }

        /// <summary>
        /// Set or fade the colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mColorTheme != null && !BlendSettings.UseBlend)
            {
                if (mTransition != null && inited)
                {
                    mTransition.TargetValue = mColorTheme.GetThemeValue(state);
                    mTransition.Play();
                }
                else
                {
                    mColorAbstraction.SetColor(mColorTheme.GetThemeValue(state));
                    inited = true;
                }
            }
        }

        protected override void ApplyBlendValues(float percent)
        {
            Color color = Color.Lerp(colors.StartValue, colors.TargetValue, percent);
            mColorAbstraction.SetColor(color);
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            if (mColorTheme != null)
            {
                colors.TargetValue = mColorTheme.GetThemeValue(state);
                mColorAbstraction.SetShaderColorType(ShaderPropertyName);
                colors.StartValue = mColorAbstraction.GetColor();
                BlendSettings.Blend.Play();
            }
        }
    }
}
