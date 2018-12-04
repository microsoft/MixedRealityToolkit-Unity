using Pixie.Core;
using System.Collections.Generic;

namespace Pixie.DeviceControl
{
    public interface IDeviceObject
    {
        void AssignDeviceID(short deviceID);
    }

    public interface IDeviceSource : ISharingAppObject
    {
        short LocalDeviceID { get; }
        bool LocalDeviceConnected { get; }
        IEnumerable<short> ConnectedDevices { get; }

        event DeviceEventDelegate OnDeviceConnected;
        event DeviceEventDelegate OnDeviceDisconnected;

        void PingDevice(short deviceID);
        void DisconnectDevice(short deviceID);
    }
}