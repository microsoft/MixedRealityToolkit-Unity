using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Simulations
{
    public interface ISimActor
    {
        Vector3 TargetDestination { get; }
        Quaternion TargetRotation { get; }

        bool IsIdle { get; }
        bool HasValidTarget { get; }
        bool CoolDownInEffect { get; }

        bool SeekPotentialTarget();
        bool AssessTarget();
        bool MoveTowardsTarget();
        bool FocusOnTarget();
        bool ActOnTarget();
        bool WaitForCooldown();
        void GotoRandomPosition();

        void UpdateSimulation(IAppStateReadOnly appState, IUserManagerServer users, IStateView stateView, IExperienceMode gameMode);
    }
}