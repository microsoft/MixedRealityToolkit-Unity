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
    public class ShaderFloatThemeWidget : InteractiveThemeWidget
    {
		[HideInInspector]				 
        [Tooltip("Select the shader float property to animate - overrided by BlendShaderFloat if attached or assigned.")]
        public string ShaderPropery;

        private string shaderColorType = ColorAbstraction.DefaultColor;

        private BlendShaderFloat mTransition;

        protected FloatInteractiveTheme floatTheme;
        protected ColorAbstraction mColorAbstraction;

		protected BlendFloats floats;							 
        protected bool inited = false;

        private void Awake()
        {
            // set up the color abstraction layer
            mColorAbstraction = new ColorAbstraction(gameObject, shaderColorType);

            mTransition = GetComponent<BlendShaderFloat>();
        }

        public override void SetTheme()
        {
            floatTheme = InteractiveThemeManager.GetFloatTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return floatTheme != null;
        }

        /// <summary>
        /// Set or fade the colors
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (floatTheme != null && !BlendSettings.UseBlend)
            {
                if (mTransition != null && inited)
                {
                    mTransition.TargetValue = floatTheme.GetThemeValue(state);
                    mTransition.Play();
                }
                else
                {
                    mColorAbstraction.SetShaderFloat(ShaderPropery, floatTheme.GetThemeValue(state));
                    inited = true;
                }
            }
        }
		protected override void ApplyBlendValues(float percent)
        {
            float newValue = floats.StartValue + (floats.TargetValue - floats.StartValue) * percent;
            mColorAbstraction.SetShaderFloat(ShaderPropery, newValue);
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {

            floats.StartValue = mColorAbstraction.GetShaderFloat(ShaderPropery);
            if (floatTheme != null)
            {
                floats.TargetValue = floatTheme.GetThemeValue(state);
                BlendSettings.Blend.Play();
            }
        }
    }
}
