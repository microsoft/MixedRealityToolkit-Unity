// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{

    public class GestureScaleWidget : GestureWidget
    {

        public Vector3 StartScale;
        public Vector3 ScaleDirection = Vector3.one;
        public float Distance;
        public bool AdditiveDirection;

        protected override void Start()
        {
            base.Start();
        }

        protected override void UpdateValues(float percent)
        {
            Vector3 direction = Control.GestureVector.normalized;

            if (AdditiveDirection)
            {
                if (Distance > 0)
                {
                    transform.localScale = StartScale + Vector3.Scale(direction, ScaleDirection) * Distance * percent;
                }
                else
                {
                    transform.localScale = StartScale + Vector3.Scale(direction, ScaleDirection) * percent;
                }
            }
            else
            {
                if (Distance > 0)
                {
                    transform.localScale = StartScale + ScaleDirection * Distance * percent;
                }
                else
                {
                    transform.localScale = StartScale + ScaleDirection * percent;
                }
            }

        }
    }
}
