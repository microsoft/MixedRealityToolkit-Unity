using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    /// <summary>
    /// A Unity object that reflects the state of an accompanying ItemState.
    /// Automatically updated and maintained by an IStateView.
    /// </summary>
    public interface IStateObject<T> : IGameObject
    {
        sbyte ItemNum { get; }
        bool IsUserType { get; }
        bool IsLocalUser { get; }

        IAppStateReadOnly AppState { get; }
        IUserView Users { get; }
        IStateView StateView { get; }

        T PreviousState { get; }
        T CurrentState { get; }
    }
}