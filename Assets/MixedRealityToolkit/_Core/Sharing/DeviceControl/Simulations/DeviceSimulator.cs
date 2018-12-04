using Pixie.Core;
using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.StateObjects;
using Pixie.DeviceControl.Users;
using Pixie.StateControl;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Simulations
{
    public class DeviceSimulator : MonoBehaviourSharingApp, IDeviceSimulator
    {
        [SerializeField]
        private GameObject simPrefab;

        private List<ISimActor> sims = new List<ISimActor>();

        public void CreateSimulatedDevice()
        {
            GameObject simGo = GameObject.Instantiate(simPrefab);
            ISimActor sim = (ISimActor)simGo.GetComponent(typeof(ISimActor));
            sims.Add(sim);
        }

        public void UpdateSimulation(IAppStateReadWrite appState, IUserManager users, IStateView stateView, IExperienceMode gameMode)
        {
            foreach (ISimActor sim in sims)
            {
                sim.UpdateSimulation(appState, users, stateView, gameMode);
            }
        }
    }
}