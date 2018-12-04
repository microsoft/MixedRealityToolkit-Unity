using Pixie.Core;
using Pixie.StateControl;
using Pixie.DeviceControl;

namespace Pixie.AppSystems.StateObjects
{
    /// <summary>
    /// A Unity object that reflects the state of an accompanying ItemState.
    /// Automatically updated and maintained by an IStateView.
    /// </summary>
    public interface IStateObject<T> : IGameObject
    {
        short ItemID { get; }
        bool IsUserType { get; }
        bool IsLocalUser { get; }

        IAppStateReadOnly AppState { get; }
        IUserView Users { get; }
        IStateView StateView { get; }

        T PreviousState { get; }
        T CurrentState { get; }
    }
}