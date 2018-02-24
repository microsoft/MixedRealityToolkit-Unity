// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// Sets the rotation (eulerAngles) of an object to the selected value in the array.
    /// Add RotateToValue for animation and easing, auto detected.
    /// </summary>
    public class CycleRotation : CycleArray<Vector3>
    {
        [Tooltip("use the local rotation - overrides the UseLocalTransform value of RotateToValue")]
        public bool UseLocalRotation = false;

        private RotateToValue mRotation;

        protected override void Awake()
        {
            mRotation = GetComponent<RotateToValue>();

            base.Awake();
        }

        /// <summary>
        /// Set the rotation from the Vector3 Euler angles
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            // get the rotation and convert it to a Quaternion
            Vector3 item = Array[Index];

            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = item;

            // set the rotation
            if (mRotation != null)
            {
                mRotation.ToLocalTransform = UseLocalRotation;
                mRotation.TargetValue = rotation;
                mRotation.StartRunning();
            }
            else
            {
                if (UseLocalRotation)
                {
                    TargetObject.transform.localRotation = rotation;
                }
                else
                {
                    TargetObject.transform.rotation = rotation;
                }
            }
        }

    }
}
