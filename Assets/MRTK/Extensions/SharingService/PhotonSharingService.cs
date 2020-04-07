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
        public event DeviceEvent OnDeviceUpdated;
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
        public RoomInfo CurrentRoom { get; private set; }
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
        public IEnumerable<DeviceInfo> AvailableDevices => availableDevices.Values;
        /// <inheritdoc />
        public int NumAvailableDevices
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
        private const byte receiveDeviceTypeEvent = 4;
        private const byte sendDataEvent = 5;
        private const byte pingDeviceEvent = 6;
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

        // Matchmaking
        private List<RoomInfo> availableRooms = new List<RoomInfo>();
        private RoomConnectResult roomConnectResult = RoomConnectResult.Waiting;

        // Devices
        private Dictionary<short, DeviceInfo> availableDevices = new Dictionary<short, DeviceInfo>();
        private Dictionary<short, AppRole> deviceIDToAppRole = new Dictionary<short, AppRole>();
        private Dictionary<short, DeviceTypeEnum> deviceIDToDeviceType = new Dictionary<short, DeviceTypeEnum>();
        private HashSet<short> announcedDevices = new HashSet<short>();
        private DeviceInfo localDevice;
        private IDeviceTypeFinder deviceTypeFinder;

        // Event Data
        private object[] eventDataReceiveData = new object[3];
        private object[] eventDataReceiveSubscriptionMode = new object[3];
        private object[] requestDeviceActionData = new object[4];
        private List<int> targetActors = new List<int>();

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

            try
            {
                deviceTypeFinder = (IDeviceTypeFinder)Activator.CreateInstance(sharingServiceProfile.DeviceTypeFinder.Type);
                if (deviceTypeFinder == null)
                {
                    Debug.LogError("Couldn't create instance of device type finder. This service will not function without one.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error when creating instance of device type finder.");
                Debug.LogException(e);
            }

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
            CurrentRoom = RoomInfo.Empty;
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
                    CurrentRoom = CreateRoomInfoFromPhotonRoomInfo(PhotonNetwork.CurrentRoom);
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
            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, deviceID);
            OnDeviceUpdated?.Invoke(info);

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

        private void ReceiveDeviceType(short deviceID, DeviceTypeEnum deviceType)
        {
            deviceIDToDeviceType[deviceID] = deviceType;
            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, deviceID);
            OnDeviceUpdated?.Invoke(info);
        }

        private void SetLocalDeviceType(DeviceTypeEnum deviceType)
        {
            // Send this request to master client via server.
            RaiseEventOptions deviceTypeOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.All,
            };

            SendOptions appRoleSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,
            };

            awaitingAppRoleRequest = true;
            PhotonNetwork.RaiseEvent(receiveDeviceTypeEvent, new object[] { (short)PhotonNetwork.LocalPlayer.ActorNumber, deviceType }, deviceTypeOptions, appRoleSendOptions);
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

                        // Get our device type (this will include editor and config settings)
                        DeviceTypeEnum deviceType = GetDeviceType(config.RequestedDeviceType);

                        // Set photon's nickname using our requested name
                        string deviceName = string.IsNullOrEmpty(config.RequestedName) ? sharingServiceProfile.DefaultRequestedName : config.RequestedName;
                        PhotonNetwork.LocalPlayer.NickName = deviceName;

                        result = await ConnectToServer(token, timeAttemptStarted) &&
                            await ConnectToLobby(token, timeAttemptStarted) &&
                            await ConnectToRoom(config.RoomJoinMode, deviceType, roomConfig, token, timeAttemptStarted) &&
                            await RequestAppRole(requestedAppRole, token, timeAttemptStarted);

                        if (result)
                        {
                            // We're ready to set our subscription mode now
                            SetLocalSubscriptionMode(subscriptionMode, subscriptionTypes);
                            SetLocalDeviceType(deviceType);
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
                    PhotonNetwork.LeaveRoom(true);
                    await Task.Delay(delayWhileExitingRoom);
                }

                return false;
            }

            // At this point, we need to send on connect events for all players who were present before us
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!announcedDevices.Contains((short)player.ActorNumber))
                {
                    DeviceInfo info = CreateDeviceInfoFromPlayer(player);
                    OnDeviceConnected?.Invoke(info);
                }
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

        private async Task<bool> ConnectToRoom(RoomJoinMode joinMode, DeviceTypeEnum deviceType, RoomConfig roomConfig, CancellationToken token, float timeAttemptStarted)
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
                    PhotonNetwork.LeaveRoom(true);
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
            roomOptions.PlayerTtl = roomConfig.DeviceExpireTime;
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

        private DeviceTypeEnum GetDeviceType(DeviceTypeEnum requestedDeviceType)
        {
            // Start by getting our ACTUAL device type
            DeviceTypeEnum deviceType = deviceTypeFinder.GetDeviceType();
#if UNITY_EDITOR
            // If this is the editor and we've specified a different device type, use that instead
            deviceType = (sharingServiceProfile.EditorRequestedDeviceType == DeviceTypeEnum.None) ? deviceType : sharingServiceProfile.EditorRequestedDeviceType;
#endif
            // If we've requested a specific device type when connecting, override both settings and use that instead
            deviceType = (requestedDeviceType == DeviceTypeEnum.None) ? deviceType : requestedDeviceType;
            return deviceType;
        }

        private DeviceInfo CreateDeviceInfoFromPlayers(Player[] playerList, int deviceToReturn = -1)
        {
            availableDevices.Clear();

            DeviceInfo returnDevice = default(DeviceInfo);

            for (int i = 0; i < playerList.Length; i++)
            {
                Player player = playerList[i];
                DeviceInfo device = CreateDeviceInfoFromPlayer(player);

                if (deviceToReturn == player.ActorNumber)
                {
                    returnDevice = device;
                }

                if (player.IsLocal)
                {
                    localDevice = device;
                }

                availableDevices.Add(device.ID, device);
            }

            return returnDevice;
        }

        private DeviceInfo CreateDeviceInfoFromPlayer(Player player)
        {
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
                Debug.LogError("Invalid property type in device properties");
            }

            short deviceID = (short)player.ActorNumber;
            deviceIDToDeviceType.TryGetValue(deviceID, out DeviceTypeEnum deviceType);
            deviceIDToAppRole.TryGetValue(deviceID, out AppRole appRole);

            DeviceInfo device = new DeviceInfo()
            {
                ID = deviceID,
                Name = player.NickName,
                IsLocalDevice = player.IsLocal,
                DeviceType = deviceType,
                AppRole = appRole,
                ConnectionState = player.IsInactive ? DeviceConnectionState.NotConnected : DeviceConnectionState.Connected,
                Props = props.ToArray(),
            };

            return device;
        }

        private RoomInfo CreateRoomInfoFromPhotonRoomInfo(global::Photon.Realtime.RoomInfo roomInfoPhoton)
        {
            List<RoomProp> roomProps = new List<RoomProp>();
            foreach (var customProp in roomInfoPhoton.CustomProperties)
            {
                try
                {
                    roomProps.Add(new RoomProp()
                    {
                        Key = (string)customProp.Key,
                        Value = (string)customProp.Value
                    });
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError("Invalid property type in room properties");
                }
            }

            RoomInfo roomInfo = new RoomInfo()
            {
                Name = roomInfoPhoton.Name,
                IsOpen = roomInfoPhoton.IsOpen,
                MaxDevices = roomInfoPhoton.MaxPlayers,
                NumDevices = (byte)roomInfoPhoton.PlayerCount,
                RoomProps = roomProps,
            };

            return roomInfo;
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

                case receiveDeviceTypeEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        ReceiveDeviceType((short)data[0], (DeviceTypeEnum)data[1]);
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
            deviceIDToDeviceType.Clear();
            awaitingAppRoleRequest = false;

            Debug.Log("Disconnected from sharing service.");
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
            // Let the new master client handle broadcasting app role results
            if (newMasterClient.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log("We are not the master client, so don't bother proceeding.");
                return;
            }

            RaiseEventOptions appRoleOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.AddToRoomCache,                        // Everyone needs to know if someone else requested this role
                Receivers = ReceiverGroup.All,                                      // Send to everyone, including us - we may be a new master client
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

            if (announcedDevices.Add(localDevice.ID))
            {
                OnDeviceConnected?.Invoke(localDevice);
            } 
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            // By the time this has happened, local player's actor number has already been set to -1
            // So store our local device info to send the event with a correct device ID later
            DeviceInfo info = localDevice;

            CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList);
            if (announcedDevices.Remove(localDevice.ID))
            {
                OnDeviceDisconnected?.Invoke(localDevice);
            }

            // Now reset local device ID
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
                    Debug.Log("Didn't join room: " + message);
                    roomConnectResult = RoomConnectResult.Failed;
                    break;
            }
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom " + newPlayer.ActorNumber);

            if (newPlayer.ActorNumber >= short.MaxValue)
            {
                Debug.LogWarning("Actor number exceeds available device ID values. This is highly unusual.");
            }

            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, newPlayer.ActorNumber);
            if (announcedDevices.Add(info.ID))
            {
                OnDeviceConnected?.Invoke(info);
            }
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("OnPlayerLeftRoom " + otherPlayer.ActorNumber);

            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, otherPlayer.ActorNumber);
            if (announcedDevices.Remove(info.ID))
            {
                OnDeviceDisconnected?.Invoke(info);
            }
        }

        void ILobbyCallbacks.OnRoomListUpdate(List<global::Photon.Realtime.RoomInfo> roomList)
        {
            availableRooms.Clear();

            foreach (var roomInfoPhoton in roomList)
            {
                RoomInfo roomInfo = CreateRoomInfoFromPhotonRoomInfo(roomInfoPhoton);
                availableRooms.Add(roomInfo);
            }
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) 
        {
            RoomInfo roomInfo = CreateRoomInfoFromPhotonRoomInfo(PhotonNetwork.CurrentRoom);
            for (int i = 0; i < availableRooms.Count; i++)
            {
                if (availableRooms[i].Name == roomInfo.Name)
                {
                    availableRooms[i] = roomInfo;
                    break;
                }
            }
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) 
        {
            DeviceInfo info = CreateDeviceInfoFromPlayers(PhotonNetwork.PlayerList, targetPlayer.ActorNumber);
            OnDeviceUpdated?.Invoke(info);
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