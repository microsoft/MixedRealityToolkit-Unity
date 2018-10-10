using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public interface IUserAlignment
    {
        void AlignUserStates(IEnumerable<IUserObject> userObjects, IAppStateReadWrite appState);
        void AlignUserObjects(IEnumerable<IUserObject> userObjects, IAppStateReadOnly appState);
    }
}