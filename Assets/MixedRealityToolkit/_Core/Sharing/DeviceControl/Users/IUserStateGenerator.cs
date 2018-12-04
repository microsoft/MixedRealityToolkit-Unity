using Pixie.StateControl;

namespace Pixie.DeviceControl.Users
{
    public interface IUserStateGenerator
    {
        int ExecutionOrder { get; }
        void GenerateUserStates(UserSlot slot, IAppStateReadWrite appState);
    }
}