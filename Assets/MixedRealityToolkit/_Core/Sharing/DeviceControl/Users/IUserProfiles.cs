using Pixie.Core;
using System.Collections.Generic;

namespace Pixie.DeviceControl.Users
{
    public interface IUserProfiles
    {
        IEnumerable<IUserProfile> Profiles { get; }

        IUserProfile GetBestMatch(
            UserRoleEnum userRole,
            UserTeamEnum userTeam,
            DeviceTypeEnum userDevice);
    }
}