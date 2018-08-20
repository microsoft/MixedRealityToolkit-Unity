// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interact;
using Blend;
using Interact.Themes;

namespace Interact.Widgets
{
	/// <summary>
    /// updates the button position based on the button theme
	/// use the Axis to issolate the position change,
	/// so the themes do not have to store information you do not care about.
	/// 
	/// This is a good widget for handling multiple items with a single theme.
	/// Like buttons sitting next to each other have different x positions,
	/// use this widget to only effect the y or z.
    /// </summary>
    public class PositionThemeAxisWidget : InteractiveThemeWidget
    {
        public Vector3 Axis = Vector3.one;
        private BlendPosition mTransform;
        private Vector3InteractiveTheme mPositionTheme;
        private Vector3 startLocalPosition;
        private Vector3 startPosition;
		
		private BlendVectors vectors;
		
        /// <summary>
        /// Get Move to Position
        /// </summary>
        private void Awake()
        {
            startPosition = transform.position;
            startLocalPosition = transform.localPosition;
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

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            Vector3 inverted = Vector3.Scale(Vector3.one, Vector3.one - Axis);
            
            base.SetState(state);

            if (mPositionTheme != null && !BlendSettings.UseBlend)
            {
                Vector3 newPosition = Vector3.Scale(mPositionTheme.GetThemeValue(state), Axis);
                Vector3 scaledPosition = Vector3.Scale(startPosition, inverted) + newPosition;
                Vector3 scaledLocalPosition = Vector3.Scale(startLocalPosition, inverted) + newPosition;

                if (mTransform != null)
                {
                    mTransform.TargetValue = mTransform.ToLocalTransform ? scaledLocalPosition : scaledPosition;
                    mTransform.Play();
                }
                else
                {
                    transform.localPosition = scaledLocalPosition;
                }
            }
        }

        protected override void ApplyBlendValues(float percent)
        {
			transform.localPosition = Vector3.Lerp(vectors.StartValue, vectors.TargetValue, percent);
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            Vector3 inverted = Vector3.Scale(Vector3.one, Vector3.one - Axis);
            vectors.StartValue = transform.localPosition;

            if (mPositionTheme != null)
            {
				Vector3 newPosition = Vector3.Scale(mPositionTheme.GetThemeValue(state), Axis);
				Vector3 scaledLocalPosition = Vector3.Scale(startLocalPosition, inverted) + newPosition;
                vectors.TargetValue = scaledLocalPosition;
                BlendSettings.Blend.Play();
            }
        }
    }
}
