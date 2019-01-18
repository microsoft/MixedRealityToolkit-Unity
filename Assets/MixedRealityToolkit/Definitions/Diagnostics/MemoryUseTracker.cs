// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    /// <summary>
    /// The Mixed Reality Toolkit's Memory Use Tracker.
    /// </summary>
    public struct MemoryUseTracker
    {
        private readonly MemoryReading[] memoryReadings;
        private MemoryReading sumReading;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer">The number of readings for this tracker.</param>
        public MemoryUseTracker(int buffer = 10)
        {
            memoryReadings = new MemoryReading[buffer];
            sumReading = new MemoryReading();
            index = 0;

            for (int i = 0; i < memoryReadings.Length; i++)
            {
                memoryReadings[i] = new MemoryReading();
            }
        }

        /// <summary>
        /// The current <see cref="MemoryReading"/>
        /// </summary>
        public MemoryReading CurrentReading
        {
            get
            {
                memoryReadings[index].GcMemoryInBytes = GC.GetTotalMemory(false);
                index = (index + 1) % memoryReadings.Length;

                return memoryReadings.Aggregate(sumReading.Reset(), (a, b) => a + b) / memoryReadings.Length;
            }
        }
    }
}
