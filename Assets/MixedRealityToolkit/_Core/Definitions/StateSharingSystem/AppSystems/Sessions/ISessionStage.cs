using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public interface ISessionStage : ISessionStageBase
    {
        IEnumerator Run(
              ITimeSource timeSource,
              IAppStateReadWrite appState,
              IUserView users,
              IStateView stateView);

        void ForceComplete();
    }
}