using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.Demos
{
    public class AppStateSubscriptionTypes : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes
        {
            get { return new Type[] {
                typeof(StateTypeA),
                typeof(StateTypeB),
                typeof(StateTypeC),
            }; }
        }

        public void GenerateRequiredStates(IAppStateReadWrite appState)
        {
            StateTypeA stateA = new StateTypeA(1);
            StateTypeB stateB = new StateTypeB(1);
            StateTypeC stateC = new StateTypeC(1);

            appState.AddState<StateTypeA>(stateA);
            appState.AddState<StateTypeB>(stateB);
            appState.AddState<StateTypeC>(stateC);
        }

        public static Core.StateArray<StateTypeA> StateTypeAArray;
        public static Core.StateArray<StateTypeB> StateTypeBArray;
        public static Core.StateArray<StateTypeC> StateTypeCArray;
    }
}