// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Uniformly scales an object using a float based on the selected value from the array
    /// Supports ScaleToValue for animation and easing, ...auto detected
    /// </summary>
    public class CycleUniformScale : CycleArray<float>
    {
        private Vector3 StartScale;
        private bool isFirstCall = true;

        private ScaleToValue mScaler;

        protected override void Awake()
        {
            mScaler = GetComponent<ScaleToValue>();

            base.Awake();
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (isFirstCall)
            {
                StartScale = TargetObject.transform.localScale;
                isFirstCall = false;
            }

            float item = Current;

            if (mScaler != null)
            {
                mScaler.TargetValue = item * StartScale;
                mScaler.StartRunning();
            }
            else
            {
                TargetObject.transform.localScale = item * StartScale;
            }
        }
    }
}
