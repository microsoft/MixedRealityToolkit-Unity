// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

#if !WINDOWS_UWP || ENABLE_IL2CPP
using System.Diagnostics;
using System.Linq;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    /// <summary>
    /// The Mixed Reality Toolkit's Cpu use tracker.
    /// </summary>
    public struct CpuUseTracker
    {
#if !WINDOWS_UWP || ENABLE_IL2CPP
        private readonly Process currentProcess;
#endif

        private readonly TimeSpan[] readings;
        private TimeSpan? processorTime;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer">The number of readings for this tracker.</param>
        public CpuUseTracker(int buffer = 20)
        {
#if !WINDOWS_UWP || ENABLE_IL2CPP
            currentProcess = Process.GetCurrentProcess();
#endif
            readings = new TimeSpan[buffer];
            processorTime = null;
            index = 0;
        }

        /// <summary>
        /// Reset the tracker.
        /// </summary>
        public void Reset()
        {
            processorTime = null;
        }

        /// <summary>
        /// The current reading in Milliseconds.
        /// </summary>
        public double CurrentReadingInMs
        {
            get
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
}
