using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public class AppStateSessionsSource : NetworkBehaviour, IAppStateSource
    {
        public bool Synchronized { get { return synchronized; } set { synchronized = value; } }

        public IEnumerable<Type> StateTypes { get { return new Type[] { typeof(SessionState) }; } }

        public SessionStateSyncList SessionStates = new SessionStateSyncList();

        [SerializeField]
        [SyncVar]
        private bool synchronized;

        [SerializeField]
        private int numConcurrentSessions = 1;

        public override int GetNetworkChannel()
        {
            return Globals.UNet.ChannelAllCosts;
        }

        public override float GetNetworkSendInterval()
        {
            return Globals.UNet.SendIntervalAllCosts;
        }

        public void GenerateRequiredStates(IAppStateReadWrite appStateBase)
        {
            for (sbyte i = 0; i < numConcurrentSessions; i++)
            {
                SessionState sessionState = new SessionState(i);
                appStateBase.AddState<SessionState>(sessionState);
            }
        }

        // Class definitions
        // This is how you would define custom sync list structs
        // They must be defined within the NetworkBehavior class that's syncing them
        // In cases where you have a large number of lists to sync,
        // you can define them in other NetworkBehaviors attached to this gameObject.
        // AppStateBase will automatically search for sync list struct properties and add them to its state lookup.
        [Serializable]
        public class SessionStateSyncList : SyncListStruct<SessionState> { }
    }
}