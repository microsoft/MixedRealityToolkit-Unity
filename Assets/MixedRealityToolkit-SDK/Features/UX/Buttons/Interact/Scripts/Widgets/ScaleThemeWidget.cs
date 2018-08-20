// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System;
using Blend;
using Interact.Themes;

namespace Interact.Widgets
{
    /// <summary>
    /// updates the button scale based on the button theme
    /// </summary>
    public class ScaleThemeWidget : InteractiveThemeWidget
    {
        private BlendScale mTransition;

        private Vector3InteractiveTheme mScaleTheme;
        private Material mMaterial;

		private BlendVectors vectors;							 
        /// <summary>
        /// Get Scale to Value
        /// </summary>
        private void Awake()
        {
            mTransition = GetComponent<BlendScale>();
        }

        public override void SetTheme()
        {
            mScaleTheme = InteractiveThemeManager.GetVector3Theme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mScaleTheme != null;
        }

        /// <summary>
        /// Set the Scale
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mScaleTheme != null && !BlendSettings.UseBlend)
            {
                if(mTransition != null)
                {
                    mTransition.TargetValue = mScaleTheme.GetThemeValue(state);
                    mTransition.Play();
                }
                else
                {
                    transform.localScale = mScaleTheme.GetThemeValue(state);
                }
            }
        }
		protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            vectors.StartValue = transform.localScale;
            if (mScaleTheme != null)
            {
                vectors.TargetValue = mScaleTheme.GetThemeValue(state);
                BlendSettings.Blend.Play();
            }
        }

        protected override void ApplyBlendValues(float percent)
        {
            transform.localScale = Vector3.Lerp(vectors.StartValue, vectors.TargetValue, percent);
        }
    }
}
