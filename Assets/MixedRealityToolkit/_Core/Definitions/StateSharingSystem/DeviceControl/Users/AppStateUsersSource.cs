using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class AppStateUsersSource : NetworkBehaviour, IAppStateSource, IUserStateGenerator
    {
        public bool Synchronized { get { return synchronized; } set { synchronized = value; } }

        public IEnumerable<Type> StateTypes { get { return new Type[] { typeof(UserSlot), typeof(UserState) }; } }

        public int ExecutionOrder { get { return 0; } }

        public UserSlotSyncList UserSlots = new UserSlotSyncList();
        public UserStateSynclist UserStates = new UserStateSynclist();

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
            UserState userState = new UserState(slot.SessionNum, slot.ItemNum);
            userState.UserRole = UserRoleEnum.None;
            userState.UserTeam = UserTeamEnum.None;
            userState.UserDevice = UserDeviceEnum.Unknown;
            appState.AddState<UserState>(userState);
        }

        // Class definitions
        // This is how you would define custom sync list structs
        // They must be defined within the NetworkBehavior class that's syncing them
        // In cases where you have a large number of lists to sync,
        // you can define them in other NetworkBehaviors attached to this gameObject.
        // AppStateBase will automatically search for sync list struct properties and add them to its state lookup.
        [Serializable]
        public class UserSlotSyncList : SyncListStruct<UserSlot> { }
        [Serializable]
        public class UserStateSynclist : SyncListStruct<UserState> { }
    }
}