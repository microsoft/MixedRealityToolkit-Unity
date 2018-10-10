using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Simulations
{
    public class DeviceSimulator : MonoBehaviour, IDeviceSimulator
    {
        public AppRoleEnum AppRole { get; set; }

        [SerializeField]
        private GameObject simPrefab;

        private List<ISimActor> sims = new List<ISimActor>();

        public void CreateSimulatedDevice()
        {
            GameObject simGo = GameObject.Instantiate(simPrefab);
            ISimActor sim = (ISimActor)simGo.GetComponent(typeof(ISimActor));
            sims.Add(sim);
        }

        public void OnSharingStart() { }

        public void OnStateInitialized() { }

        public void OnSharingStop() { }

        public void UpdateSimulation(IAppStateReadWrite appState, IUserManagerServer users, IStateView stateView, IExperienceMode gameMode)
        {
            foreach (ISimActor sim in sims)
            {
                sim.UpdateSimulation(appState, users, stateView, gameMode);
            }
        }
    }
}