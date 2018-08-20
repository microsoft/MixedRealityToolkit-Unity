// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend.Automation
{
    /// <summary>
    /// snapps billboarding to specified increments
    /// </summary>
    public class BillboardSnapping : BillboardEasing
    {
        [Tooltip("the angle increment for snapping")]
        public float SnapAngle = 90;

        /// <summary>
        /// rotate the object
        /// </summary>
        /// <param name="lookRotation"></param>
        protected override void ApplyRotation(Quaternion lookRotation)
        {
            Vector3 direction = lookRotation.eulerAngles;
            float steps = Mathf.Round(direction.y / SnapAngle);

            if (PivotAxis == BillboardAxis.Y)
            {
                direction.y = steps * SnapAngle;
                lookRotation = Quaternion.Euler(direction);
            }
            else
            {
                //TODO: try to make this work like faceted rotations
            }

            base.ApplyRotation(lookRotation);
        }
    }
}
