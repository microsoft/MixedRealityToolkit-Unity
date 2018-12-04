using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class UserObject : MonoBehaviour, IUserObject
    {
        public UserRoleEnum UserRole { get { return userSlotState.UserRole; } }
        public UserTeamEnum UserTeam { get { return userSlotState.UserTeam; } }
        public short UserID { get { return userSlotState.UserID; } }
        public bool Simulated { get { return false; } }
        public bool IsDestroyed { get { return isDestroyed; } }
        public bool IsLocalUser { get { return isLocalUser; } }
        public bool IsAssigned { get { return userSlotState.UserID >= 0; } }
        
        public Transform SceneOffset { get { return sceneOffset; } }
        public Transform SceneAlignment { get { return sceneAlignment; } }
        
        [Header("Camera / scene transforms")]
        [SerializeField]
        private Transform sceneOffset;
        [SerializeField]
        private Transform sceneAlignment;

        private bool isLocalUser;
        private bool isDestroyed;
        private UserSlot userSlotState;
        
        public void AssignUser (UserSlot userSlotState)
        {
            this.userSlotState = userSlotState;
            gameObject.name = "UserObject: " + userSlotState.UserRole + " " + userSlotState.UserID.ToString();
        }

        public void AssignLocalUser(bool isLocalUser)
        {
            this.isLocalUser = isLocalUser;
        }

        private void OnDestroy()
        {
            isDestroyed = true;
        }
    }
}