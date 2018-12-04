using Pixie.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public interface ICameraProfile
    {
        LayerMask CameraCullingMask { get; }
        LightingControlSetting Lighting { get; }
    }

    public interface IUserProfile : ICameraProfile
    {
        string Name { get; }
        UserRoleEnum UserRole { get; }
        UserTeamEnum UserTeam { get; }
        DeviceTypeEnum DeviceType { get; }
        IEnumerable<string> LaunchSceneNames { get; }
        IEnumerable<string> RoleSceneNames { get; }
    }

    public class UserProfile : ScriptableObject, IUserProfile
    {
        public string Name { get { return name; } }
        public UserRoleEnum UserRole { get { return userRole; } }
        public UserTeamEnum UserTeam { get { return userTeam; } }
        public DeviceTypeEnum DeviceType { get { return deviceType; } }
        public LayerMask CameraCullingMask { get { return cameraCullingMask; } }
        public string ActiveSceneName { get { return activeSceneName; } }
        public IEnumerable<string> LaunchSceneNames { get { return launchSceneNames; } }
        public IEnumerable<string> RoleSceneNames { get { return roleSceneNames; } }
        public LightingControlSetting Lighting { get { return lighting; } }

        [SerializeField]
        private UserRoleEnum userRole = UserRoleEnum.None;
        [SerializeField]
        private UserTeamEnum userTeam = UserTeamEnum.None;
        [SerializeField]
        private DeviceTypeEnum deviceType = DeviceTypeEnum.Mobile;
        [SerializeField]
        private LayerMask cameraCullingMask;
        [SerializeField]
        private LightingControlSetting lighting;

        [Header("Scene names")]
        [SerializeField]
        private string activeSceneName = "Startup";
        [SerializeField]
        private string[] launchSceneNames = new string[] { "Client", "ClientGUI", "SpatialServices" };
        [SerializeField]
        private string[] roleSceneNames = new string[] { "Ferrier" };
    }
}