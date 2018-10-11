using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    /// <summary>
    /// Interface for generic player object.
    /// This is the only client object with authority to execuite commands.
    /// </summary>
    public interface IUserObject : INetworkBehaviour
    {
        UserRoleEnum UserRole { get; }
        UserTeamEnum UserTeam { get; }
        UserDeviceEnum UserDevice { get; }
        sbyte UserNum { get; }
        bool HasRole { get; }
        bool Simulated { get; }
        bool IsDestroyed { get; }

        void AssignUserRole(
            UserRoleEnum userRole, 
            UserTeamEnum userTeam, 
            UserDeviceEnum userDevice,        
            sbyte userNum);

        void RevokeUserRole();

        // local values
        Vector3 HeadPos { get; set; }
        Vector3 HeadRot { get; set; }
        Vector3 LHandPos { get; set; }
        Vector3 LHandRot { get; set; }
        Vector3 RHandPos { get; set; }
        Vector3 RHandRot { get; set; }
        Vector3 HeadDir { get; set; }

        Transform SceneAlignment { get; }

        // object references
        IUserTime UserTime { get; }
        IStatePipeOutput StatePipeOutput { get; }
    }
}