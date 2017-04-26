// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleRotation : CycleArray<Vector3>
    {
        [Tooltip("Requires Interpolator")]
        public bool SmoothLerpToTarget = false;
        public float RotationDegreesPerSecond = 720.0f;
        public float RotationSpeedScaler = 0.0f;
        public float SmoothRotationLerpRatio = 0.5f;

        private Interpolator mInterpolator;

        protected override void Awake()
        {
            mInterpolator = GetComponent<Interpolator>();

            base.Awake();
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Vector3 item = Array[Index];

            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = item;

            if (mInterpolator != null)
            {
                mInterpolator.SmoothLerpToTarget = SmoothLerpToTarget;
                mInterpolator.SmoothRotationLerpRatio = SmoothRotationLerpRatio;
                mInterpolator.RotationSpeedScaler = RotationSpeedScaler;
                mInterpolator.RotationDegreesPerSecond = RotationDegreesPerSecond;
                mInterpolator.SetTargetRotation(rotation);
            }
            else
            {
                TargetObject.transform.rotation = rotation;
            }
        }

    }
}
