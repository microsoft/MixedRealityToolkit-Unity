using Pixie.Core;
using System.Collections.Generic;

namespace Pixie.DeviceControl
{
    public delegate void DeviceEventDelegate(short deviceID, string deviceName, bool isLocalDevice, Dictionary<string,string> properties);

    public interface IDeviceAssigner : ISharingAppObject
    {
        void RevokeAssignment(short deviceID);
        bool TryAssignDevice(short deviceID, short userID, DeviceRoleEnum deviceRole);
        bool GetAssignedUser(short userID, DeviceRoleEnum deviceRole, out short deviceID);
    }
}