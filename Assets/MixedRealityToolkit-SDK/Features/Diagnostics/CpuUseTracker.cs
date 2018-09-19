// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Diagnostics;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class CpuUseTracker
    {
        private TimeSpan? processorTime;
        private Process currentProcess = Process.GetCurrentProcess();
        private TimeSpan[] readings = new TimeSpan[20];
        int index = 0;

        public void Reset()
        {
            processorTime = null;
        }

        public double GetReadingInMs()
        {
            if (!processorTime.HasValue)
            {
                processorTime = currentProcess.TotalProcessorTime;
                return 0;
            }

            var currentTime = currentProcess.TotalProcessorTime;
            var diff = currentTime - processorTime.Value;
            processorTime = currentTime;

            readings[index] = diff;
            index = (index + 1) % readings.Length;

            return Math.Round(readings.Average(t => t.TotalMilliseconds), 2);
        }
    }
}
