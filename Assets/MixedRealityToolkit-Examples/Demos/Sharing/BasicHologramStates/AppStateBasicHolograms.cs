using Pixie.Core;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.Demos
{
    public class AppStateBasicHolograms : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes
        {
            get { return new Type[] { typeof(BasicHologramState) }; }
        }

        public void GenerateRequiredStates(IAppStateReadWrite appState)
        {
            // Create a state for each of the hologram objects in our scene
            foreach (BasicHologram hologram in holograms)
            {
                BasicHologramState hologramState = new BasicHologramState(hologram.HologramID);
                appState.AddState<BasicHologramState>(hologramState);
            }
        }

        [SerializeField]
        private BasicHologram[] holograms;

        // TEMPORARY WORKAROUND - defining this AOT so IL2CPP builds can use it. This will be handled automatically.
        static StateArray<BasicHologramState> basicHologramStates;
    }
}