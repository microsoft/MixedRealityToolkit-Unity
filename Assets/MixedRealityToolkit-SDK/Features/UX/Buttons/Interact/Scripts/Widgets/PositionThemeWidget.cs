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
    /// updates the button position based on the button theme
    /// </summary>
    public class PositionThemeWidget : InteractiveThemeWidget
    {

        private BlendPosition mTransform;

        private Vector3InteractiveTheme mPositionTheme;

		private BlendVectors vectors;							 
        /// <summary>
        /// Get Move to Position
        /// </summary>
        private void Awake()
        {
            mTransform = GetComponent<BlendPosition>();
        }

        public override void SetTheme()
        {
            mPositionTheme = InteractiveThemeManager.GetVector3Theme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mPositionTheme != null;
        }

        /// <summary>
        /// Set the position
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mPositionTheme != null && !BlendSettings.UseBlend)
            {
                if(mTransform != null)
                {
                    mTransform.TargetValue = mPositionTheme.GetThemeValue(state);
                    mTransform.Play();
                }
                else
                {
                    transform.localPosition = mPositionTheme.GetThemeValue(state);
                }
            }
        }
		
		protected override void ApplyBlendValues(float percent)
        {
            transform.localPosition = Vector3.Lerp(vectors.StartValue, vectors.TargetValue, percent);
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            vectors.StartValue = transform.localPosition;

            if (mPositionTheme != null)
            {
                vectors.TargetValue = mPositionTheme.GetThemeValue(state);
                BlendSettings.Blend.Play();
            }
        }
    }
}
