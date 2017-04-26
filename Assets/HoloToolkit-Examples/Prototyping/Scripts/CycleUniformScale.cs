// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleUniformScale : CycleArray<float>
    {
        [Tooltip("Requires Interpolator")]
        public bool SmoothLerpToTarget = false;
        public float ScalePerSecond = 5.0f;
        public float SmoothScaleLerpRatio = 0.5f;

        private Vector3 StartScale;
        private bool isFirstCall = true;

        private Interpolator mInterpolator;

        protected override void Awake()
        {
            mInterpolator = GetComponent<Interpolator>();

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

            if (mInterpolator != null)
            {
                mInterpolator.SmoothLerpToTarget = SmoothLerpToTarget;
                mInterpolator.SmoothScaleLerpRatio = SmoothScaleLerpRatio;
                mInterpolator.ScalePerSecond = ScalePerSecond;
                mInterpolator.SetTargetLocalScale(item * StartScale);
            }
            else
            {
                TargetObject.transform.localScale = item * StartScale;
            }
        }
    }
}
