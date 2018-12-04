using Pixie.Core;
using System;

namespace Pixie.DeviceControl
{
    [Serializable]
    public struct LocalDevicePrefsState
    {
        public short UserID;
        public DeviceRoleEnum DeviceRole;
    }
}