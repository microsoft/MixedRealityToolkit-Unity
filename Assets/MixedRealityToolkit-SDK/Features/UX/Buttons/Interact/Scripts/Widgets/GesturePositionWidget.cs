// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// Applies gesture data to the position of an object
    /// </summary>
    public class GesturePositionWidget : GestureWidget
    {

        public Vector3 StartPosition;
        public Vector3 MoveDirection = Vector3.one;
        public float Distance;
        public bool AdditiveDirection;

        protected override void Start()
        {
            base.Start();
        }

        protected override void UpdateValues(float percent)
        {
            Vector3 direction = Control.GestureVector.normalized;

            if(AdditiveDirection)
            {
                if (Distance > 0)
                {
                    transform.localPosition = StartPosition + Vector3.Scale(direction, MoveDirection) * Distance * percent;
                }
                else
                {
                    transform.localPosition = StartPosition + Vector3.Scale(direction, MoveDirection) * percent;
                }
            }
            else
            {
                if (Distance > 0)
                {
                    transform.localPosition = StartPosition + MoveDirection * Distance * percent;
                }
                else
                {
                    transform.localPosition = StartPosition + MoveDirection * percent;
                }
            }
            
        }
    }
}
