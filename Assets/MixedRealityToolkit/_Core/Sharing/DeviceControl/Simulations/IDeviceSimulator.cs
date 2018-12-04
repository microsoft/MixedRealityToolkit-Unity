using Pixie.Core;
using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.StateObjects;
using Pixie.DeviceControl.Users;
using Pixie.StateControl;

namespace Pixie.DeviceControl.Simulations
{
    /// <summary>
    /// Looks at each slot in the player manager and creates / updates simulated users for any 'simulated' slots it finds.
    /// </summary>
    public interface IDeviceSimulator : ISharingAppObject
    {
        void UpdateSimulation(IAppStateReadWrite appState, IUserManager users, IStateView stateView, IExperienceMode gameMode);
        void CreateSimulatedDevice();
    }
}