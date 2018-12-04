using Pixie.Core;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AppSystems.Sessions
{
    public class AppStateSessionsSource : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes { get { return new Type[] { typeof(SessionState) }; } }

        [SerializeField]
        private int numConcurrentSessions = 1;

        public void GenerateRequiredStates(IAppStateReadWrite appStateBase)
        {
            for (sbyte i = 0; i < numConcurrentSessions; i++)
            {
                SessionState sessionState = new SessionState(i);
                appStateBase.AddState<SessionState>(sessionState);
            }
        }

        // TEMPORARY WORKAROUND - defining this AOT so IL2CPP builds can use it. This will be handled automatically.
        static StateArray<SessionState> sessionStates;
    }
}