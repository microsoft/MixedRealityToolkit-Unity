// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Blend.Cycle
{
    /// <summary>
    /// Cycle through a list of colors and apply the current color to the material
    /// Supports ColorTransition for animaiton and easing. Auto detected, just add it to the component
    /// </summary>
    public class CycleColors : CycleArray<Color>
    {
        [Tooltip("Select the shader color property to animate: _Color is default")]
        public string ShaderColorProperty = ColorAbstraction.DefaultColor;

        // color to blend to
        private Color mTargetColor;
        
        // color transition component - used for animation
        private BlendColor mTransition;
        

        private ColorAbstraction mColorAbstraction;

        private bool mStarted = false;

        protected override void Awake()
        {
            base.Awake();
            mTransition = TargetObject.GetComponent<BlendColor>();
        }

        protected override void Start()
        {
            base.Start();
            mStarted = true;
        }

        /// <summary>
        /// Select the color from the Array and apply it.
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            mTargetColor = Array[Index];

            if (mTransition != null && mStarted)
            {
                mTransition.TargetValue = mTargetColor;
                mTransition.Play();
            }
            else
            {
                GetColorAbstraction().SetColor(mTargetColor);
            }
        }

        protected ColorAbstraction GetColorAbstraction()
        {
            if (mColorAbstraction == null)
            {
                mColorAbstraction = new ColorAbstraction(this.gameObject, ShaderColorProperty);
            }

            return mColorAbstraction;
        }
    }
}
