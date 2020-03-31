using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct DeviceEventArgs
    {
        public short DeviceID;
        public string DeviceName;
        public bool IsLocalDevice;
        public Dictionary<string, string> Properties;
    }
}