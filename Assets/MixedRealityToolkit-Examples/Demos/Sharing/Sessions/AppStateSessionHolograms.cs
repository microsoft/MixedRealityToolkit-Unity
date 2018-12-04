using Pixie.Core;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.Demos
{
    public class AppStateSessionHolograms : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes
        {
            get { return new Type[] { typeof(SessionHologramState) }; }
        }

        public void GenerateRequiredStates(IAppStateReadWrite appState) { }

        [SerializeField]
        private BasicHologram[] holograms;

        // TEMPORARY WORKAROUND - defining this AOT so IL2CPP builds can use it. This will be handled automatically.
        static StateArray<SessionHologramState> basicHologramStates;
    }
}
