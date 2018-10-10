using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserStateGenerator
    {
        int ExecutionOrder { get; }
        void GenerateUserStates(UserSlot slot, IAppStateReadWrite appState);
    }
}