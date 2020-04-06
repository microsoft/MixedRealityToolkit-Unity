// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Default implementation of IDeviceTypeFinder.
    /// Create your own if the sharing service is having trouble accurately guessing the type of connecting devices.
    /// </summary>
    public class DefaultDeviceTypeFinder : IDeviceTypeFinder
    {
        public DeviceTypeEnum GetDeviceType()
        {
            // TODO - badly need to improve and test these
            switch (SystemInfo.deviceType)
            {
                case DeviceType.Desktop:
                    return DeviceTypeEnum.Immersive;
                case DeviceType.Console:
                    return DeviceTypeEnum.Immersive;
                case DeviceType.Handheld:
                    return DeviceTypeEnum.Mobile;
                case DeviceType.Unknown:
                    return DeviceTypeEnum.IOT;
                default:
                    return DeviceTypeEnum.None;
            }
        }
    }
}