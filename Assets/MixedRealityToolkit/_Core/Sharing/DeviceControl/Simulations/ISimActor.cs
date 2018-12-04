using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.StateObjects;
using Pixie.DeviceControl.Users;
using Pixie.StateControl;
using UnityEngine;

namespace Pixie.DeviceControl.Simulations
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

        void UpdateSimulation(IAppStateReadOnly appState, IUserManager users, IStateView stateView, IExperienceMode gameMode);
    }
}