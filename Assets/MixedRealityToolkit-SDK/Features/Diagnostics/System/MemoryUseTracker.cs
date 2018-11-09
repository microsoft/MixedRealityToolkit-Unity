// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class MemoryUseTracker
    {
        private readonly MemoryReading[] memoryReadings;
        private int index = 0;

        private MemoryReading sumReading = new MemoryReading();

        public MemoryUseTracker()
        {
            memoryReadings = new MemoryReading[10];

            for (int i = 0; i < memoryReadings.Length; i++)
            {
                memoryReadings[i] = new MemoryReading();
            }
        }


        public MemoryReading GetReading()
        {
            var reading = memoryReadings[index];
            reading.GCMemoryInBytes = GC.GetTotalMemory(false);

            index = (index + 1) % memoryReadings.Length;

            return memoryReadings.Aggregate(sumReading.Reset(), (a, b) => a + b) / memoryReadings.Length;
        }

        public struct MemoryReading
        {
            public long GCMemoryInBytes { get; set; }

            public MemoryReading Reset()
            {
                GCMemoryInBytes = 0;

                return this;
            }

            public static MemoryReading operator +(MemoryReading a, MemoryReading b)
            {
                a.GCMemoryInBytes += b.GCMemoryInBytes;

                return a;
            }

            public static MemoryReading operator /(MemoryReading a, int b)
            {
                a.GCMemoryInBytes /= b;

                return a;
            }
        }
    }
}
