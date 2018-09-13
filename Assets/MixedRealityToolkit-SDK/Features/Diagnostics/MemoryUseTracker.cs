using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

internal class MemoryUseTracker
{
    private Process currentProcess = Process.GetCurrentProcess();

    public MemoryReading GetReading()
    {
        return new MemoryReading()
        {
            VirtualMemoryInBytes = currentProcess.VirtualMemorySize64,
            WorkingSetMemoryInBytes = currentProcess.WorkingSet64,
            GCMemory = GC.GetTotalMemory(false)
        };
    }

    public struct MemoryReading
    {
        public long VirtualMemoryInBytes { get; set; }
        public long WorkingSetMemoryInBytes { get; set; }
        public long GCMemory { get; set; }
    }
}
