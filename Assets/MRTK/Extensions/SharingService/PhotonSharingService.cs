#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#else
// When Photon is not present some fields and events cause 'never used' warnings.
#pragma warning disable CS0067
#pragma warning disable CS0414
#endif
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
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
        public event ConnectionEvent OnConnect;
        /// <inheritdoc />
        public event ConnectionEvent OnDisconnect;
        /// <inheritdoc />
        public event ConnectionEvent OnAppRoleSelected;
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
        public SubscriptionModeEnum LocalSubscriptionMode { get; private set; } = SubscriptionModeEnum.Default;
        /// <inheritdoc />
        public string LobbyName { get; private set; }
        /// <inheritdoc />
        public string RoomName { get; private set; }

        #endregion

        #region Public device management props

        /// <inheritdoc />
        public short LocalDeviceID => localDeviceID;
        /// <inheritdoc />
        public bool LocalDeviceConnected { get { return localDeviceID >= 0; } }
        /// <inheritdoc />
        public IEnumerable<short> ConnectedDevices
        {
#if PHOTON_UNITY_NETWORKING
            get
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (userIDToDeviceIDLookup.TryGetValue(player.UserId, out short deviceID))
                    {
                        yield return deviceID;
                    }
                }
            }
#else
            get
            {
                yield break;
            }
#endif
        }
        /// <inheritdoc />
        public int NumConnectedDevices => userIDToDeviceIDLookup.Count;

        #endregion

        #region Private data type definitions

        private const byte setSubscriptionEvent = 1;
        private const byte sendDataEvent = 2;
        private const byte pingDeviceEvent = 3;
        private const byte requestAddDeviceEvent = 4;
        private const byte requestRemoveDeviceEvent = 5;

        #endregion

        #region Private fields

        // Profile
        private SharingServiceProfile sharingServiceProfile;

        // Connection
        private Task connectTask;
        private CancellationTokenSource connectTokenSource;
        private AppRole currentRequestedRole = AppRole.None;

        // Subscriptions
        private List<short> localSubscriptionTypes = new List<short>();
        private Dictionary<string, HashSet<short>> subscribedTypes = new Dictionary<string, HashSet<short>>();
        private Dictionary<string, SubscriptionModeEnum> subscriptionModes = new Dictionary<string, SubscriptionModeEnum>();
        private object[] eventDataReceiveSubscriptionMode = new object[3];

        // Data
        private object[] eventDataReceiveData = new object[3];
        private List<int> targetActors = new List<int>();

        // Devices
        private Queue<UnhandledLocalEvent> unhandledLocalEvents = new Queue<UnhandledLocalEvent>();
        private Queue<UnhandledRemoteEvent> unhandledRemoteEvents = new Queue<UnhandledRemoteEvent>();
        private Dictionary<string, short> userIDToDeviceIDLookup = new Dictionary<string, short>();
        private Dictionary<short, string> deviceIdToUserIDLookup = new Dictionary<short, string>();
        private object[] requestDeviceActionData = new object[4];
        private short nextDeviceID = 1;
        private short localDeviceID = -1;

        #endregion

        #region Photon private fields

#if PHOTON_UNITY_NETWORKING
        // Cached event / send options for frequent events
        private RaiseEventOptions sendDataEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,                                  // Only subscribed users receive data.
            CachingOption = EventCaching.DoNotCache,                        // Data is not cached by default. New users must manually synchronize.
        };

        private SendOptions sendDataSendOptions = new SendOptions();
#endif

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void Connect()
        {
            ConnectCustom(new ConnectConfig()
            {
                LobbyName = string.Empty,
                RoomName = string.Empty,
                RequestedRole = AppRole.None,
                SubscriptionMode = SubscriptionModeEnum.Default,
                SubscriptionTypes = null,
            });
        }

        /// <inheritdoc />
        public void ConnectCustom(ConnectConfig config)
        {
            if (!CheckConnectionAndPlayMode("connect", ConnectStatus.NotConnected))
            {
                return;
            }
#if PHOTON_UNITY_NETWORKING
            connectTokenSource = new CancellationTokenSource();
            connectTask = ConnectInternalAsync(config, connectTokenSource.Token);
#endif
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!CheckConnectionAndPlayMode("disconnect", ConnectStatus.Connected | ConnectStatus.Connecting))
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
            if (!CheckConnectionAndPlayMode("send data", ConnectStatus.Connected))
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
                eventDataReceiveData[2] = LocalDeviceID;
                PhotonNetwork.RaiseEvent(sendDataEvent, eventDataReceiveData, sendDataEventOptions, sendDataSendOptions);
            }
#endif
        }

        /// <inheritdoc />
        public void SetLocalSubscriptionMode(SubscriptionModeEnum subscriptionMode, IEnumerable<short> subscriptionTypes = null)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can't set local subscription modes when not in play mode.");
                return;
            }

            if (Status != ConnectStatus.Connected)
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
            List<int> subscriptionTypesList = new List<int>();
            switch (subscriptionMode)
            {
                case SubscriptionModeEnum.All:
                default:
                    break;

                case SubscriptionModeEnum.Manual:
                    if (subscriptionTypes == null)
                    {
                        Debug.LogError("Subscription types cannot be null when subscription is manual");
                    }

                    foreach (int type in subscriptionTypes)
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
            eventDataReceiveSubscriptionMode[2] = PhotonNetwork.LocalPlayer.UserId;

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
            if (!CheckConnectionAndPlayMode("check local subscriptions", ConnectStatus.Connected))
            {
                return false;
            }

#if PHOTON_UNITY_NETWORKING
            return IsPhotonPlayerSubscribedToType(PhotonNetwork.LocalPlayer.UserId, type);
#else
            return true;
#endif
        }

        /// <inheritdoc />
        public bool IsDeviceSubscribedToType(short deviceID, short type)
        {
            if (!deviceIdToUserIDLookup.TryGetValue(deviceID, out string userID))
            {
                Debug.LogError("Device ID " + deviceID + " not recognized.");
                return false;
            }

            return IsPhotonPlayerSubscribedToType(userID, type);
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

                case SubscriptionModeEnum.Manual:
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
                eventDataReceiveSubscriptionMode[2] = PhotonNetwork.LocalPlayer.UserId;

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
            // Find the user id associated with this device ID
            if (!deviceIdToUserIDLookup.TryGetValue(deviceID, out string userID))
            {
                Debug.LogError("Device ID " + deviceID + " not recognized.");
                return;
            }

#if PHOTON_UNITY_NETWORKING
            Player targetPlayer = null;
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                if (player.UserId == userID)
                {
                    targetPlayer = player;
                    break;
                }
            }

            if (targetPlayer == null)
            {
                Debug.LogError("Couldn't ping deviceID " + deviceID + " - no accompanying player found.");
                return;
            }

            RaiseEventOptions pingDeviceOptions = new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,                            // Ping events don't need to be cached
                TargetActors = new int[] { targetPlayer.ActorNumber }               // Only send to single target
            };

            SendOptions pingDeviceSendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Unreliable,     // Ping events are not critical
            };

            PhotonNetwork.RaiseEvent(pingDeviceEvent, null, pingDeviceOptions, pingDeviceSendOptions);
#endif
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            nextDeviceID = 1;
            localDeviceID = -1;
            currentRequestedRole = AppRole.None;

#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.AddCallbackTarget(this);
#endif
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (sharingServiceProfile.AutoConnectOnStartup && Status == ConnectStatus.NotConnected)
            {
                Connect();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            // Monitor our connect task, if it exists.
            if (connectTask != null)
            {
                if (connectTask.IsCompleted)
                {
                    switch (connectTask.Status)
                    {
                        case TaskStatus.Faulted:
                            Debug.LogError("Error when attempting to connect to sharing service.");
                            Debug.LogException(connectTask.Exception);
                            break;

                        default:
                            break;
                    }

                    connectTask = null;
                }
            }
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
#endif
            if (Status != ConnectStatus.NotConnected)
            {
                Disconnect();
            }
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

        private bool IsPhotonPlayerSubscribedToType(string userID, short type)
        {
            SubscriptionModeEnum modeForPlayer = SubscriptionModeEnum.All;
            // If we don't have a subscription entry for this player then they're subscribed by default
            if (!subscriptionModes.TryGetValue(userID, out modeForPlayer))
            {
                return true;
            }

            switch (modeForPlayer)
            {
                case SubscriptionModeEnum.All:
                default:
                    return true;

                case SubscriptionModeEnum.Manual:
                    if (subscribedTypes.TryGetValue(userID, out HashSet<short> subscriptions))
                    {
                        return subscriptions.Contains(type);
                    }
                    else
                    {
                        Debug.LogWarning("Warning: Subscription type is set to manual but no types are defined for user " + userID);
                        return true;
                    }
            }
        }

        #endregion

        #region photon private methods

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
                            if (!IsPhotonPlayerSubscribedToType(player.UserId, args.Type))
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
                            if (!IsPhotonPlayerSubscribedToType(player.UserId, args.Type) || player.IsLocal)
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
                            if (!deviceIdToUserIDLookup.TryGetValue(args.Targets[i], out string userID))
                            {
                                Debug.LogError("Couldn't find accompanying user ID for manual target " + args.Targets[i]);
                                continue;
                            }

                            bool foundPlayer = false;
                            foreach (Player player in PhotonNetwork.PlayerList)
                            {
                                if (player.UserId == userID)
                                {
                                    foundPlayer = true;
                                    targetActors.Add(player.ActorNumber);
                                    break;
                                }
                            }

                            if (!foundPlayer)
                            {
                                Debug.LogError("Couldn't find accompanying player for user ID " + userID + "(via manual target " + args.Targets[i] + ")");
                            }
                        }
                    }
                    break;
            }
        }

        private void ReceiveSubscriptionMode(SubscriptionModeEnum newSubscriptionMode, short[] newSubscriptionTypes, string userID)
        {
            subscriptionModes[userID] = newSubscriptionMode;

            HashSet<short> stateTypes;
            if (!subscribedTypes.TryGetValue(userID, out stateTypes))
            {
                stateTypes = new HashSet<short>();
                subscribedTypes.Add(userID, stateTypes);
            }

            stateTypes.Clear();
            switch (newSubscriptionMode)
            {
                case SubscriptionModeEnum.All:
                default:
                    break;

                case SubscriptionModeEnum.Manual:
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

            if (userID == PhotonNetwork.LocalPlayer.UserId)
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

        private async Task ConnectInternalAsync(ConnectConfig config, CancellationToken token)
        {
            string lobbyName = string.IsNullOrEmpty(config.LobbyName) ? sharingServiceProfile.DefaultLobbyName : config.LobbyName;
            string roomName = string.IsNullOrEmpty(config.RoomName) ? sharingServiceProfile.DefaultRoomName : config.RoomName;
            currentRequestedRole = (config.RequestedRole == AppRole.None) ? sharingServiceProfile.DefaultRequestedRole : config.RequestedRole;

            // If we've set the subscription mode before connecting, use that instead of our default subscription mode
            SubscriptionModeEnum currentSubscriptionMode = (LocalSubscriptionMode == SubscriptionModeEnum.Default) ? sharingServiceProfile.DefaultSubscriptionMode : LocalSubscriptionMode;
            IEnumerable<short> currentSubscriptionTypes = (LocalSubscriptionMode != SubscriptionModeEnum.Manual) ? sharingServiceProfile.DefaultSubscriptionTypes : localSubscriptionTypes;

            // If the config specifies a subscription mode, use that instead of our current subscription mode
            SubscriptionModeEnum subscriptionMode = (config.SubscriptionMode == SubscriptionModeEnum.Default) ? currentSubscriptionMode : config.SubscriptionMode;
            IEnumerable<short> subscriptionTypes = (config.SubscriptionMode != SubscriptionModeEnum.Manual) ? currentSubscriptionTypes : config.SubscriptionTypes;

            float timeAttemptStarted = Time.realtimeSinceStartup;

            Status = ConnectStatus.Connecting;
            AppRole = AppRole.None;
            LobbyName = string.Empty;
            RoomName = string.Empty;

            if (!PhotonNetwork.ConnectUsingSettings())
            {
                Debug.LogError("Couldn't connect using photon settings.");
                Status = ConnectStatus.NotConnected;
                return;
            }

            TypedLobby typedLobby = new TypedLobby(lobbyName, LobbyType.Default);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;

            while (!PhotonNetwork.IsConnectedAndReady)
            {
                await Task.Delay(500);

                if (token.IsCancellationRequested)
                {
                    PhotonNetwork.Disconnect();
                    return;
                }

                if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                {
                    Debug.Log("Connect attempt timed out.");
                    PhotonNetwork.Disconnect();
                    return;
                }
            }

            do
            {
                if (token.IsCancellationRequested)
                {
                    PhotonNetwork.Disconnect();
                    return;
                }

                Debug.Log("Trying to join lobby");
                if (!PhotonNetwork.JoinLobby(typedLobby))
                {
                    Debug.LogError("Couldn't join lobby " + lobbyName + ". Trying again in a few seconds...");
                    await Task.Delay(5000);
                }

                if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                {
                    Debug.Log("Connect attempt timed out.");
                    PhotonNetwork.Disconnect();
                    return;
                }

                await Task.Delay(1000);
            }
            while (!PhotonNetwork.InLobby);

            do
            {
                if (token.IsCancellationRequested)
                {
                    PhotonNetwork.Disconnect();
                    return;
                }

                Debug.Log("Trying to create or join room");
                if (!PhotonNetwork.JoinOrCreateRoom(lobbyName, roomOptions, typedLobby))
                {
                    Debug.LogError("Couldn't connect to room " + roomName + ". Trying again in a few seconds...");
                    await Task.Delay(5000);
                }

                if (Time.realtimeSinceStartup > timeAttemptStarted + sharingServiceProfile.ConnectAttemptTimeout)
                {
                    Debug.Log("Connect attempt timed out.");
                    PhotonNetwork.Disconnect();
                    return;
                }

                await Task.Delay(1000);
            }
            while (!PhotonNetwork.InRoom);

            if (currentRequestedRole != AppRole.None)
            {
                AppRole = currentRequestedRole;
            }
            else
            {
                AppRole = PhotonNetwork.IsMasterClient ? AppRole.Host : AppRole.Client;
            }

            Status = ConnectStatus.Connected;
            LobbyName = lobbyName;
            RoomName = roomName;

            Debug.Log("Connected with app role " + AppRole);

            OnAppRoleSelected?.Invoke(new ConnectEventArgs()
            {
                AppRole = this.AppRole,
                Status = this.Status
            });

            OnConnect?.Invoke(new ConnectEventArgs()
            {
                AppRole = this.AppRole,
                Status = this.Status
            });

            // Set out subscription mode now that we're connected
            SetLocalSubscriptionMode(subscriptionMode, subscriptionTypes);

            // If we have any unhandled device events, deal with those now
            while (unhandledLocalEvents.Count > 0)
            {
                UnhandledLocalEvent e = unhandledLocalEvents.Dequeue();
                Debug.Log("Handling device event " + e.Player.UserId + " " + e.Type);

                switch (e.Type)
                {
                    case UnhandledLocalEvent.TypeEnum.Enter:
                        HandlePlayerEnter(e.Player);
                        break;

                    case UnhandledLocalEvent.TypeEnum.Leave:
                        HandlePlayerLeave(e.Player);
                        break;
                }
            }

            while (unhandledRemoteEvents.Count > 0)
            {
                UnhandledRemoteEvent e = unhandledRemoteEvents.Dequeue();
                Debug.Log("Handling device event " + e.UserID + " " + e.Type);
                switch (e.Type)
                {
                    case UnhandledRemoteEvent.TypeEnum.Add:
                        HandleAddDevice(e.UserID, e.DeviceName, e.DeviceID, e.DeviceProperties);
                        break;

                    case UnhandledRemoteEvent.TypeEnum.Remove:
                        HandleRemoveDevice(e.UserID, e.DeviceName, e.DeviceID, e.DeviceProperties);
                        break;
                }
            }
        }

        private void HandlePlayerEnter(Player player)
        {
            Debug.Log("HandlePlayerEnter: " + player.UserId);

            switch (AppRole)
            {
                case AppRole.None:
                    // Deal with this when we're connected
                    Debug.Log("(Adding as unhandled event, we'll deal with this later.)");
                    unhandledLocalEvents.Enqueue(new UnhandledLocalEvent()
                    {
                        Player = player,
                        Type = UnhandledLocalEvent.TypeEnum.Enter
                    });
                    return;

                case AppRole.Client:
                    // Let the server handle this
                    return;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(player.UserId))
            {
                Debug.LogError("User id is empty - set RoomOptions.PublishUserID to true");
                return;
            }

            short deviceID = -1;
            // If it's the same player joining again, there's no need to generate a new ID
            if (!userIDToDeviceIDLookup.TryGetValue(player.UserId, out deviceID))
            {   // Otherwise, generate a new ID and add it to the lookup
                deviceID = nextDeviceID;
                nextDeviceID++;
            }

            Dictionary<string, string> deviceProperties = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in player.CustomProperties)
                deviceProperties.Add(entry.Key.ToString(), entry.Value.ToString());

            Debug.Log("Device " + deviceID + " has connected.");

            requestDeviceActionData[0] = player.UserId;
            requestDeviceActionData[1] = string.IsNullOrEmpty(player.NickName) ? "Device " + deviceID : player.NickName;
            requestDeviceActionData[2] = deviceID;
            requestDeviceActionData[3] = deviceProperties;

            RaiseEventOptions eventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All,                          // Let everyone know, including ourselves
                CachingOption = EventCaching.AddToRoomCacheGlobal,      // Device add / remove calls do not need to be cached
            };

            SendOptions sendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,
            };

            // Let everyone else know that a device has been added
            // This will invoke OnDeviceConnected for everyone including server
            PhotonNetwork.RaiseEvent(requestAddDeviceEvent, requestDeviceActionData, eventOptions, sendOptions);
        }

        private void HandlePlayerLeave(Player player)
        {
            Debug.Log("HandlePlayerLeave: " + player.UserId);

            switch (AppRole)
            {
                case AppRole.None:
                    Debug.Log("(Adding as unhandled event, we'll deal with this later.)");
                    // Deal with this when we're connected
                    unhandledLocalEvents.Enqueue(new UnhandledLocalEvent()
                    {
                        Player = player,
                        Type = UnhandledLocalEvent.TypeEnum.Leave
                    });
                    return;

                case AppRole.Client:
                    // Let the server handle this
                    return;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(player.UserId))
            {
                Debug.LogError("User id is empty - set RoomOptions.PublishUserID to true");
                return;
            }

            short deviceID = -1;
            if (!userIDToDeviceIDLookup.TryGetValue(player.UserId, out deviceID))
            {
                Debug.LogError("User id has no associated device ID - this should never happen!");
                return;
            }

            Dictionary<string, string> deviceProperties = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in player.CustomProperties)
                deviceProperties.Add(entry.Key.ToString(), entry.Value.ToString());

            Debug.Log("Device " + deviceID + " has disconnected.");

            requestDeviceActionData[0] = player.UserId;
            requestDeviceActionData[1] = string.IsNullOrEmpty(player.NickName) ? "Device " + deviceID : player.NickName;
            requestDeviceActionData[2] = deviceID;
            requestDeviceActionData[3] = deviceProperties;

            RaiseEventOptions eventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All,                          // Let everyone know, including ourselves
                CachingOption = EventCaching.AddToRoomCacheGlobal,      // Device add / remove calls do not need to be cached
            };

            SendOptions sendOptions = new SendOptions()
            {
                DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable,
            };

            // Let everyone else know that a device has been removed
            // This will invoke OnDeviceDisconnected for everyone including server
            PhotonNetwork.RaiseEvent(requestRemoveDeviceEvent, requestDeviceActionData, eventOptions, sendOptions);
        }

        private void RequestAddDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            switch (AppRole)
            {
                case AppRole.None:
                    unhandledRemoteEvents.Enqueue(new UnhandledRemoteEvent()
                    {
                        Type = UnhandledRemoteEvent.TypeEnum.Add,
                        UserID = userID,
                        DeviceName = deviceName,
                        DeviceID = deviceID,
                        DeviceProperties = deviceProperties
                    });
                    return;

                default:
                    break;
            }

            HandleAddDevice(userID, deviceName, deviceID, deviceProperties);
        }

        private void RequestRemoveDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            switch (AppRole)
            {
                case AppRole.None:
                    unhandledRemoteEvents.Enqueue(new UnhandledRemoteEvent()
                    {
                        Type = UnhandledRemoteEvent.TypeEnum.Remove,
                        UserID = userID,
                        DeviceName = deviceName,
                        DeviceID = deviceID,
                        DeviceProperties = deviceProperties
                    });
                    return;

                default:
                    break;
            }

            HandleRemoveDevice(userID, deviceName, deviceID, deviceProperties);
        }

        private void HandleAddDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            Debug.Log("Added device: " + userID + ", " + deviceID);
            userIDToDeviceIDLookup.Add(userID, deviceID);
            deviceIdToUserIDLookup.Add(deviceID, userID);
            bool isLocalDevice = (PhotonNetwork.LocalPlayer.UserId == userID);

            if (isLocalDevice)
            {   // We've been connected
                localDeviceID = deviceID;
            }

            OnDeviceConnected?.Invoke(new DeviceEventArgs()
            {
                DeviceID = deviceID,
                DeviceName = deviceName,
                IsLocalDevice = isLocalDevice,
                Properties = deviceProperties
            });
        }

        private void HandleRemoveDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            Debug.Log("removed device: " + userID + ", " + deviceID);
            userIDToDeviceIDLookup.Remove(userID);
            deviceIdToUserIDLookup.Remove(deviceID);
            bool isLocalDevice = (PhotonNetwork.LocalPlayer.UserId == userID);

            if (isLocalDevice)
            {   // We've been disconnected
                localDeviceID = -1;
            }

            OnDeviceDisconnected?.Invoke(new DeviceEventArgs()
            {
                DeviceID = deviceID,
                DeviceName = deviceName,
                IsLocalDevice = isLocalDevice,
                Properties = deviceProperties
            });
        }

#endif

#endregion

        #region Photon callbacks (used)

#if PHOTON_UNITY_NETWORKING
        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
        {
            // Reset everything to do with our connection
            AppRole = AppRole.None;
            Status = ConnectStatus.NotConnected;
            LobbyName = string.Empty;
            RoomName = string.Empty;
            LocalSubscriptionMode = SubscriptionModeEnum.Default;
            localSubscriptionTypes.Clear();
            currentRequestedRole = AppRole.None;

            Debug.Log("Disconnected from sharing service.");

            OnDisconnect?.Invoke(new ConnectEventArgs()
            {
                Status = this.Status,
                AppRole = AppRole.None,
                Message = cause.ToString()
            });
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
            if (currentRequestedRole != AppRole.None)
            {
                // The current requested role is set, so master client changes will have no effect.
                return;
            }

            AppRole newAppRole = (newMasterClient.UserId == PhotonNetwork.LocalPlayer.UserId) ? AppRole.Host : AppRole.Client;

            if (AppRole != newAppRole)
            {
                AppRole = newAppRole;
                Debug.Log((object)("App role changed in sharing service: " + AppRole));

                OnAppRoleSelected?.Invoke(new ConnectEventArgs()
                {
                    AppRole = this.AppRole,
                    Status = this.Status,
                });
            }
        }

        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case setSubscriptionEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        ReceiveSubscriptionMode((SubscriptionModeEnum)data[0], (short[])data[1], (string)data[2]);
                    }
                    break;

                case sendDataEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        OnReceiveData?.Invoke(new DataEventArgs()
                        {
                            Type = (short)data[0],
                            Data = (byte[])data[1],
                            Sender =(short)data[2],
                        });
                    }
                    break;

                case pingDeviceEvent:
                    {
                        OnLocalDevicePinged?.Invoke();
                    }
                    break;

                case requestAddDeviceEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        RequestAddDevice((string)data[0], (string)data[1], (short)data[2], (Dictionary<string, string>)data[3]);
                    }
                    break;

                case requestRemoveDeviceEvent:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        RequestRemoveDevice((string)data[0], (string)data[1], (short)data[2], (Dictionary<string,string>)data[3]);
                    }
                    break;

                default:
                    break;
            }
        }

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            HandlePlayerEnter(PhotonNetwork.LocalPlayer);
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            HandlePlayerLeave(PhotonNetwork.LocalPlayer);
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            HandlePlayerEnter(newPlayer);
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            HandlePlayerLeave(otherPlayer);
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

        void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) { }

        void IMatchmakingCallbacks.OnCreatedRoom() { }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message) { }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message) { }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) { }
#endif

        #endregion

        #region Helper classes

        /// <summary>
        /// Used to store device connections obtained before OnAppConnect has been called.
        /// </summary>
        private class UnhandledLocalEvent
        {
            public enum TypeEnum
            {
                Leave,
                Enter,
            }

            public TypeEnum Type = TypeEnum.Enter;
#if PHOTON_UNITY_NETWORKING
            public Player Player = null;
#endif
        }

        private class UnhandledRemoteEvent
        {
            public enum TypeEnum
            {
                Add,
                Remove,
            }

            public TypeEnum Type = TypeEnum.Add;
            public string UserID = string.Empty;
            public string DeviceName = string.Empty;
            public short DeviceID = -1;
            public Dictionary<string, string> DeviceProperties = null;
        }

#endregion
    }
}