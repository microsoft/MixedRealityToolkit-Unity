using Pixie.Core;
using System;

namespace Pixie.DeviceControl
{
    [Serializable]
    [AppStateType]
    public struct UserDeviceState : IItemState, IItemStateComparer<UserDeviceState>
    {
        public UserDeviceState(short deviceID)
        {
            DeviceID = deviceID;
            DeviceName = "Device";
            DeviceType = DeviceTypeEnum.None;
            UserID = -1;
            DeviceRoleIndex = 0;
            ConnectionState = DeviceConnectionStateEnum.NotConnected;
        }

        short IItemState.Key { get { return DeviceID; } }

        public short DeviceID;
        public string DeviceName;
        public DeviceTypeEnum DeviceType;
        public short UserID;
        public byte DeviceRoleIndex;
        public DeviceConnectionStateEnum ConnectionState;

        public bool IsAssigned { get { return UserID >= 0; } }

        public bool IsDifferent(UserDeviceState from)
        {
            return DeviceID != from.DeviceID
                || DeviceName != from.DeviceName
                || DeviceType != from.DeviceType
                || UserID != from.UserID
                || DeviceRoleIndex != from.DeviceRoleIndex
                || ConnectionState != from.ConnectionState;
        }

        public UserDeviceState Merge(UserDeviceState clientValue, UserDeviceState serverValue)
        {
            return serverValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}