// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Networking;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// Controls player behavior (local and remote).
    /// </summary>
    [NetworkSettings(sendInterval = 0.033f)]
    public class PlayerController : NetworkBehaviour, IInputClickHandler
    {
        private static PlayerController _Instance = null;
        /// <summary>
        /// Instance of the PlayerController that represents the local player.
        /// </summary>
        public static PlayerController Instance
        {
            get
            {
                return _Instance;
            }
        }

        /// <summary>
        /// The game object that represents the 'bullet' for 
        /// this player. Must exist in the spawnable prefabs on the
        /// NetworkManager.
        /// </summary>
        public GameObject bullet;

        public bool CanShareAnchors;

        /// <summary>
        /// The transform of the shared world anchor.
        /// </summary>
        private Transform sharedWorldAnchorTransform;

        /// <summary>
        /// The anchor manager.
        /// </summary>
        private UNetAnchorManager anchorManager;

        /// <summary>
        /// The position relative to the shared world anchor.
        /// </summary>
        [SyncVar]
        private Vector3 localPosition;

        /// <summary>
        /// The rotation relative to the shared world anchor.
        /// </summary>
        [SyncVar]
        private Quaternion localRotation;

        /// <summary>
        /// Sets the localPosition and localRotation on clients.
        /// </summary>
        /// <param name="postion">the localPosition to set</param>
        /// <param name="rotation">the localRotation to set</param>
        [Command(channel = 1)]
        public void CmdTransform(Vector3 postion, Quaternion rotation)
        {
            localPosition = postion;
            localRotation = rotation;
        }

        /// <summary>
        /// Tracks if the player associated with the script has found the shared anchor
        /// </summary>
        [SyncVar(hook = "AnchorEstablishedChanged")]
        bool AnchorEstablished;

        /// <summary>
        /// Sent from a local client to the host to update if the shared
        /// anchor has been found.
        /// </summary>
        /// <param name="Established">true if the shared anchor is found</param>
        [Command]
        private void CmdSendAnchorEstablished(bool Established)
        {
            AnchorEstablished = Established;
            if (Established && SharesSpatialAnchors && !isLocalPlayer)
            {
                Debug.Log("remote device likes the anchor");
#if UNITY_WSA
                anchorManager.AnchorFoundRemotely();
#endif
            }
        }

        /// <summary>
        /// Called when the anchor is either lost or found
        /// </summary>
        /// <param name="update">true if the anchor is found</param>
        void AnchorEstablishedChanged(bool update)
        {
            Debug.LogFormat("AnchorEstablished for {0} was {1} is now {2}", PlayerName, AnchorEstablished, update);
            AnchorEstablished = update;
            // only draw the mesh for the player if the anchor is found.
            GetComponentInChildren<MeshRenderer>().enabled = update;
        }

        /// <summary>
        /// Tracks the player name.
        /// </summary>
        [SyncVar(hook = "PlayerNameChanged")]
        string PlayerName;

        /// <summary>
        /// Called to set the player name
        /// </summary>
        /// <param name="playerName">The name to update to</param>
        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            PlayerName = playerName;
        }

        /// <summary>
        /// Called when the player name changes.
        /// </summary>
        /// <param name="update">the updated name</param>
        void PlayerNameChanged(string update)
        {
            Debug.LogFormat("Player name changing from {0} to {1}", PlayerName, update);
            PlayerName = update;
            // Special case for spectator view
            if (PlayerName.ToLower() == "spectatorviewpc")
            {
                gameObject.SetActive(false);
            }
        }

#pragma warning disable 0414
        /// <summary>
        /// Keeps track of the player's IP address
        /// </summary>
        [SyncVar(hook = "PlayerIpChanged")]
        string PlayerIp;
#pragma warning restore 0414

        /// <summary>
        /// Called to set the IP address
        /// </summary>
        /// <param name="playerIp"></param>
        [Command]
        private void CmdSetPlayerIp(string playerIp)
        {
            PlayerIp = playerIp;
        }

        /// <summary>
        /// Called when the player IP address changes
        /// </summary>
        /// <param name="update">The updated IP address</param>
        void PlayerIpChanged(string update)
        {
            PlayerIp = update;
        }

        /// <summary>
        /// Tracks if the player can share spatial anchors
        /// </summary>
        [SyncVar(hook = "SharesAnchorsChanged")]
        public bool SharesSpatialAnchors;

        /// <summary>
        /// Called to update if the player can share spatial anchors.
        /// </summary>
        /// <param name="canShareAnchors">True if the device can share spatial anchors.</param>
        [Command]
        private void CmdSetCanShareAnchors(bool canShareAnchors)
        {
            Debug.Log("CMDSetCanShare " + canShareAnchors);
            SharesSpatialAnchors = canShareAnchors;
        }

        /// <summary>
        /// Called when the ability to share spatial anchors changes
        /// </summary>
        /// <param name="update">True if the device can share spatial anchors.</param>
        void SharesAnchorsChanged(bool update)
        {
            SharesSpatialAnchors = update;
            Debug.LogFormat("{0} {1} share", PlayerName, SharesSpatialAnchors ? "does" : "does not");
        }

        /// <summary>
        /// Script that handles finding, creating, and joining sessions.
        /// </summary>
        private NetworkDiscoveryWithAnchors networkDiscovery;

        void Awake()
        {
            networkDiscovery = NetworkDiscoveryWithAnchors.Instance;
            anchorManager = UNetAnchorManager.Instance;
        }

        private void Start()
        {
            if (SharedCollection.Instance == null)
            {
                Debug.LogError("This script required a SharedCollection script attached to a GameObject in the scene");
                Destroy(this);
                return;
            }

            if (isLocalPlayer)
            {
                // If we are the local player then we want to have airtaps 
                // sent to this object so that projectiles can be spawned.
                InputManager.Instance.AddGlobalListener(gameObject);
                InitializeLocalPlayer();
            }
            else
            {
                Debug.Log("remote player");
                GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                AnchorEstablishedChanged(AnchorEstablished);
                SharesAnchorsChanged(SharesSpatialAnchors);
            }

            sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;
            transform.SetParent(sharedWorldAnchorTransform);
        }

        private void Update()
        {
            // If we aren't the local player, we just need to make sure that the position of this object is set properly
            // so that we properly render their avatar in our world.
            if (!isLocalPlayer && string.IsNullOrEmpty(PlayerName) == false)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, 0.3f);
                transform.localRotation = localRotation;
                return;
            }

            if (!isLocalPlayer)
            {
                return;
            }

            // if our anchor established state has changed, update everyone
            if (AnchorEstablished != anchorManager.AnchorEstablished)
            {
                CmdSendAnchorEstablished(anchorManager.AnchorEstablished);
            }

            // if our anchor isn't established, we shouldn't bother sending transforms.
            if (AnchorEstablished == false)
            {
                return;
            }

            // if we are the remote player then we need to update our worldPosition and then set our 
            // local (to the shared world anchor) position for other clients to update our position in their world.
            transform.position = CameraCache.Main.transform.position;
            transform.rotation = CameraCache.Main.transform.rotation;

            // For UNET we use a command to signal the host to update our local position
            // and rotation
            CmdTransform(transform.localPosition, transform.localRotation);
        }

        /// <summary>
        /// Sets up all of the local player information
        /// </summary>
        private void InitializeLocalPlayer()
        {
            if (isLocalPlayer)
            {
                Debug.Log("Setting instance for local player ");
                _Instance = this;
                Debug.LogFormat("Set local player name {0} IP {1}", networkDiscovery.broadcastData, networkDiscovery.LocalIp);
                CmdSetPlayerName(networkDiscovery.broadcastData);
                CmdSetPlayerIp(networkDiscovery.LocalIp);
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
                CanShareAnchors = !UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque;
#else
                CanShareAnchors = !Application.isEditor && UnityEngine.VR.VRDevice.isPresent;
#endif
#endif
                Debug.LogFormat("local player {0} share anchors ", (CanShareAnchors ? "does not" : "does"));
                CmdSetCanShareAnchors(CanShareAnchors);
            }
        }

        private void OnDestroy()
        {
            if (isLocalPlayer)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        /// <summary>
        /// Called when the local player starts.  In general the side effect should not be noticed
        /// as the players' avatar is always rendered on top of their head.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        }

        /// <summary>
        /// Called on the host when a bullet needs to be added. 
        /// This will 'spawn' the bullet on all clients, including the 
        /// client on the host.
        /// </summary>
        [Command]
        void CmdFire()
        {
            Vector3 bulletDir = transform.forward;
            Vector3 bulletPos = transform.position + bulletDir * 1.5f;

            // The bullet needs to be transformed relative to the shared anchor.
            GameObject nextBullet = (GameObject)Instantiate(bullet, sharedWorldAnchorTransform.InverseTransformPoint(bulletPos), Quaternion.Euler(bulletDir));
            nextBullet.GetComponentInChildren<Rigidbody>().velocity = bulletDir * 1.0f;
            NetworkServer.Spawn(nextBullet);

            // Clean up the bullet in 8 seconds.
            Destroy(nextBullet, 8.0f);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (isLocalPlayer)
            {
                CmdFire();
            }
        }

        [Command]
        private void CmdSendSharedTransform(GameObject target, Vector3 pos, Quaternion rot)
        {
            UNetSharedHologram ush = target.GetComponent<UNetSharedHologram>();
            ush.CmdTransform(pos, rot);
        }

        /// <summary>
        /// For sending transforms for holograms which do not frequently change.
        /// </summary>
        /// <param name="target">The shared hologram</param>
        /// <param name="pos">position relative to the shared anchor</param>
        /// <param name="rot">rotation relative to the shared anchor</param>
        public void SendSharedTransform(GameObject target, Vector3 pos, Quaternion rot)
        {
            if (isLocalPlayer)
            {
                CmdSendSharedTransform(target, pos, rot);
            }
        }
    }
}
