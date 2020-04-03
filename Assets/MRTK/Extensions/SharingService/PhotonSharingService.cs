// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
#else
// When PHOTON_UNITY_NETWORKING is not defined some fields and events cause 'never used' warnings.
#pragma warning disable CS0067
#pragma warning disable CS0414
#endif
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing.Photon
{
    /// <summary>
    /// A photon-based implementation of MRTK's ISharingService.
    /// </summary>
    [MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone | SupportedPlatforms.WindowsUniversal)]
    public class PhotonSharingService : BaseExtensionService,
#if PHOTON_UNITY_NETWORKING
        IConnectionCallbacks,
        IMatchmakingCallbacks,
        IInRoomCallbacks,
        ILobbyCallbacks,
        IOnEventCallback,
#endif
        ISharingService,
        IMixedRealityExtensionService
    {
        public PhotonSharingService(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile)
        {
            sharingServiceProfile = (SharingServiceProfile)profile;
        }

        #region Public events

        /// <inheritdoc />
        public event StatusEvent OnStatusChange;
        /// <inheritdoc />
        public event StatusEvent OnAppRoleSelected;
        /// <inheritdoc />
        public event SubscriptionEvent OnLocalSubscriptionModeChange;
        /// <inheritdoc />
        public event DataEvent OnReceiveData;
        /// <inheritdoc />
        public event DeviceEvent OnDeviceConnected;
        /// <inheritdoc />
        public event DeviceEvent OnDeviceDisconnected;
        /// <inheritdoc />
        public event PingEvent OnLocalDevicePinged;

        #endregion

        #region Public connection props

        /// <inheritdoc />
        public ConnectStatus Status { get; private set; } = ConnectStatus.NotConnected;
        /// <inheritdoc />
        public AppRole AppRole { get; private set; } = AppRole.None;
        /// <inheritdoc />
        public SubscriptionMode LocalSubscriptionMode { get; private set; } = SubscriptionMode.Default;
        /// <inheritdoc />
        public string LobbyName { get; private set; }
        /// <inheritdoc />
        public string RoomName { get; private set; }
        /// <inheritdoc />        /// <inheritdoc />
        public IEnumerable<RoomInfo> AvailableRooms => availableRooms;
        /// <inheritdoc />
        public int NumAvailableRooms => availableRooms.Count;

        #endregion

        #region Public device management props

        /// <inheritdoc />
        public DeviceInfo LocalDevice => localDevice;
        /// <inheritdoc />
        public bool LocalDeviceConnected { get { return localDevice.ID >= 0; } }
        /// <inheritdoc />
        public IEnumerable<DeviceInfo> ConnectedDevices => connectedDevices.Values;
        /// <inheritdoc />
        public int NumConnectedDevices
        {
            get
            {
#if PHOTON_UNITY_NETWORKING
                return playersInRoom.Count;
#else
                return 0;
#endif
            }
        }
        /// <inheritdoc />
        public int NumTimesPinged { get; private set; }
        /// <inheritdoc />
        public float TimeLastPinged { get; private set; }
        #endregion

        #region Private constants

        private const byte setSubscriptionEvent = 1;
        private const byte requestAppRoleEvent = 2;
        private const byte receiveAppRoleEvent = 3;
        private const byte sendDataEvent = 4;
        private const byte pingDeviceEvent = 5;
        private const int delayWhileConnectingToMaster = 500;
        private const int delayWhileConnectingToLobby = 5000;
        private const int delayWhileWaitingForResult = 100;
        private const int delayWhileExitingRoom = 100;
        private const int rejoinRoomError = 32749;

        #endregion

        #region Private fields

        // Profile
        private SharingServiceProfile sharingServiceProfile;

        // Connection
        private CancellationTokenSource connectTokenSource;
        private string lastRoomJoined = string.Empty;
        private bool awaitingAppRoleRequest = false;

        // Subscriptions
        private List<short> localSubscriptionTypes = new List<short>();
        private Dictionary<short, HashSet<short>> subscribedTypes = new Dictionary<short, HashSet<short>>();
        private Dictionary<short, SubscriptionMode> subscriptionModes = new Dictionary<short, SubscriptionMode>();
        private object[] eventDataReceiveSubscriptionMode = new object[3];

        // Data
        private object[] eventDataReceiveData = new object[3];
        private List<int> targetActors = new List<int>();

        // Matchmaking
        private HashSet<RoomInfo> availableRooms = new HashSet<RoomInfo>();
        private RoomConnectResult roomConnectResult = RoomConnectResult.Waiting;

        // Devices
        private Dictionary<short, DeviceInfo> connectedDevices = new Dictionary<short, DeviceInfo>();
        private Dictionary<short, AppRole> deviceIDToAppRole = new Dictionary<short, AppRole>();
        private object[] requestDeviceActionData = new object[4];
        private DeviceInfo localDevice;

        #endregion

        #region Photon private fields

#if PHOTON_UNITY_NETWORKING
        private List<Player> playersInRoom = new List<Player>();
        // Cached event / send options for frequent events
        private RaiseEventOptions sendDataEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,                                  // Only subscribed users receive data.
            CachingOption = EventCaching.DoNotCache,                        // Data is not cached by default. New users must manually synchronize.
        };

        private SendOptions sendDataSendOptions = new SendOptions();
        // We define one typed lobby on initialization.
        TypedLobby typedLobby;
#endif

        #endregion

        #region Public methods

        /// <inheritdoc />
        public Task<bool> FastConnect()
        {
            return FastConnect(new ConnectConfig());
        }

        /// <inheritdoc />
        public Task<bool> FastConnect(ConnectConfig config)
        {
#if PHOTON_UNITY_NETWORKING
            if (connectTokenSource != null)
            {
                connectTokenSource.Cancel();
            }

            connectTokenSource = new CancellationTokenSource();
            return ConnectInternal(config, ConnectStatus.FullyConnected, connectTokenSource.Token);
#else
            return Task.Run<bool>(() => { return true; });
#endif
        }

        /// <inheritdoc />
        public Task<bool> JoinLobby()
        {
#if PHOTON_UNITY_NETWORKING
            if (connectTokenSource != null)
            {
                connectTokenSource.Cancel();
            }

            ConnectConfig config = new ConnectConfig()
            {
                RoomConfig = default(RoomConfig),
                RequestedRole = AppRole.None,
                SubscriptionMode = SubscriptionMode.Default,
                SubscriptionTypes = null,
            };

            connectTokenSource = new CancellationTokenSource();
            return ConnectInternal(config, ConnectStatus.ConnectedToLobby, connectTokenSource.Token);
#else
            return Task.Run<bool>(() => { return true; });
#endif
        }

        /// <inheritdoc />
        public Task<bool> JoinRoom(ConnectConfig config)
        {
#if PHOTON_UNITY_NETWORKING
            if (connectTokenSource != null)
            {
                connectTokenSource.Cancel();
            }

            connectTokenSource = new CancellationTokenSource();
            return ConnectInternal(config, ConnectStatus.FullyConnected, connectTokenSource.Token);
#else
            return Task.Run<bool>(() => { return true; });
#endif
        }

        /// <inheritdoc />
        public void LeaveRoom()
        {
            if (!CheckConnectionAndPlayMode("leave room", ConnectStatus.FullyConnected))
            {
                return;
            }

#if PHOTON_UNITY_NETWORKING
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom(true);
            }
#endif
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!CheckConnectionAndPlayMode("disconnect", ConnectStatus.FullyConnected | ConnectStatus.ConnectedToLobby | ConnectStatus.ConnectedToServer))
            {
                return;
            }

            if (connectTokenSource != null)
            {
                connectTokenSource.Cancel();
                connectTokenSource = null;
            }

#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.Disconnect();
#endif
        }

        /// <inheritdoc />
        public void SendData(SendDataArgs args)
        {
            if (!CheckConnectionAndPlayMode("send data", ConnectStatus.FullyConnected))
            {
                return;
            }

#if PHOTON_UNITY_NETWORKING
            // Figure out who we're supposed to send this to.
            // This could be accomplished with Photon interest groups and listeners, but that approach may not carry over to other back-ends.
            GetTargetActors(args, targetActors);

            if (targetActors.Count > 0)
            {
                sendDataSendOptions.DeliveryMode = (ExitGames.Client.Photon.DeliveryMode)args.DeliveryMode;
                sendDataEventOptions.TargetActors = targetActors.ToArray();

                eventDataReceiveData[0] = args.Type;
                eventDataReceiveData[1] = args.Data;
                eventDataReceiveData[2] = LocalDevice.ID;
                PhotonNetwork.RaiseEvent(sendDataEvent, eventDataReceiveData, sendDataEventOptions, sendDataSendOptions);
            }
#endif
        }

        /// <inheritdoc />
        public void SetLocalSubscriptionMode(SubscriptionMode subscriptionMode, IEnumerable<short> subscriptionTypes = null)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can't set local subscription modes when not in play mode.");
                return;
            }

            if (Status != ConnectStatus.FullyConnected)
            {
                // We can set the local subscription mode, but we can't send anything yet.
                LocalSubscriptionMode = subscriptionMode;
                localSubscriptionTypes.Clear();
                if (subscriptionTypes != null)
                {
                    localSubscriptionTypes.AddRange(subscriptionTypes);
                }
                return;
            }


            // Otherwise, we can update everyone's subscriptions with a buffered RPC call.
            // This ensures that everyone will know what everyone else is subscribed to when they connect.
            List<short> subscriptionTypesList = new List<short>();
            switch (subscriptionMode)
            {
                case SubscriptionMode.All:
                default:
                    break;

                case SubscriptionMode.Manual:
                    if (subscriptionTypes == null)
                    {
                        Debug.LogError("Subscription types cannot be null when subscription is manual");
                    }

                    foreach (short type in subscriptionTypes)
                    {
                        subscriptionTypesList.Add(type);
                    }

                    // If we have NO subscriptions, that's not okay. We have to subscribe to at least one thing.
                    if (subscriptionTypesList.Count == 0)
                    {
                        Debug.LogError("Subscription types cannot be empty when subscription is manual");
                    }
                    break;
            }

#if PHOTON_UNITY_NETWORKING
            eventDataReceiveSubscriptionMode[0] = subscriptionMode;
            eventDataReceiveSubscriptionMode[1] = subscriptionTypesList.ToArray();
            eventDataReceiveSubscriptionMode[2] = (short)PhotonNetwork.LocalPlayer.ActorNumber;

            RaiseEventOptions subscriptionEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All,                                  // Everyone needs to know about subscription events
                CachingOption = EventCaching.AddToRoomCacheGlobal               // Subscription events are cached so new users are aware of them on connect
            };

            SendOptions subscriptionSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable    // Subscription events are very important
            };

            PhotonNetwork.RaiseEvent(setSubscriptionEvent, eventDataReceiveSubscriptionMode, subscriptionEventOptions, subscriptionSendOptions);
#endif
        }

        /// <inheritdoc />
        public bool IsLocalDeviceSubscribedToType(short type)
        {
            if (!CheckConnectionAndPlayMode("check local subscriptions", ConnectStatus.FullyConnected))
            {
                return false;
            }

            return IsDeviceSubscribedToType(LocalDevice.ID, type);
        }

        /// <inheritdoc />
        public bool IsDeviceSubscribedToType(short deviceID, short type)
        {
            SubscriptionMode modeForPlayer = SubscriptionMode.All;
            // If we don't have a subscription entry for this player then they're subscribed by default
            if (!subscriptionModes.TryGetValue(deviceID, out modeForPlayer))
            {
                return true;
            }

            switch (modeForPlayer)
            {
                case SubscriptionMode.All:
                default:
                    return true;

                case SubscriptionMode.Manual:
                    if (subscribedTypes.TryGetValue(deviceID, out HashSet<short> subscriptions))
                    {
                        return subscriptions.Contains(type);
                    }
                    else
                    {
                        Debug.LogWarning("Warning: Subscription type is set to manual but no types are defined for user " + deviceID);
                        return true;
                    }
            }
        }

        /// <inheritdoc />
        public void SetLocalSubscription(short dataType, bool subscribed)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can't set local subscriptions when not in play mode.");
                return;
            }

            switch (LocalSubscriptionMode)
            {
                default:
                    Debug.LogError("Can't set local subscription when mode is not set to manual.");
                    return;

                case SubscriptionMode.Manual:
                    break;
            }

            bool sendResult = false;
            if (subscribed)
            {
                if (!localSubscriptionTypes.Contains(dataType))
                {
                    localSubscriptionTypes.Add(dataType);
                    sendResult = true;
                }
            }
            else
            {
                if (localSubscriptionTypes.Remove(dataType))
                {
                    sendResult = true;
                }
            }

            if (sendResult)
            {
#if PHOTON_UNITY_NETWORKING
                eventDataReceiveSubscriptionMode[0] = LocalSubscriptionMode;
                eventDataReceiveSubscriptionMode[1] = localSubscriptionTypes.ToArray();
                eventDataReceiveSubscriptionMode[2] = (short)PhotonNetwork.LocalPlayer.ActorNumber;

                RaiseEventOptions subscriptionEventOptions = new RaiseEventOptions()
                {
                    Receivers = ReceiverGroup.All,                                  // Everyone needs to know about subscription events
                    CachingOption = EventCaching.AddToRoomCacheGlobal               // Subscription events are cached so new users are aware of them on connect
                };

                SendOptions subscriptionSendOptions = new SendOptions()
                {
                    DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable    // Subscription events are very important
                };

                PhotonNetwork.RaiseEvent(setSubscriptionEvent, eventDataReceiveSubscriptionMode, subscriptionEventOptions, subscriptionSendOptions);
#endif
            }
        }

        /// <inheritdoc />
        public void PingDevice(short deviceID)
        {
#if PHOTON_UNITY_NETWORKING          
            RaiseEventOptions pingDeviceOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,                            // Ping events don't need to be cached
                TargetActors = new int[] { deviceID }                               // Only send to single target
            };

            SendOptions pingDeviceSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Unreliable,     // Ping events are not critical
            };

            PhotonNetwork.RaiseEvent(pingDeviceEvent, null, pingDeviceOptions, pingDeviceSendOptions);
#endif
        }

        /// <inheritdoc />
        public bool ValidateRoomConfig(RoomConfig config, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(config.Name))
            {
                error = "Room name cannot be empty";
                return false;
            }

            if (config.MaxDevices < 1)
            {
                error = "Max players must be greater than zero";
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            localDevice.ID = -1;
            TimeLastPinged = 0;

#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.AddCallbackTarget(this);
            typedLobby = new TypedLobby(sharingServiceProfile.LobbyName, LobbyType.Default);
#endif
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (sharingServiceProfile.FastConnectOnStartup && Status == ConnectStatus.NotConnected)
            {
                FastConnect();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            // Check for connect status updates
            ConnectStatus newStatus = CheckStatusInternal();
            if (Status != newStatus)
            {
                Status = newStatus;
                OnStatusChange?.Invoke(new StatusEventArgs()
                {
                    AppRole = this.AppRole,
                    Status = this.Status,
                });
            }

#if PHOTON_UNITY_NETWORKING
            // Cache this since it uses Linq
            playersInRoom.Clear();
            playersInRoom.AddRange(PhotonNetwork.PlayerList);
#endif
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.RemoveCallbackTarget(this);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
#endif
        }

        #endregion

        #region Private methods

        private bool CheckConnectionAndPlayMode(string action, ConnectStatus permittedStatus)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can't " + action + " when not in play mode.");
                return false;
            }

            if ((Status & permittedStatus) == 0)
            {
                Debug.LogError("Can't " + action + " when status is " + Status + ", taking no action.");
                return false;
            }

            return true;
        }

        private ConnectStatus CheckStatusInternal()
        {
            RoomName = string.Empty;
            LobbyName = string.Empty;

#if PHOTON_UNITY_NETWORKING
            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.ConnectingToNameServer:
                case ClientState.Authenticating:
                case ClientState.ConnectingToMasterServer:
                    return ConnectStatus.AttemptingToConnect;

                case ClientState.ConnectedToMasterServer:
                case ClientState.JoiningLobby:
                    return ConnectStatus.ConnectedToServer;

                case ClientState.JoinedLobby:
                case ClientState.ConnectingToGameServer:
                case ClientState.ConnectedToGameServer:
                case ClientState.Joining:
                    LobbyName = PhotonNetwork.CurrentLobby.Name;
                    return ConnectStatus.ConnectedToLobby;

                case ClientState.Joined:
                    LobbyName = PhotonNetwork.CurrentLobby.Name;
                    RoomName = PhotonNetwork.CurrentRoom.Name;
                    return ConnectStatus.FullyConnected;

                case ClientState.PeerCreated:
                case ClientState.Disconnected:
                case ClientState.Disconnecting:
                case ClientState.DisconnectingFromGameServer:
                case ClientState.DisconnectingFromMasterServer:
                default:
                    return ConnectStatus.NotConnected;
            }
#else
            return ConnectStatus.NotConnected;
#endif
        }

        #endregion

        #region Photon private methods

#if PHOTON_UNITY_NETWORKING

        private void GetTargetActors(SendDataArgs args, List<int> targetActors)
        {
            targetActors.Clear();

            switch (args.TargetMode)
            {
                case TargetMode.Default:
                default:
                    {
                        // Everyone who is subscribed receives the data including sender.
                        foreach (Player player in PhotonNetwork.PlayerList)
                        {
                            if (!IsDeviceSubscribedToType((short)player.ActorNumber, args.Type))
                                continue;

                            targetActors.Add(player.ActorNumber);
                        }
                    }
                    break;

                case TargetMode.SkipSender:
                    {
                        // Everyone except the local player who is subscribed receives the data including sender.
                        foreach (Player player in PhotonNetwork.PlayerList)
                        {
                            if (!IsDeviceSubscribedToType((short)player.ActorNumber, args.Type) || player.IsLocal)
                                continue;

                            targetActors.Add(player.ActorNumber);
                        }
                    }
                    break;

                case TargetMode.Manual:
                    {
                        if (args.Targets == null)
                        {
                            Debug.LogError("Targets must not be null when send mode is set to " + args.TargetMode);
                            return;
                        }

                        for (int i = 0; i < args.Targets.Length; i++)
                        {
                            bool foundPlayer = false;
                            foreach (Player player in PhotonNetwork.PlayerList)
                            {
                                if (player.ActorNumber == args.Targets[i])
                                {
                                    foundPlayer = true;
                                    targetActors.Add(player.ActorNumber);
                                    break;
                                }
                            }

                            if (!foundPlayer)
                            {
                                Debug.LogError("Couldn't find accompanying player for user ID " + args.Targets[i] + "(via manual target " + args.Targets[i] + ")");
                            }
                        }
                    }
                    break;
            }
        }

        private void ReceiveSubscriptionMode(SubscriptionMode newSubscriptionMode, short[] newSubscriptionTypes, short deviceID)
        {
            subscriptionModes[deviceID] = newSubscriptionMode;

            HashSet<short> stateTypes;
            if (!subscribedTypes.TryGetValue(deviceID, out stateTypes))
            {
                stateTypes = new HashSet<short>();
                subscribedTypes.Add(deviceID, stateTypes);
            }

            stateTypes.Clear();
            switch (newSubscriptionMode)
            {
                case SubscriptionMode.All:
                default:
                    break;

                case SubscriptionMode.Manual:
                    if (newSubscriptionTypes == null)
                    {
                        Debug.LogError("Subscription types cannot be null when subscription is manual");
                    }

                    foreach (short newStateType in newSubscriptionTypes)
                    {
                        stateTypes.Add(newStateType);
                    }

                    if (stateTypes.Count == 0)
                    {
                        // If we have NO subscriptions, that's not okay. We have to subscribe to at least one thing.
                        Debug.LogError("Subscription types cannot be empty when subscription is manual");
                    }
                    break;
            }

            if (deviceID == localDevice.ID)
            {
                LocalSubscriptionMode = newSubscriptionMode;
                localSubscriptionTypes.Clear();
                localSubscriptionTypes.AddRange(newSubscriptionTypes);

                OnLocalSubscriptionModeChange?.Invoke(new SubscriptionEventArgs()
                {
                    Mode = LocalSubscriptionMode,
                    Types = localSubscriptionTypes
                });
            }
        }
        
        private void ReceiveAppRoleRequest(short deviceID, AppRole requestedRole)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Only the master client should deal with app role requests.");
                return;
            }

            AppRole result = AppRole.None;

            // Find our target player
            Player targetPlayer = PhotonNetwork.PlayerList.Where(p => p.ActorNumber == deviceID).First();
            if (targetPlayer == null)
            {
                Debug.LogError("Couldn't find target player " + deviceID + " to set app role - player may have disconnected.");
                return;
            }

            switch (requestedRole)
            {
                case AppRole.None:
                case AppRole.Client:
                    {   // Assign default role based on MasterClient status
                        result = targetPlayer.IsMasterClient ? AppRole.Host : AppRole.Client;
                    }
                    break;

                case AppRole.Host:
                    {   // Request master client role
                        if (!targetPlayer.IsMasterClient)
                        {   // If it doesn't happen this is not considered a failure.
                            if (PhotonNetwork.SetMasterClient(targetPlayer))
                            {
                                result = AppRole.Host;
                            }
                            else
                            {
                                result = AppRole.Client;
                            }
                        }
                        else
                        {
                            result = AppRole.Host;
                        }
                    }
                    break;

                case AppRole.Server:
                    {   // This is more complicated
                        // We have to ensure that nobody else has taken this role yet
                        bool alreadyOccupied = false;
                        foreach (KeyValuePair<short, AppRole> role in deviceIDToAppRole)
                        {
                            if (role.Value == AppRole.Server && role.Key != deviceID)
                            {
                                Debug.LogError("Can't set app role to server, " + role.Key + " has already taken this role.");
                                alreadyOccupied = true;
                                break;
                            }
                        }

                        if (!alreadyOccupied)
                        {
                            if (!targetPlayer.IsMasterClient)
                            {
                                if (!PhotonNetwork.SetMasterClient(targetPlayer))
                                {
                                    Debug.LogError("Couldn't transfer master client to server role.");
                                    break;
                                }
                                else
                                {
                                    result = AppRole.Server;
                                }
                            }
                        }
                    }
                    break;
            }

            RaiseEventOptions appRoleOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.AddToRoomCache,                        // Everyone needs to know if someone else requested this role
                Receivers = ReceiverGroup.All,                                      // Send to everyone
            };

            SendOptions appRoleSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,       // App role events are crucial and must be in sequence
            };

            PhotonNetwork.RaiseEvent(receiveAppRoleEvent, new object[] { deviceID, result }, appRoleOptions, appRoleSendOptions);
        }

        private void ReceiveAppRoleResult(short deviceID, AppRole result)
        {
            deviceIDToAppRole[deviceID] = result;

            // If this was the local player making the request, set our reqested app role to none.
            if (deviceID == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                awaitingAppRoleRequest = false;

                if (result != AppRole)
                {
                    AppRole = result;
                    OnAppRoleSelected?.Invoke(new StatusEventArgs()
                    {
                        AppRole = this.AppRole,
                        Status = this.Status,
                    });
                }
            }
        }

        private async Task<bool> ConnectInternal(ConnectConfig config, ConnectStatus targetStatus, CancellationToken token)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("This can only be called during play mode.");
                return false;
            }

            float timeAttemptStarted = Time.realtimeSinceStartup;
            bool result = false;

            switch (targetStatus)
            {
                case ConnectStatus.ConnectedToServer:
                    {
                        result = await ConnectToServer(token, timeAttemptStarted);
                    }
                    break;

                case ConnectStatus.ConnectedToLobby:
                    {
                        result = await ConnectToServer(token, timeAttemptStarted) && await ConnectToLobby(token, timeAttemptStarted);
                    }
                    break;

                case ConnectStatus.FullyConnected:
                    {
                        // Figure out what our config means
                        RoomConfig roomConfig = config.RoomConfig.IsEmpty ? sharingServiceProfile.DefaultRoomConfig : config.RoomConfig;
                        AppRole requestedAppRole = (config.RequestedRole == AppRole.None) ? sharingServiceProfile.DefaultRequestedRole : config.RequestedRole;

                        // If we've set the subscription mode before connecting, use that instead of our default subscription mode
                        SubscriptionMode currentSubscriptionMode = (LocalSubscriptionMode == SubscriptionMode.Default) ? sharingServiceProfile.DefaultSubscriptionMode : LocalSubscriptionMode;
                        IEnumerable<short> currentSubscriptionTypes = (LocalSubscriptionMode != SubscriptionMode.Manual) ? sharingServiceProfile.DefaultSubscriptionTypes : localSubscriptionTypes;

                        // If the config specifies a subscription mode, use that instead of our current subscription mode
                        SubscriptionMode subscriptionMode = (config.SubscriptionMode == SubscriptionMode.Default) ? currentSubscriptionMode : config.SubscriptionMode;
                        IEnumerable<short> subscriptionTypes = (config.SubscriptionMode != SubscriptionMode.Manual) ? currentSubscriptionTypes : config.SubscriptionTypes;

                        result = await ConnectToServer(token, timeAttemptStarted) &&
                            await ConnectToLobby(token, timeAttemptStarted) &&
                            await ConnectToRoom(config.RoomJoinMode, roomConfig, token, timeAttemptStarted) &&
                            await RequestAppRole(requestedAppRole, token, timeAttemptStarted);

                        if (result)
                        {
                            // We're ready to set our subscription mode now
                            SetLocalSubscriptionMode(subscriptionMode, subscriptionTypes);
                        }
                        else
                        {
                            Debug.LogError("There was a problem connecting to the room.");
                            return false;
                        }
                    }
                    break;

                default:
                    {
                        Debug.LogError(targetStatus + " is not a valid target status.");
                        return false;
                    }
            }

            if (!result)
            {
                // Something went wrong. If we've joined a room, disconnect from it now.
                // We won't fully disconnect from the server or lobby because no error warrants that.
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                    await Task.Delay(delayWhileExitingRoom);
                }

                return false;
            }

            return true;
        }

        private async Task<bool> ConnectToServer(CancellationToken token, float timeAttemptStarted)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {   // Already connected!
                return true;
            }

            if (!PhotonNetwork.ConnectUsingSettings())
            {
                Debug.LogError("Couldn't connect using photon settings.");
                return false;
            }

            while (!PhotonNetwork.IsConnectedAndReady)
            {
                await Task.Delay(delayWhileConnectingToMaster);

                if (token.IsCancellationRequested)
                {   // Callbacks will reset our status
                    PhotonNetwork.Disconnect();
                    return false;
                }

                if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                {   // Callbacks will reset our status
                    Debug.Log("Connect attempt timed out after " + (Time.realtimeSinceStartup - timeAttemptStarted) + " seconds.");
                    PhotonNetwork.Disconnect();
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ConnectToLobby(CancellationToken token, float timeAttemptStarted)
        {
            if (PhotonNetwork.InLobby)
            {   // We're already here!
                return true;
            }

            do
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (!PhotonNetwork.IsConnected)
                {   // We disconnected while attempting to join
                    return false;
                }

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (!PhotonNetwork.JoinLobby(typedLobby))
                    {
                        Debug.LogWarning("Couldn't join lobby " + sharingServiceProfile.LobbyName + ". Trying again in a few seconds...");
                        await Task.Delay(delayWhileConnectingToLobby);
                    }

                    if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                    {
                        Debug.Log("Connect attempt timed out after " + (Time.realtimeSinceStartup - timeAttemptStarted) + " seconds.");
                        PhotonNetwork.Disconnect();
                        return false;
                    }
                }

                await Task.Delay(delayWhileWaitingForResult);
            }
            while (!PhotonNetwork.InLobby);

            return true;
        }

        private async Task<bool> ConnectToRoom(RoomJoinMode joinMode, RoomConfig roomConfig, CancellationToken token, float timeAttemptStarted)
        {
            Debug.Log("Connect to room with join mode " + joinMode);

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.Name == roomConfig.Name)
                {   // We're already here!
                    return true;
                }
                else
                {   // Leave this room before proceeding
                    Debug.Log("Currently in room " + PhotonNetwork.CurrentRoom.Name + " - leaving before attempting to join new room " + roomConfig.Name);
                    PhotonNetwork.LeaveRoom();
                    await Task.Delay(delayWhileExitingRoom);
                }
            }

            switch (joinMode)
            {
                case RoomJoinMode.JoinRejoinCreate:
                    {
                        if (!(await JoinRoom(roomConfig, token, timeAttemptStarted) ||
                            await RejoinRoom(roomConfig, token, timeAttemptStarted) ||
                            await CreateRoom(roomConfig, token, timeAttemptStarted)))
                        {
                            return false;
                        }
                    }
                    break;

                case RoomJoinMode.JoinForceRejoin:
                    {
                        // If we couldn't join room, disconnect entirely, then try to join again
                        if (!await JoinRoom(roomConfig, token, timeAttemptStarted))
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                PhotonNetwork.LeaveRoom(true);
                            }

                            if (!(await JoinLobby() || await JoinRoom(roomConfig, token, timeAttemptStarted)))
                            {
                                return false;
                            }
                        }
                    }
                    break;

                case RoomJoinMode.RejoinOnly:
                    {
                        if (!await RejoinRoom(roomConfig, token, timeAttemptStarted))
                        {
                            return false;
                        }
                    }
                    break;

                case RoomJoinMode.JoinOnly:
                    {
                        if (!await JoinRoom(roomConfig, token, timeAttemptStarted))
                        {
                            return false;
                        }
                    }
                    break;

                case RoomJoinMode.CreateOnly:
                    {
                        if (!await CreateRoom(roomConfig, token, timeAttemptStarted))
                        {
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private async Task<bool> JoinRoom(RoomConfig roomConfig, CancellationToken token, float timeAttemptStarted)
        {
            roomConnectResult = RoomConnectResult.Waiting;
            PhotonNetwork.JoinRoom(roomConfig.Name);

            // Wait for our room callbacks to set this result
            while (roomConnectResult == RoomConnectResult.Waiting)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (!PhotonNetwork.IsConnected)
                {   // We disconnected while attempting to join
                    return false;
                }

                await Task.Delay(delayWhileWaitingForResult);
            }

            // See what happened with our initial attempt
            switch (roomConnectResult)
            {
                case RoomConnectResult.Succeeded:
                    Debug.Log("Successfully created room " + roomConfig.Name);
                    return true;

                default:
                    return false;
            }
        }

        private async Task<bool> RejoinRoom(RoomConfig roomConfig, CancellationToken token, float timeAttemptStarted)
        {
            roomConnectResult = RoomConnectResult.Waiting;
            PhotonNetwork.RejoinRoom(roomConfig.Name);

            // Wait for our room callbacks to set this result
            while (roomConnectResult == RoomConnectResult.Waiting)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (!PhotonNetwork.IsConnected)
                {   // We disconnected while attempting to join
                    return false;
                }

                await Task.Delay(delayWhileWaitingForResult);
            }

            // See what happened with our initial attempt
            switch (roomConnectResult)
            {
                case RoomConnectResult.Succeeded:
                    Debug.Log("Successfully created room " + roomConfig.Name);
                    return true;

                default:
                    return false;
            }
        }

        private async Task<bool> CreateRoom(RoomConfig roomConfig, CancellationToken token, float timeAttemptStarted)
        {
            if (!ValidateRoomConfig(roomConfig, out string error))
            {
                Debug.LogError("Room config is invalid: " + error);
                return false;
            }

            // Create our room options
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CleanupCacheOnLeave = true;
            roomOptions.MaxPlayers = roomConfig.MaxDevices;
            roomOptions.EmptyRoomTtl = roomConfig.ExpireTime;
            roomOptions.IsVisible = roomConfig.VisibleInLobby;
            roomOptions.PlayerTtl = int.MaxValue; // Allow players to remain inactive indefinitely when disconnected
            roomOptions.CustomRoomPropertiesForLobby = roomConfig.LobbyProps;

            if (roomConfig.RoomProps.Length > 0)
            {
                ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
                foreach (RoomProp prop in roomConfig.RoomProps)
                    properties[prop.Key] = prop.Value;

                roomOptions.CustomRoomProperties = properties;
            }

            // Try to join the room and see what happens
            roomConnectResult = RoomConnectResult.Waiting;
            PhotonNetwork.CreateRoom(roomConfig.Name, roomOptions, typedLobby);

            // Wait for our room callbacks to set this result
            while (roomConnectResult == RoomConnectResult.Waiting)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (!PhotonNetwork.IsConnected)
                {   // We disconnected while attempting to join
                    return false;
                }

                await Task.Delay(delayWhileWaitingForResult);
            }

            // See what happened with our initial attempt
            switch (roomConnectResult)
            {
                case RoomConnectResult.Succeeded:
                    Debug.Log("Successfully created room " + roomConfig.Name);
                    return true;

                default:
                    return false;
            }
        }

        private async Task<bool> RequestAppRole(AppRole requestedAppRole, CancellationToken token, float timeAttemptStarted)
        {
            // Send this request to master client via server.
            RaiseEventOptions appRoleOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,                        // No one else needs to know about requests
                Receivers = ReceiverGroup.MasterClient,                         // Let the master client sort this out
            };

            SendOptions appRoleSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,   // App role events are important and must be in sequence                
            };

            awaitingAppRoleRequest = true;
            PhotonNetwork.RaiseEvent(requestAppRoleEvent, new object[] { (short)PhotonNetwork.LocalPlayer.ActorNumber, requestedAppRole }, appRoleOptions, appRoleSendOptions);

            // Wait for a response
            // Once a response is received, it will be set to none
            while (awaitingAppRoleRequest)
            {
                Debug.Log("Waiting for role requested on connect...");

                if (token.IsCancellationRequested)
                {
                    Debug.Log("Cancelled...");
                    return false;
                }

                if (!PhotonNetwork.IsConnected)
                {   // We disconnected while attempting to join
                    return false;
                }

                if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                {   // Callbacks will reset our status
                    Debug.Log("Connect attempt timed out after " + (Time.realtimeSinceStartup - timeAttemptStarted) + " seconds.");
                    PhotonNetwork.Disconnect();
                    return false;
                }

                await Task.Delay(delayWhileWaitingForResult);
            }

            // If our app role is still none, something went wrong.
            if (AppRole == AppRole.None)
            {
                Debug.LogError("Couldn't obtain requested app role " + requestedAppRole);
                return false;
            }

            // Otherwise, everything went great!
            return true;
        }

        private DeviceInfo CreateDeviceInfoFromPlayers(Player[] playerList, int deviceToReturn = -1)
        {
            connectedDevices.Clear();

            DeviceInfo returnDevice = default(DeviceInfo);

            for (int i = 0; i < playerList.Length; i++)
            {
                Player player = playerList[i];

                List<DeviceProp> props = new List<DeviceProp>();
                try
                {
                    foreach (var entry in player.CustomProperties)
                    {
                        props.Add(new DeviceProp()
                        {
                            Key = (string)entry.Key,
                            Value = (string)entry.Value
                        });
                    }
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError("Invalid property type in device properties,");
                }

                DeviceInfo device = new DeviceInfo()
                {
                    ID = (short)player.ActorNumber,
                    Name = player.NickName,
                    IsLocalDevice = player.IsLocal,
                    Props = props.ToArray(),
                };

                if (deviceToReturn == player.ActorNumber)
                {
                    returnDevice = device;
                }

                if (player.IsLocal)
                {
                    localDevice = device;
                }

                connectedDevices.Add(device.ID, device);
            }

            return returnDevice;
        }

#endif

        #endregion

        #region Photon callbacks (used)

#if PHOTON_UNITY_NETWORKING

        // This is where all of our internal events are routed.
        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case setSubscriptionEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        ReceiveSubscriptionMode((SubscriptionMode)data[0], (short[])data[1], (short)data[2]);
                    }
                    break;

                case requestAppRoleEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        ReceiveAppRoleRequest((short)data[0], (AppRole)data[1]);
                    }
                    break;

                case receiveAppRoleEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        ReceiveAppRoleResult((short)data[0], (AppRole)data[1]);
                    }
                    break;

                case sendDataEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        OnReceiveData?.Invoke(new DataEventArgs()
                        {
                            Type = (short)data[0],
                            Data = (byte[])data[1],
                            Sender = (short)data[2],
                        });
                    }
                    break;

                case pingDeviceEvent:
                    {
                        NumTimesPinged++;
                        TimeLastPinged = Time.realtimeSinceStartup;
                        OnLocalDevicePinged?.Invoke();
                    }
                    break;

                default:
                    break;
            }
        }

        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
        {
            // Reset everything to do with our connection
            AppRole = AppRole.None;
            LocalSubscriptionMode = SubscriptionMode.Default;
            localSubscriptionTypes.Clear();
            deviceIDToAppRole.Clear();
            awaitingAppRoleRequest = false;

            Debug.Log("Disconnected from sharing service.");
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
            // Let the new master client handle broadcasting app role results
            if (newMasterClient != PhotonNetwork.LocalPlayer)
            {
                return;
            }

            RaiseEventOptions appRoleOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.AddToRoomCache,                        // Everyone needs to know if someone else requested this role
                Receivers = ReceiverGroup.Others,                                   // Send to everyone except us
            };

            SendOptions appRoleSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,       // App role events are crucial and must be in sequence
            };

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!deviceIDToAppRole.TryGetValue((short)player.ActorNumber, out AppRole currentRole))
                    continue;

                AppRole newAppRole = currentRole;

                switch (currentRole)
                {
                    case AppRole.Server:
                        // Don't do anything with servers
                        continue;

                    case AppRole.Host:
                    case AppRole.Client:
                        newAppRole = player.IsMasterClient ? AppRole.Host : AppRole.Client;
                        break;

                    case AppRole.None:
                        break;
                }

                if (currentRole != newAppRole)
                {
                    PhotonNetwork.RaiseEvent(receiveAppRoleEvent, new object[] { (short)player.ActorNumber, newAppRole }, appRoleOptions, appRoleSendOptions);
                }
            }
        }

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList);
            // Store this so we can rejoin if we're disconnected
            lastRoomJoined = PhotonNetwork.CurrentRoom.Name;
            roomConnectResult = RoomConnectResult.Succeeded;
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList);
            localDevice.ID = -1;
            NumTimesPinged = 0;
            TimeLastPinged = 0;
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to create room: " + message);
            roomConnectResult = RoomConnectResult.Failed;
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
            switch (returnCode)
            {
                case rejoinRoomError:
                    roomConnectResult = RoomConnectResult.FailedShouldRejoin;
                    break;

                default:
                    Debug.Log("Failed to join room: " + message);
                    roomConnectResult = RoomConnectResult.Failed;
                    break;
            }
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.ActorNumber >= short.MaxValue)
            {
                Debug.LogWarning("Actor number exceeds available device ID values. This is highly unusual.");
            }

            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, newPlayer.ActorNumber);
            OnDeviceConnected?.Invoke(info);
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, otherPlayer.ActorNumber);
            OnDeviceDisconnected?.Invoke(info);
        }

        void ILobbyCallbacks.OnRoomListUpdate(List<global::Photon.Realtime.RoomInfo> roomList)
        {
            availableRooms.Clear();

            foreach (var roomInfoPhoton in roomList)
            {
                availableRooms.Add(new RoomInfo()
                {
                    Name = roomInfoPhoton.Name,
                    IsOpen = roomInfoPhoton.IsOpen,
                    MaxDevices = roomInfoPhoton.MaxPlayers,
                    NumDevices = (byte)roomInfoPhoton.PlayerCount,
                });
            }
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) 
        { 
        
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) 
        {
        
        }

        void ILobbyCallbacks.OnLeftLobby()
        {
            availableRooms.Clear();
        }
#endif

        #endregion

        #region Photon callbacks (unused)

#if PHOTON_UNITY_NETWORKING
        void IConnectionCallbacks.OnConnected() { }

        void IConnectionCallbacks.OnConnectedToMaster() { }

        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler) { }

        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage) { }

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) { }

        void IMatchmakingCallbacks.OnCreatedRoom() { }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) { }

        void ILobbyCallbacks.OnJoinedLobby() { }

        void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }
#endif

        #endregion

        #region Helper structs, classes, enums

        private enum RoomConnectResult
        {
            Waiting,
            Succeeded,
            FailedShouldRejoin,
            Failed,
        }

        #endregion
    }
}