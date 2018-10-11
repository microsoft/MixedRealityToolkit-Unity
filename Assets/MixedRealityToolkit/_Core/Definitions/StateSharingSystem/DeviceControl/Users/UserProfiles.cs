using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class UserProfiles : MonoBehaviour, IUserProfiles
    {
        public IEnumerable<UserProfile> Profiles { get { return profiles; } }

        public UserProfile GetBestMatch(UserRoleEnum userRole, UserTeamEnum userTeam, UserDeviceEnum userDevice)
        {
            // TEMP
            return profiles[0];
        }

        [SerializeField]
        private UserProfile[] profiles;
    }
}