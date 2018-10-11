using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserManagerServer : IUserManager
    {
        bool AllSlotsFilled { get; }
        bool AllUsersInitialized { get; }

        void GenerateUserStates(int numSessions, IEnumerable<StandInSetting> slotTypes);
        void ClearUserSlot(sbyte slotNum);
        bool SetSlotIgnored(sbyte slotNum, bool ignore);
        bool TryAssignUserToSlot(IUserObject userObject, sbyte slotNum, UserDeviceEnum userDevice, UserSlot.FillStateEnum state = UserSlot.FillStateEnum.Filled);
    }
}