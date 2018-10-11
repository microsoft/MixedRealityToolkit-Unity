using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserProfiles
    {
        IEnumerable<UserProfile> Profiles { get; }

        UserProfile GetBestMatch(
            UserRoleEnum userRole,
            UserTeamEnum userTeam,
            UserDeviceEnum userDevice);
    }
}