// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Cycle through a list of colors and apply the current color to the material
    /// Supports ColorTransition for animation and easing. Auto detected, just add it to the component
    /// </summary>
    public class CycleColors : CycleArray<Color>
    {
        // color to blend to
        private Color mTargetColor;

        // the material to change colors
        private Material mMaterial;

        // color transition component - used for animation
        private ColorTransition mColorTransition;

        protected override void Start()
        {
            mColorTransition = TargetObject.GetComponent<ColorTransition>();
            base.Start();
        }

        /// <summary>
        /// Select the color from the Array and apply it.
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            mTargetColor = Array[Index];

            if (mColorTransition == null)
            {
                Renderer renderer = TargetObject.GetComponent<Renderer>();

                if (renderer != null)
                {
                    mMaterial = renderer.material;
                }

                mMaterial.color = mTargetColor;
            }
            else
            {
                mColorTransition.StartTransition(mTargetColor);
            }
        }

        /// <summary>
        /// clean up material if one was created dynamically
        /// </summary>
        private void OnDestroy()
        {
            if(mMaterial != null)
            {
                GameObject.Destroy(mMaterial);
            }
        }
    }
}
