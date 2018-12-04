using Pixie.Core;
using Pixie.StateControl;
using Pixie.DeviceControl;
using System;

namespace Pixie.AppSystems.StateObjects
{
    public interface IStateObjectBase : IGameObject
    {
        short ItemID { get; }
        Type StateType { get; }

        IAppStateReadOnly AppState { get; }
        IUserView Users { get; }
        IStateView StateView { get; }

        bool IsUserType { get; }
        bool IsLocalUser { get; }
        bool IsInitialized { get; }
        bool AutoName { get; }

        void Initialize(short itemNum, IAppStateReadWrite appState, IUserView users, IStateView stateView);

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