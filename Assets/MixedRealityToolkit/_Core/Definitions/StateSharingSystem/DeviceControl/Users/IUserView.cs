using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    /// <summary>
    /// Provides a limited view of user objects.
    /// Used by StateObjects.
    /// </summary>
    public interface IUserView
    {
        bool LocalUserSpawned { get; }
        ILocalUserObject LocalUserObject { get; }
        IEnumerable<IUserObject> UserObjects { get; }

        bool GetUserObject(short userNum, out IUserObject userObject);
    }
}