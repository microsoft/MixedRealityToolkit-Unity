using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class InterfaceInfo
    {
        public string Description;
        public string GUID;
        public int Index;
        public NetworkProfileInfo[] ProfilesList;
    }
}