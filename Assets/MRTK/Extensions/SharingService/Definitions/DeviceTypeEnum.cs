using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    [Serializable]
    public enum DeviceTypeEnum : byte
    {
        None = 0,
        HoloLens = 1,
        Mobile = 2,
        Immersive = 4,
        IOT = 8,
    }
}