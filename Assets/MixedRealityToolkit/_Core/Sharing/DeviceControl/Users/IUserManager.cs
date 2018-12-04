using Pixie.Core;
using System.Collections.Generic;

namespace Pixie.DeviceControl.Users
{
    public interface IUserManager : IUserView, ISharingAppObject
    {
        bool AllSlotsFilled { get; }
        bool UsersDefined { get; }

        void GenerateUserStates(IEnumerable<UserDefinition> userDefinitions);
        void CheckForUserObjects();
        void ClearUserSlot(short userID);
        bool SetSlotFillState(short userID, UserSlot.FillStateEnum fillState);
    }
}