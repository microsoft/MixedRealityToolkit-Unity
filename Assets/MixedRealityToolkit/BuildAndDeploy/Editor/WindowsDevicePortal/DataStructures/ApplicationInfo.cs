using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class ApplicationInfo
    {
        public string Name;
        public string PackageFamilyName;
        public string PackageFullName;
        public int PackageOrigin;
        public string PackageRelativeId;
        public string Publisher;
    }
}
