using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    /// <summary>
    /// Manages adding / removing player objects as they join / leave the game.
    /// </summary>
    public interface IUserManager : IUserView, ISharingAppObject
    {
        void AddAssignedUserObject(IUserObject userObject);
        void RevokeAssignment(IUserObject userObject);
    }
}