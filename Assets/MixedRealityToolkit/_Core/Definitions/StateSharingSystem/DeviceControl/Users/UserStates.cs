using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class UserStates : NetworkBehaviour, IUserStates
    {
        public UserState UserState
        {
            get { return userState; }
            set
            {
                if (!(isLocalPlayer | isServer))
                    throw new System.Exception("Can't set states unless server or player object is local user.");

                userState = value;
            }
        }

        public HandState HandState
        {
            get { return handState; }
            set
            {
                if (!(isLocalPlayer | isServer))
                    throw new System.Exception("Can't set states unless server or player object is local user.");

                handState = value;
            }
        }

        [Header("Synchronized States")]
        [SyncVar]
        [SerializeField]
        private UserState userState;
        
        [SyncVar]
        [SerializeField]
        private HandState handState;

        private IUserCamera userCamera;
        private ILocalUserObject userObject;
        private float timeLastHandsPush;
        private float timeLastPlayerPush;
        private bool lastDispatchReceived;

        private void OnEnable()
        {
            userCamera = (IUserCamera)gameObject.GetComponent(typeof(IUserCamera));
            userObject = (ILocalUserObject)gameObject.GetComponent(typeof(ILocalUserObject));
        }

        #region Client->Server

        [Command(channel = Globals.UNet.ChannelReliableSequenced)]
        private void Cmd_UpdateLocalState(UserState newState)
        {
            userState = newState;
        }

        [Command(channel = Globals.UNet.ChannelReliableSequenced)]
        private void Cmd_UpdateLocalHandState(HandState newState)
        {
            handState = newState;
        }

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

        private void Update()
        {
            if (!userObject.HasRole)
                return;

            if (isLocalPlayer)
            {
                UpdateLocalUser();
            }
            else
            {
                UpdateRemoteUser();
            }
        }

        private void UpdateLocalUser()
        {
            if (userCamera.HasCamera)
            {
                // Set our culling mask based on player type
                userObject.HeadPos = userCamera.CameraTransform.position;
                userObject.HeadDir = userCamera.CameraTransform.forward;
            }

            #region update hands for winter users

            float pushTime = UnityEngine.Time.unscaledTime;

            /*if (handState.UserRole == UserRoleEnum.Observer)
            {
                // Update our camera parent to zero
                cameraParent.localPosition = Vector3.zero;
                // Update our player's hand positions (only relevant in immersive for now - possibly in HoloLens later)
                HandState newHandState = states.HandState;
                newHandState.LHandPos = leftHandTransform.position;
                newHandState.LHandDir = leftHandTransform.forward;
                newHandState.LHandVel = (newHandState.LHandPos - localHandState.LHandPos).magnitude;

                newHandState.RHandPos = rightHandTransform.position;
                newHandState.RHandDir = rightHandTransform.forward;
                newHandState.RHandVel = (newHandState.RHandPos - localHandState.RHandPos).magnitude;

                if (pushTime > timeLastHandsPush + Globals.UNet.PushIntervalPlayer)
                {
                    timeLastHandsPush = pushTime;
                    // Push these changes to server
                    if (newHandState.IsDifferent(localHandState))
                    {
                        Cmd_UpdateLocalHandState(newHandState);
                    }
                }
            }
            else
            {
                // Update camera parent with player's hight
                cameraParent.localPosition = Vector3.up * info.Height;
            }*/
            #endregion

            #region update head for all users

            // Make sure this is always reset
            userObject.CameraParent.localRotation = Quaternion.identity;

            // Common - update player state and alignment state
            UserState newPlayerState = userState;
            // Update our player object state to reflect transform positions
            newPlayerState.HeadPos = userObject.HeadTransform.position;
            newPlayerState.HeadDir = userObject.HeadTransform.forward;
            newPlayerState.HeadVel = (newPlayerState.HeadPos - userState.HeadPos).magnitude;
            
            if (pushTime > timeLastPlayerPush + Globals.UNet.PushIntervalPlayer)
            {
                timeLastPlayerPush = pushTime;
                if (newPlayerState.IsDifferent(userState))
                {
                    // Push these changes to server
                    Cmd_UpdateLocalState(newPlayerState);
                }
            }

            #endregion
        }

        private void UpdateRemoteUser()
        {
            // We're a slave to player data
            // Update our transforms based on the game state
            if (userState.HeadDir != Vector3.zero)
            {
                userObject.HeadTransform.forward = userState.HeadDir;
            }
            userObject.HeadTransform.localPosition = userState.HeadPos;

            // Update our hand transforms (even if we don't have hands)
            userObject.LeftHandTransform.localPosition = handState.LHandPos;
            if (handState.LHandDir != Vector3.zero)
            {
                userObject.LeftHandTransform.forward = handState.LHandDir;
            }

            userObject.RightHandTransform.localPosition = handState.RHandPos;
            if (handState.RHandDir != Vector3.zero)
            {
                userObject.RightHandTransform.forward = handState.RHandDir;
            }

            // Scene alignment will be applied by player scene alignment classes
        }

        #endregion
    }
}