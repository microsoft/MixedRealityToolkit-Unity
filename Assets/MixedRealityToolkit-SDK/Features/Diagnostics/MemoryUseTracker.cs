// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Diagnostics;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class MemoryUseTracker
    {
        private Process currentProcess = Process.GetCurrentProcess();
        private MemoryReading[] memoryReadings = new MemoryReading[10];
        private int index = 0;

        public MemoryReading GetReading()
        {
            var reading = new MemoryReading()
            {
                VirtualMemoryInBytes = currentProcess.VirtualMemorySize64,
                WorkingSetMemoryInBytes = currentProcess.WorkingSet64,
                GCMemoryInBytes = GC.GetTotalMemory(false)
            };

            memoryReadings[index] = reading;
            index = (index + 1) % memoryReadings.Length;

            var sum = memoryReadings.Aggregate(new MemoryReading(), (a, b) => a + b);
            return sum / memoryReadings.Length;
        }

        public struct MemoryReading
        {
            public long VirtualMemoryInBytes { get; set; }
            public long WorkingSetMemoryInBytes { get; set; }
            public long GCMemoryInBytes { get; set; }

            public static MemoryReading operator +(MemoryReading a, MemoryReading b)
            {
                return new MemoryReading()
                {
                    VirtualMemoryInBytes = a.VirtualMemoryInBytes + b.VirtualMemoryInBytes,
                    WorkingSetMemoryInBytes = a.WorkingSetMemoryInBytes + b.WorkingSetMemoryInBytes,
                    GCMemoryInBytes = a.GCMemoryInBytes + b.GCMemoryInBytes
                };
            }

            public static MemoryReading operator /(MemoryReading a, int b)
            {
                return new MemoryReading()
                {
                    VirtualMemoryInBytes = a.VirtualMemoryInBytes / b,
                    WorkingSetMemoryInBytes = a.WorkingSetMemoryInBytes / b,
                    GCMemoryInBytes = a.GCMemoryInBytes / b
                };
            }
        }
    }
}
