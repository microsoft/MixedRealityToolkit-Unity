// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class FpsUseTracker
    {
        private int index = 0;
        private float[] timings = new float[10];

        public float GetFpsInSeconds()
        {
            timings[index] = Time.unscaledDeltaTime;
            index = (index + 1) % timings.Length;

            return timings.Average();
        }
    }
}
