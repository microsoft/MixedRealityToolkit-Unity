using Pixie.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class UserProfiles : MonoBehaviour, IUserProfiles
    {
        public IEnumerable<IUserProfile> Profiles { get { return profiles; } }

        public IUserProfile GetBestMatch(UserRoleEnum userRole, UserTeamEnum userTeam, DeviceTypeEnum userDevice)
        {
            // TEMP
            foreach (UserProfile profile in profiles)
            {
                if (profile.UserRole == userRole)
                    return profile;
            }
            return profiles[0];
        }

        [SerializeField]
        private UserProfile[] profiles;
    }
}