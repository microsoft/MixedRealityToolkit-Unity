using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class UserProfile : ScriptableObject
    {
        public UserRoleEnum UserRole { get { return userRole; } }

        public UserTeamEnum UserTeam { get { return userTeam; } }

        public UserDeviceEnum DeviceType { get { return deviceType; } }

        public LayerMask CameraCullingMask { get { return cameraCullingMask; } }

        public string OverlaySceneName { get { return overlayScene.name; } }

        public string RoleSceneName { get { return roleScene.name; } }

        public LightingControlSetting Lighting { get { return lighting; } }

        [SerializeField]
        private UserRoleEnum userRole = UserRoleEnum.Ferrier;
        [SerializeField]
        private UserTeamEnum userTeam = UserTeamEnum.None;
        [SerializeField]
        private UserDeviceEnum deviceType = UserDeviceEnum.Mobile;
        [SerializeField]
        private LayerMask cameraCullingMask;
        [SerializeField]
        private LightingControlSetting lighting;
        [SerializeField]
        private UnityEngine.Object overlayScene;
        [SerializeField]
        private UnityEngine.Object roleScene;
    }
}