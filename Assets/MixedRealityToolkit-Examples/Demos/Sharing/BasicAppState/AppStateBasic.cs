using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.Demos
{
    public class AppStateBasic : MonoBehaviour, IAppStateSource
    {
        /// <summary>
        /// Returns the state types we want to serialize.
        /// </summary>
        public IEnumerable<Type> StateTypes
        {
            get { return new Type[] { typeof(BasicState) }; }
        }

        /// <summary>
        /// This is called immediately after a state type is added to the app state.
        /// You can use it to generate states which you KNOW all users will be using.
        /// (If you're trying to generate user states, use IUserStateGenerator instead.)
        /// </summary>
        /// <param name="appState"></param>
        public void GenerateRequiredStates(IAppStateReadWrite appState)
        {
            // We're going to generate 3 basic states with non-sequential IDs.
            // We're not using sessions or any other filtration technique here, so filter is just set to zero.
            BasicState state1 = new BasicState(5);
            BasicState state2 = new BasicState(7);
            BasicState state3 = new BasicState(9);

            appState.AddState<BasicState>(state1);
            appState.AddState<BasicState>(state2);
            appState.AddState<BasicState>(state3);
        }

        public static Core.StateArray<BasicState> BasicStateArray;
    }
}