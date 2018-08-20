// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using Blend;
using Interact.Themes;

namespace Interact.Widgets
{
    /// <summary>
    /// updates the button rotation based on a Quaternion theme
    /// </summary>
    public class QuaternionThemeWidget : InteractiveThemeWidget
    {

        private BlendQuaternion mTransition;

        private QuaternionInteractiveTheme mRotationTheme;
		private BlendQuaternions quaternions;									 
        
        /// <summary>
        /// Get Move to Position
        /// </summary>
        private void Awake()
        {
            mTransition = GetComponent<BlendQuaternion>();
        }

        public override void SetTheme()
        {
            mRotationTheme = InteractiveThemeManager.GetQuaternionTheme(ThemeTag);
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
					if (mTransition != null)
					{
						mTransition.TargetValue = mRotationTheme.GetThemeValue(state);
						mTransition.Play();
					}
					else
					{
						transform.localRotation = mRotationTheme.GetThemeValue(state);
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
            transform.localRotation = Quaternion.Lerp(quaternions.StartValue, quaternions.TargetValue, percent);
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            quaternions.StartValue = transform.localRotation;

            if (mRotationTheme != null)
            {
                quaternions.TargetValue = mRotationTheme.GetThemeValue(state);
                BlendSettings.Blend.Play();
            }

        }
    }
}
