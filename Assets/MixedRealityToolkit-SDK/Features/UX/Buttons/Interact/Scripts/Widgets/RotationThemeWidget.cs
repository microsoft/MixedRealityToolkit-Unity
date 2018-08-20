// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using Blend;
using Interact.Themes;

namespace Interact.Widgets
{
    /// <summary>
    /// updates the button rotation based on a Vector3 theme (Euler Angles)
    /// </summary>
    public class RotationThemeWidget : InteractiveThemeWidget
    {
        
        private BlendRotation mTransition;

        private Vector3InteractiveTheme mRotationTheme;

		private BlendVectors vectors;							 
        /// <summary>
        /// Get Move to Position
        /// </summary>
        private void Awake()
        {
            mTransition = GetComponent<BlendRotation>();
        }

        public override void SetTheme()
        {
            mRotationTheme = InteractiveThemeManager.GetVector3Theme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mRotationTheme != null;
        }

        /// <summary>
        /// Set the position
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mRotationTheme != null)
            {
				if (!BlendSettings.UseBlend)
				{
					if(mTransition != null)
					{
						mTransition.TargetValue = mRotationTheme.GetThemeValue(state);
						mTransition.Play();
					}
					else
					{
						transform.localRotation = Quaternion.Euler(mRotationTheme.GetThemeValue(state));
					}
				}
            }
            else
            {
                IsInited = false;
            }
        }
		protected override void ApplyBlendValues(float percent)
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(vectors.StartValue, vectors.TargetValue, percent));
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            vectors.StartValue = transform.localRotation.eulerAngles;
            if (mRotationTheme != null)
            {
                vectors.TargetValue = mRotationTheme.GetThemeValue(state);
                BlendSettings.Blend.Play();
            }
        }
    }
}
