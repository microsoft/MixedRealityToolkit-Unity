using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    public interface IDevicePool : ISharingAppObject
    {
        IEnumerable<IUserDevice> Devices { get; }

        void AddDevice(IUserDevice device);
        void RevokeAssignment(IUserDevice device);
        bool TryAssignDevice(IUserDevice device, sbyte userSlot);
        bool GetAssignedUserSlot(IUserDevice device, out sbyte userSlot);
    }
}