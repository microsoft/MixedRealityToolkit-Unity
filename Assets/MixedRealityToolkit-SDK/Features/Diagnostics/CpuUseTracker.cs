using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class CpuUseTracker
    {
        private TimeSpan? processorTime;
        private Process currentProcess = Process.GetCurrentProcess();

        public void Reset()
        {
            processorTime = null;
        }

        public CpuReading GetReading()
        {
            if (!processorTime.HasValue)
            {
                processorTime = currentProcess.TotalProcessorTime;
                return new CpuReading();
            }

            var currentTime = currentProcess.TotalProcessorTime;
            var diff = currentTime - processorTime.Value;
            processorTime = currentTime;

            return new CpuReading()
            {
                CpuTime = diff
            };
        }

        public struct CpuReading
        {
            public TimeSpan CpuTime { get; set; }
        }
    }
}
