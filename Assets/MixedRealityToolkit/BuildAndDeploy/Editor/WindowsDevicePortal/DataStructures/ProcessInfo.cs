using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class ProcessInfo
    {
        public float CPUUsage;
        public string ImageName;
        public float PageFileUsage;
        public int PrivateWorkingSet;
        public int ProcessId;
        public int SessionId;
        public string UserName;
        public int VirtualSize;
        public int WorkingSetSize;
    }
}