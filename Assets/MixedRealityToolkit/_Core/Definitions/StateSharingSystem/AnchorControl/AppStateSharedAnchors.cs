using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public class AppStateSharedAnchors : NetworkBehaviour, IAppStateSource
    {
        public bool Synchronized { get { return synchronized; } set { synchronized = value; } }

        public IEnumerable<Type> StateTypes { get { return new Type[] { typeof(AlignmentState), typeof(SharedAnchorState), typeof(UserAnchorState) }; } }

        public AlignmentStateSyncList AlignmentStates = new AlignmentStateSyncList();
        public SharedAnchorStateSyncList SharedAnchorStates = new SharedAnchorStateSyncList();
        public UserAnchorStateSyncList UserAnchorStates = new UserAnchorStateSyncList();

        [SerializeField]
        [SyncVar]
        private bool synchronized;
        
        public override int GetNetworkChannel()
        {
            return Globals.UNet.ChannelUnreliableFragmented;
        }

        public override float GetNetworkSendInterval()
        {
            return Globals.UNet.SendIntervalModerate;
        }

        public void GenerateRequiredStates(IAppStateReadWrite appState) { }

        [Serializable]
        public class AlignmentStateSyncList : SyncListStruct<AlignmentState> { }
        [Serializable]
        public class SharedAnchorStateSyncList : SyncListStruct<SharedAnchorState> { }
        [Serializable]
        public class UserAnchorStateSyncList : SyncListStruct<UserAnchorState> { }
    }
}