using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    [Serializable]
    public struct StandInSetting
    {
        public UserTeamEnum Team;
        public UserRoleEnum Role;
        [Tooltip("Type must include full namespace.")]
        public string StandInClassType;
    }
}