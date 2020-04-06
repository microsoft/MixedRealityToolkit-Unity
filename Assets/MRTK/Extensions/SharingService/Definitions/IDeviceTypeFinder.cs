// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Interface for a class that figures out a device's type based on system info. Used by ISharingService to determine a local device's type.
    /// </summary>
    public interface IDeviceTypeFinder
    {
        DeviceTypeEnum GetDeviceType();
    }
}