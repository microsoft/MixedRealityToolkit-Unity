using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Simulations
{
    /// <summary>
    /// Looks at each slot in the player manager and creates / updates simulated users for any 'simulated' slots it finds.
    /// </summary>
    public interface IDeviceSimulator : ISharingAppObject
    {
        void UpdateSimulation(IAppStateReadWrite appState, IUserManagerServer users, IStateView stateView, IExperienceMode gameMode);
        void CreateSimulatedDevice();
    }
}