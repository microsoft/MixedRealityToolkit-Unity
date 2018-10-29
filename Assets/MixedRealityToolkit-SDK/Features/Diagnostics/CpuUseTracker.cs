// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

#if !WINDOWS_UWP || ENABLE_IL2CPP
using System.Diagnostics;
using System.Linq;
#endif

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class CpuUseTracker
    {
        private TimeSpan? processorTime;
        private TimeSpan[] readings = new TimeSpan[20];
        private int index = 0;

#if !WINDOWS_UWP || ENABLE_IL2CPP
        private Process currentProcess = Process.GetCurrentProcess();
#endif

        public void Reset()
        {
            processorTime = null;
        }

        public double GetReadingInMs()
        {
#if WINDOWS_UWP && !ENABLE_IL2CPP
            // UWP doesn't support process with .NET runtime, so we can't get CPU readings with the current pattern.
            return -1;
#else
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
#endif
        }
    }
}
