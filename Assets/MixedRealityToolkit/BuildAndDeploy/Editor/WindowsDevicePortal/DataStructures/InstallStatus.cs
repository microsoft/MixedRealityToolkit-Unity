using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class InstallStatus
    {
        public int Code;
        public string CodeText;
        public string Reason;
        public bool Success;
    }
}