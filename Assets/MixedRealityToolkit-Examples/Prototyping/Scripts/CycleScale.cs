// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// scales an object based on the selected value in the array
    /// Supports ScaleToValue for animation and easing, ...auto detected.
    /// </summary>
    public class CycleScale : CycleArray<Vector3>
    {
        private ScaleToValue mScaler;

        protected override void Awake()
        {
            mScaler = GetComponent<ScaleToValue>();

            base.Awake();
        }

        /// <summary>
        /// Set the scale value or animate scale
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Vector3 item = Current;

            if (mScaler != null)
            {
                mScaler.TargetValue = item;
                mScaler.StartRunning();
            }
            else
            {
                TargetObject.transform.localScale = item;
            }
        }
    }
}
