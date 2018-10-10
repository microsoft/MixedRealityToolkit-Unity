using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.Targeting
{
    public class AppStateTargetSource : NetworkBehaviour, IAppStateSource, IUserStateGenerator
    {
        public bool Synchronized { get { return synchronized; } set { synchronized = value; } }

        public IEnumerable<Type> StateTypes { get { return new Type[] { typeof(TargetState) }; } }

        public int ExecutionOrder { get { return 0; } }

        public TargetStateSyncList TargetStates = new TargetStateSyncList();

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

        public void GenerateRequiredStates(IAppStateReadWrite appState) { }

        public void GenerateUserStates(UserSlot slot, IAppStateReadWrite appState)
        {
            TargetState targetState = new TargetState(slot.SessionNum, slot.ItemNum);
            targetState.TargetType = TargetTypeEnum.None;
            appState.AddState<TargetState>(targetState);
        }

        [Serializable]
        public class TargetStateSyncList : SyncListStruct<TargetState> { }
    }
}