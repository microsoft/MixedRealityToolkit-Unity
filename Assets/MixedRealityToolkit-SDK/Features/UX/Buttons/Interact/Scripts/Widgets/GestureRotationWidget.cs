// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{

    public class GestureRotationWidget : GestureWidget
    {

        public Vector3 StartRotation;
        public Vector3 RotateDirection = Vector3.one;
        public float Distance;
        public bool AdditiveDirection;
        public bool DirectionalAxis;
        //public bool LookDirection; // we should use an enum for type of output instead of all these bools.

        protected override void Start()
        {
            base.Start();
        }

        protected override void UpdateValues(float percent)
        {
            Vector3 direction = Control.GestureVector.normalized;

            float x = Vector3.Dot(direction, Camera.main.transform.right) * percent;
            float y = Vector3.Dot(direction, Camera.main.transform.up) * percent;
            float z = Vector3.Dot(direction, Camera.main.transform.forward) * percent;

            Vector3 calculatedDirection = DirectionalAxis ? new Vector3(y, x+z, 0) : direction;
            /*
             * TODO: make a version that looks at the gesture direction and simplify the toggles.
            if (LookDirection)
            {
                calculatedDirection = (Quaternion.LookRotation(direction, Camera.main.transform.right) * Quaternion.LookRotation(direction, Camera.main.transform.forward)).eulerAngles;
            */

            if (AdditiveDirection)
            {
                if (Distance > 0)
                {
                    transform.localRotation = Quaternion.Euler(StartRotation) * Quaternion.Euler(Vector3.Scale(calculatedDirection, RotateDirection) * Distance * percent);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(StartRotation) * Quaternion.Euler(Vector3.Scale(calculatedDirection, RotateDirection) * percent);
                }
            }
            else
            {
                if (Distance > 0)
                {
                    transform.localRotation = Quaternion.Euler(StartRotation) * Quaternion.Euler(RotateDirection * Distance * percent);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(StartRotation) * Quaternion.Euler(RotateDirection * percent);
                }
            }

        }
    }
}
