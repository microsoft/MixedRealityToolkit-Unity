using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public interface IStateObjectBase : IGameObject
    {
        sbyte ItemNum { get; }
        Type StateType { get; }

        IAppStateReadOnly AppState { get; }
        IUserView Users { get; }
        IStateView StateView { get; }

        bool IsUserType { get; }
        bool IsLocalUser { get; }
        bool IsInitialized { get; }
        bool AutoName { get; }

        void Initialize(sbyte itemNum, IAppStateReadWrite appState, IUserView users, IStateView stateView);

        /// <summary>
        /// Checks app state for changed states, calls actions
        /// </summary>
        void CheckAppState();

        /// <summary>
        /// Updates client side behavior
        /// </summary>
        void UpdateClientListeners();

        /// <summary>
        /// Updates server side behavior
        /// </summary>
        void UpdateServerListeners();
    }
}