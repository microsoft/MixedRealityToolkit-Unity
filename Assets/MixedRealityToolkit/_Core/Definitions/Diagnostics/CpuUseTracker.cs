// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

#if ENABLE_IL2CPP && !WINDOWS_UWP
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
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer">The number of readings for this tracker.</param>
        public CpuUseTracker(int buffer = 20)
        {
#if ENABLE_IL2CPP && !WINDOWS_UWP
            currentProcess = Process.GetCurrentProcess();
            readings = new TimeSpan[buffer];
            processorTime = null;
            index = 0;
#endif
        }

#if ENABLE_IL2CPP && !WINDOWS_UWP
        private readonly Process currentProcess;
        private readonly TimeSpan[] readings;
        private TimeSpan? processorTime;
        private int index;
#endif

        /// <summary>
        /// The current reading in Milliseconds.
        /// </summary>
        public double CurrentReadingInMs
        {
            get
            {
#if ENABLE_IL2CPP && !WINDOWS_UWP
                if (!processorTime.HasValue)
                {
                    try
                    {
                        processorTime = currentProcess.TotalProcessorTime;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                    }

                    return 0;
                }

                var currentTime = currentProcess.TotalProcessorTime;
                var diff = currentTime - processorTime.Value;
                processorTime = currentTime;

                readings[index] = diff;
                index = (index + 1) % readings.Length;

                return Math.Round(readings.Average(t => t.TotalMilliseconds), 2);
#else
                // Platforms that don't support IL2CPP can't get CPU readings with the current pattern.
                return -1;
#endif
            }
        }

        /// <summary>
        /// Reset the tracker.
        /// </summary>
        public void Reset()
        {
#if ENABLE_IL2CPP && !WINDOWS_UWP
            processorTime = null;
#endif
        }
    }
}
