using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class UserObject : NetworkBehaviour, ILocalUserObject
    {
        public UserRoleEnum UserRole { get { return states.UserState.UserRole; } }
        public UserTeamEnum UserTeam { get { return states.UserState.UserTeam; } }
        public UserDeviceEnum UserDevice { get { return states.UserState.UserDevice; } }
        public sbyte UserNum { get { return states.UserState.ItemNum; } }

        public bool HasRole
        {
            get
            {
                return states.UserState.ItemNum >= 0
                    && states.UserState.UserRole != UserRoleEnum.None;
            }
        }

        public bool Simulated { get { return false; } }
        public float Latency { get { return time.Latency; } }

        public IUserTime UserTime { get { return time; } }
        public IUserCamera Camera { get { return cam; } }
        public IUserStates States { get { return states; } }
        public IStatePipeInput StatePipeInput { get { return statePipeInput; } }
        public IStatePipeOutput StatePipeOutput { get { return statePipeOutput; } }
        
        public Transform SceneOffset { get { return sceneOffset; } }
        public Transform SceneAlignment { get { return sceneAlignment; } }
        public Transform CameraParent { get { return cameraParent; } }

        public bool IsDestroyed { get { return isDestroyed; } }

        #region hand / head positions

        public Transform HeadTransform { get { return headTransform; } }
        public Transform RightHandTransform { get { return rightHandTransform; } }
        public Transform LeftHandTransform { get { return leftHandTransform; } }

        // Synced head & hand positions
        // Only the local player authority can set these values
        public Vector3 HeadPos
        {
            get { return headTransform.localPosition; }
            set { if (isLocalPlayer) { headTransform.localPosition = value; } }
        }

        public Vector3 HeadRot
        {
            get { return headTransform.localEulerAngles; }
            set { if (isLocalPlayer) { headTransform.localEulerAngles = value; } }
        }

        public Vector3 LHandPos
        {
            get { return leftHandTransform.localPosition; }
            set { if (isLocalPlayer) { leftHandTransform.localPosition = value; } }
        }

        public Vector3 LHandRot
        {
            get { return leftHandTransform.localEulerAngles; }
            set { if (isLocalPlayer) { leftHandTransform.localEulerAngles = value; } }
        }

        public Vector3 RHandPos
        {
            get { return rightHandTransform.localPosition; }
            set { if (isLocalPlayer) { rightHandTransform.localPosition = value; } }
        }

        public Vector3 RHandRot
        {
            get { return rightHandTransform.localEulerAngles; }
            set { if (isLocalPlayer) { rightHandTransform.localEulerAngles = value; } }
        }

        public Vector3 HeadDir
        {
            get { return headTransform.forward; }
            set { if (isLocalPlayer) { headTransform.forward = value; } }
        }

        #endregion

        [Header("Body / scene transforms")]
        [SerializeField]
        private Transform headTransform;
        [SerializeField]
        private Transform leftHandTransform;
        [SerializeField]
        private Transform rightHandTransform;
        [SerializeField]
        private Transform cameraParent;
        [SerializeField]
        private Transform sceneOffset;
        [SerializeField]
        private Transform sceneAlignment;

        private IUserTime time;
        private IUserCamera cam;
        private IUserStates states;
        private IStatePipeInput statePipeInput;
        private IStatePipeOutput statePipeOutput;

        private bool isDestroyed;

        public override int GetNetworkChannel()
        {
            if (isLocalPlayer)
            {
                return Globals.UNet.ChannelReliableSequenced;
            }
            else
            {
                return Globals.UNet.ChannelUnreliable;
            }

        }

        public override float GetNetworkSendInterval()
        {
            if (isLocalPlayer)
            {
                return Globals.UNet.SendIntervalFast;
            }
            else
            {
                return Globals.UNet.SendIntervalSlow;
            }
        }

        #region initialization

        private void Awake()
        {
            time = (IUserTime)gameObject.GetComponent(typeof(IUserTime));
            cam = (IUserCamera)gameObject.GetComponent(typeof(IUserCamera));
            states = (IUserStates)gameObject.GetComponent(typeof(IUserStates));
            statePipeInput = (IStatePipeInput)gameObject.GetComponent(typeof(IStatePipeInput));
            statePipeOutput = (IStatePipeOutput)gameObject.GetComponent(typeof(IStatePipeOutput));
        }

        private void OnEnable()
        {
            IUserManager users;
            SceneScraper.FindInScenes<IUserManager>(out users);
            users.AddAssignedUserObject(this);
        }

        #endregion

        #region Server->Client

        [Server]
        public void AssignUserRole(
            UserRoleEnum userRole,
            UserTeamEnum userTeam,
            UserDeviceEnum userDevice,
            sbyte userNum)
        {
            UserState userState = states.UserState;

            // Update on server
            userState.UserRole = userRole;
            userState.UserTeam = userTeam;
            userState.ItemNum = userNum;

            states.UserState = userState;

            gameObject.name = "UserObject: " + userState.UserRole.ToString() + " - " + userState.ItemNum.ToString();
            //Debug.Assert(userState.ItemNum >= 0, "Invalid user number.");
            //Debug.Assert(userState.UserRole != UserRoleEnum.None, "User type cannot be unknown.");
            //Debug.Assert(userState.UserDevice != UserDeviceEnum.Unknown, "User device cannot be unknown.");

            IUserManager users;
            SceneScraper.FindInScenes<IUserManager>(out users);
            users.AddAssignedUserObject(this);
            // Update on client
            Target_AssignUserRole(connectionToClient, userRole, userTeam, userDevice, userNum);
        }

        [Server]
        public void RevokeUserRole()
        {
            UserState userState = states.UserState;

            // Revoke on server
            userState.UserRole = UserRoleEnum.None;
            userState.UserTeam = UserTeamEnum.None;
            userState.ItemNum = -1;

            states.UserState = userState;

            gameObject.name = "(Unassigned User)";

            IUserManager users;
            SceneScraper.FindInScenes<IUserManager>(out users);
            users.RevokeAssignment(this);
            // Revoke on client
            Target_RevokeUserRole(connectionToClient);
        }

        [TargetRpc]
        private void Target_AssignUserRole(
            NetworkConnection conn,
            UserRoleEnum userRole,
            UserTeamEnum userTeam,
            UserDeviceEnum userDevice,
            sbyte userNum)
        {
            Debug.Log("Target_AssignUserRole: " + userRole + ", " + userNum);

            UserState userState = states.UserState;

            userState.UserRole = userRole;
            userState.UserTeam = userTeam;
            userState.ItemNum = userNum;

            states.UserState = userState;

            gameObject.name = "UserObject: " + userState.UserRole.ToString() + " - " + userState.ItemNum.ToString();
            // Debug.Assert(userState.ItemNum >= 0, "Invalid user number.");
            // Debug.Assert(userState.UserRole != UserRoleEnum.None, "User type cannot be unknown.");
            // Debug.Assert(userState.UserDevice != UserDeviceEnum.Unknown, "User device cannot be unknown.");

            IUserManager users;
            SceneScraper.FindInScenes<IUserManager>(out users);
            users.AddAssignedUserObject(this);
        }

        [TargetRpc]
        private void Target_RevokeUserRole(NetworkConnection conn)
        {
            Debug.Log("Target_RevokeUserRole");

            UserState userState = states.UserState;

            userState.UserRole = UserRoleEnum.None;
            userState.UserTeam = UserTeamEnum.None;
            userState.ItemNum = -1;

            states.UserState = userState;

            gameObject.name = "(Unassigned User)";

            IUserManager users;
            SceneScraper.FindInScenes<IUserManager>(out users);
            users.RevokeAssignment(this);
        }

        [TargetRpc]
        private void Target_ResetDeviceOrigin(NetworkConnection conn)
        {
            Debug.Log("Target_ResetDeviceOrigin called in " + name);

            sceneOffset.position = transform.position;
            Vector3 euelerAngles = transform.eulerAngles;
            euelerAngles.x = 0f;
            euelerAngles.z = 0f;
            sceneOffset.eulerAngles = euelerAngles;
        }

        [Server]
        public void ResetUserOrigin()
        {
            // Tell the client to reset its device origin
            Target_ResetDeviceOrigin(connectionToClient);
        }

        #endregion

        private void OnDestroy()
        {
            isDestroyed = true;
        }

        #region Gizmos

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (!HasRole)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.TransformPoint(states.UserState.HeadPos), Vector3.one * 0.2f);
            Gizmos.DrawLine(transform.TransformPoint(states.UserState.HeadPos), transform.TransformPoint(states.UserState.HeadPos) + transform.TransformDirection(states.UserState.HeadDir));
            if (UserRole == UserRoleEnum.Observer)
            {
                Gizmos.DrawWireCube(transform.TransformPoint(states.HandState.LHandPos), Vector3.one * 0.2f);
                Gizmos.DrawLine(transform.TransformPoint(states.HandState.LHandPos), transform.TransformPoint(states.HandState.LHandPos) + transform.TransformDirection(states.HandState.LHandDir));
                Gizmos.DrawWireCube(transform.TransformPoint(states.HandState.RHandPos), Vector3.one * 0.2f);
                Gizmos.DrawLine(transform.TransformPoint(states.HandState.RHandPos), transform.TransformPoint(states.HandState.RHandPos) + transform.TransformDirection(states.HandState.RHandDir));
            }
        }

        #endregion
    }
}