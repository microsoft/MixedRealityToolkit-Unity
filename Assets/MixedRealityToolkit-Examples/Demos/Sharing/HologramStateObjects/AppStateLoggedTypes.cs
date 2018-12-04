using System;
using System.Collections.Generic;
using Pixie.Core;
using Pixie.StateControl;
using UnityEngine;

namespace Pixie.Demos
{
    public class AppStateLoggedTypes : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes
        {
            get { return new Type[] { typeof(LoggedTypeAState), typeof (LoggedTypeBState) }; }
        }

        public void GenerateRequiredStates(IAppStateReadWrite appState)
        {
            // Don't generate any states this time - we'll do that at runtime
        }

        // TEMPORARY WORKAROUND - defining this AOT so IL2CPP builds can use it. This will be handled automatically.
        static StateArray<HologramState> hologramStates;
    }
}