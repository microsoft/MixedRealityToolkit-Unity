using System.Collections.Generic;

namespace Pixie.DeviceControl
{
    /// <summary>
    /// Provides a limited view of user objects.
    /// Used by StateObjects.
    /// </summary>
    public interface IUserView
    {
        bool LocalUserAssigned { get; }
        IUserObject LocalUserObject { get; }
        IEnumerable<IUserObject> UserObjects { get; }

        bool GetUserObject(short userID, out IUserObject userObject);
    }
}