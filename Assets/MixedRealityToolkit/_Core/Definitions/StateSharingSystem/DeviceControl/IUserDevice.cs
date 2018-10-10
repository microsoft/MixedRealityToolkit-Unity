using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    public interface IUserDevice : IGameObject
    {
        short DeviceID { get; }
        bool Initialized { get; }
        bool IsDestroyed { get; }
        RuntimePlatform DevicePlatform { get; }
        UserDeviceEnum DeviceType { get; }
        string DeviceName { get; }
    }
}