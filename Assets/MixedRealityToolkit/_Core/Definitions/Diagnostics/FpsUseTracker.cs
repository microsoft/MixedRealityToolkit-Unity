// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    /// <summary>
    /// The Mixed Reality Toolkit's Fps use tracker.
    /// </summary>
    public struct FpsUseTracker
    {
        private readonly float[] timings;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer">The number of readings for this tracker.</param>
        public FpsUseTracker(int buffer = 10)
        {
            timings = new float[buffer];
            index = 0;
        }

        /// <summary>
        /// Gets the current FPS in seconds.
        /// </summary>
        public float CurrentReadingInSeconds
        {
            get
            {
                timings[index] = Time.unscaledDeltaTime;
                index = (index + 1) % timings.Length;
                return timings.Average();
            }
        }
    }
}
