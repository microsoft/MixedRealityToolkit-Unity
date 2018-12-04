using System.Collections.Generic;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using Pixie.StateControl;

namespace Pixie.AnchorControl
{
    public interface IUserAlignment
    {
        void SetAlignedWorldTransforms(IUserView users, IAppStateReadWrite appState);
        void AlignUserObjects(IEnumerable<IUserObject> userObjects, IAppStateReadOnly appState);
    }
}