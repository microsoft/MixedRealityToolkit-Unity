// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Blend.Cycle
{
    /// <summary>
    /// scales an object based on the selected value in the array
    /// Supports ScaletoValue for animaiton and easing, ...auto detected.
    /// </summary>
    public class CycleScale : CycleArray<Vector3>
    {
        private BlendScale mTransition;

        protected override void Awake()
        {
            base.Awake();

            mTransition = TargetObject.GetComponent<BlendScale>();
        }

        /// <summary>
        /// Set the scale value or animate scale
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Vector3 item = Current;

            if (mTransition != null)
            {
                mTransition.TargetValue = item;
                mTransition.Play();
            }
            else
            {
                TargetObject.transform.localScale = item;
            }
        }
    }
}
