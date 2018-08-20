// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blend
{
    /// <summary>
    /// Transition from date to date
    /// </summary>
    public class BlendDateTime : Blend<DateTime>
    {
        [Tooltip("The output value")]
        public DateTime Value;

        public override bool CompareValues(DateTime value1, DateTime value2)
        {
            return value1 == value2;
        }

        public override DateTime GetValue()
        {
            return Value;
        }

        public override DateTime LerpValues(DateTime startValue, DateTime targetValue, float percent)
        {
            double addedTimeTicks = (targetValue - startValue).Ticks * (double)percent;
            TimeSpan addedTimeSpan = TimeSpan.FromTicks((long)addedTimeTicks);
            return startValue + addedTimeSpan;
        }

        public override void SetValue(DateTime value)
        {
            Value = value;
        }
    }
}

