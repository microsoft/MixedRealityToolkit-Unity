// ----------------------------------------------------------------------------
// <copyright file="LoadBalancingPeer.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Provides operations to use the LoadBalancing and Cloud photon servers.
//   No logic is implemented here.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_3_OR_NEWER
    using UnityEngine;
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClassPun = ExitGames.Client.Photon.SupportClass;
#endif


    /// <summary>
    /// A LoadbalancingPeer provides the operations and enum definitions needed to use the loadbalancing server application which is also used in Photon Cloud.
    /// </summary>
    /// <remarks>
    /// Internally used by PUN.
    /// The LoadBalancingPeer does not keep a state, instead this is done by a LoadBalancingClient.
    /// </remarks>
    internal class LoadBalancingPeer : PhotonPeer
    {

        internal bool IsProtocolSecure
        {
            get { return this.UsedProtocol == ConnectionProtocol.WebSocketSecure; }
        }

        private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>(); // used in OpRaiseEvent() (avoids lots of new Dictionary() calls)


        /// <summary>
        /// Creates a Peer with specified connection protocol. You need to set the Listener before using the peer.
        /// </summary>
        /// <remarks>Each connection protocol has it's own default networking ports for Photon.</remarks>
        /// <param name="protocolType">The preferred option is UDP.</param>
        public LoadBalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
        {
            // this does not require a Listener, so:
            // make sure to set this.Listener before using a peer!
        }

        /// <summary>
        /// Creates a Peer with specified connection protocol and a Listener for callbacks.
        /// </summary>
        public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : this(protocolType)
        {
            this.Listener = listener;
        }

        public virtual bool OpGetRegions(string appId)
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[(byte)ParameterCode.ApplicationId] = appId;

            return this.OpCustom(OperationCode.GetRegions, parameters, true, 0, true);
        }

        /// <summary>
        /// Joins the lobby on the Master Server, where you get a list of RoomInfos of currently open rooms.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <param name="lobby">The lobby join to.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public virtual bool OpJoinLobby(TypedLobby lobby = null)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
            }

            Dictionary<byte, object> parameters = null;
            if (lobby != null && !lobby.IsDefault)
            {
                parameters = new Dictionary<byte, object>();
                parameters[(byte)ParameterCode.LobbyName] = lobby.Name;
                parameters[(byte)ParameterCode.LobbyType] = (byte)lobby.Type;
            }

            return this.OpCustom(OperationCode.JoinLobby, parameters, true);
        }


        /// <summary>
        /// Leaves the lobby on the Master Server.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <returns>If the operation could be sent (requires connection).</returns>
        public virtual bool OpLeaveLobby()
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
            }

            return this.OpCustom(OperationCode.LeaveLobby, null, true);
        }


        /// <summary>Used in the RoomOptionFlags parameter, this bitmask toggles options in the room.</summary>
        enum RoomOptionBit : int
        {
            CheckUserOnJoin = 0x01,  // toggles a check of the UserId when joining (enabling returning to a game)
            DeleteCacheOnLeave = 0x02,  // deletes cache on leave
            SuppressRoomEvents = 0x04,  // suppresses all room events
            PublishUserId = 0x08,  // signals that we should publish userId
            DeleteNullProps = 0x10,  // signals that we should remove property if its value was set to null. see RoomOption to Delete Null Properties
            BroadcastPropsChangeToAll = 0x20,  // signals that we should send PropertyChanged event to all room players including initiator
        }

        private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
        {
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }

            Hashtable gameProperties = new Hashtable();
            gameProperties[GamePropertyKey.IsOpen] = roomOptions.IsOpen;
            gameProperties[GamePropertyKey.IsVisible] = roomOptions.IsVisible;
            gameProperties[GamePropertyKey.PropsListedInLobby] = (roomOptions.CustomRoomPropertiesForLobby == null) ? new string[0] : roomOptions.CustomRoomPropertiesForLobby;
            gameProperties.MergeStringKeys(roomOptions.CustomRoomProperties);
            if (roomOptions.MaxPlayers > 0)
            {
                gameProperties[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
            }
            op[ParameterCode.GameProperties] = gameProperties;


            int flags = 0;  // a new way to send the room options as bitwise-flags
            op[ParameterCode.CleanupCacheOnLeave] = roomOptions.CleanupCacheOnLeave;	// this is actually setting the room's config
            if (roomOptions.CleanupCacheOnLeave)
            {
                flags = flags | (int)RoomOptionBit.DeleteCacheOnLeave;
                gameProperties[GamePropertyKey.CleanupCacheOnLeave] = true;  			// this is only informational for the clients which join
            }

            // in PUN v1.88 and PUN 2, CheckUserOnJoin is set by default:
            flags = flags | (int)RoomOptionBit.CheckUserOnJoin;
            op[ParameterCode.CheckUserOnJoin] = true;

            if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
            {
                op[ParameterCode.PlayerTTL] = roomOptions.PlayerTtl;    // TURNBASED
            }

            if (roomOptions.EmptyRoomTtl > 0)
            {
                op[ParameterCode.EmptyRoomTTL] = roomOptions.EmptyRoomTtl;   //TURNBASED
            }

            if (roomOptions.SuppressRoomEvents)
            {
                flags = flags | (int)RoomOptionBit.SuppressRoomEvents;
                op[ParameterCode.SuppressRoomEvents] = true;
            }
            if (roomOptions.Plugins != null)
            {
                op[ParameterCode.Plugins] = roomOptions.Plugins;
            }
            if (roomOptions.PublishUserId)
            {
                flags = flags | (int)RoomOptionBit.PublishUserId;
                op[ParameterCode.PublishUserId] = true;
            }
            if (roomOptions.DeleteNullProperties)
            {
                flags = flags | (int)RoomOptionBit.DeleteNullProps; // this is only settable as flag
            }

            op[ParameterCode.RoomOptionFlags] = flags;
        }


        /// <summary>
        /// Creates a room (on either Master or Game Server).
        /// The OperationResponse depends on the server the peer is connected to:
        /// Master will return a Game Server to connect to.
        /// Game Server will return the joined Room's data.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        /// If the room is already existing, the OperationResponse will have a returnCode of ErrorCode.GameAlreadyExists.
        /// </remarks>
        public virtual bool OpCreateRoom(EnterRoomParams opParams)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
            }

            Dictionary<byte, object> op = new Dictionary<byte, object>();

            if (!string.IsNullOrEmpty(opParams.RoomName))
            {
                op[ParameterCode.RoomName] = opParams.RoomName;
            }
            if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
            {
                op[ParameterCode.LobbyName] = opParams.Lobby.Name;
                op[ParameterCode.LobbyType] = (byte)opParams.Lobby.Type;
            }

            if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
            {
                op[ParameterCode.Add] = opParams.ExpectedUsers;
            }
            if (opParams.OnGameServer)
            {
                if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
                {
                    op[ParameterCode.PlayerProperties] = opParams.PlayerProperties;
                    op[ParameterCode.Broadcast] = true; // TODO: check if this also makes sense when creating a room?! // broadcast actor properties
                }

                this.RoomOptionsToOpParameters(op, opParams.RoomOptions);
            }

            //UnityEngine.Debug.Log("CreateRoom: " + SupportClassPun.DictionaryToString(op));
            return this.OpCustom(OperationCode.CreateGame, op, true);
        }

        /// <summary>
        /// Joins a room by name or creates new room if room with given name not exists.
        /// The OperationResponse depends on the server the peer is connected to:
        /// Master will return a Game Server to connect to.
        /// Game Server will return the joined Room's data.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        /// If the room is not existing (anymore), the OperationResponse will have a returnCode of ErrorCode.GameDoesNotExist.
        /// Other possible ErrorCodes are: GameClosed, GameFull.
        /// </remarks>
        /// <returns>If the operation could be sent (requires connection).</returns>
        public virtual bool OpJoinRoom(EnterRoomParams opParams)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
            }
            Dictionary<byte, object> op = new Dictionary<byte, object>();

            if (!string.IsNullOrEmpty(opParams.RoomName))
            {
                op[ParameterCode.RoomName] = opParams.RoomName;
            }

            if (opParams.CreateIfNotExists)
            {
                op[ParameterCode.JoinMode] = (byte)JoinMode.CreateIfNotExists;
                if (opParams.Lobby != null)
                {
                    op[ParameterCode.LobbyName] = opParams.Lobby.Name;
                    op[ParameterCode.LobbyType] = (byte)opParams.Lobby.Type;
                }
            }

            if (opParams.RejoinOnly)
            {
                op[ParameterCode.JoinMode] = (byte)JoinMode.RejoinOnly; // changed from JoinMode.JoinOrRejoin
            }

            if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
            {
                op[ParameterCode.Add] = opParams.ExpectedUsers;
            }

            if (opParams.OnGameServer)
            {
                if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
                {
                    op[ParameterCode.PlayerProperties] = opParams.PlayerProperties;
                    op[ParameterCode.Broadcast] = true; // broadcast actor properties
                }

                if (opParams.CreateIfNotExists)
                {
                    this.RoomOptionsToOpParameters(op, opParams.RoomOptions);
                }
            }

            // UnityEngine.Debug.Log("JoinRoom: " + SupportClassPun.DictionaryToString(op));
            return this.OpCustom(OperationCode.JoinGame, op, true);
        }


        /// <summary>
        /// Operation to join a random, available room. Overloads take additional player properties.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// If all rooms are closed or full, the OperationResponse will have a returnCode of ErrorCode.NoRandomMatchFound.
        /// If successful, the OperationResponse contains a gameserver address and the name of some room.
        /// </summary>
        /// <returns>If the operation could be sent currently (requires connection).</returns>
        public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
            }

            Hashtable expectedRoomProperties = new Hashtable();
            expectedRoomProperties.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
            if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
            {
                expectedRoomProperties[GamePropertyKey.MaxPlayers] = opJoinRandomRoomParams.ExpectedMaxPlayers;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (expectedRoomProperties.Count > 0)
            {
                opParameters[ParameterCode.GameProperties] = expectedRoomProperties;
            }

            if (opJoinRandomRoomParams.MatchingType != MatchmakingMode.FillRoom)
            {
                opParameters[ParameterCode.MatchMakingType] = (byte)opJoinRandomRoomParams.MatchingType;
            }

            if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
            {
                opParameters[ParameterCode.LobbyName] = opJoinRandomRoomParams.TypedLobby.Name;
                opParameters[ParameterCode.LobbyType] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
            }

            if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
            {
                opParameters[ParameterCode.Data] = opJoinRandomRoomParams.SqlLobbyFilter;
            }

            if (opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length > 0)
            {
                opParameters[ParameterCode.Add] = opJoinRandomRoomParams.ExpectedUsers;
            }

            // UnityEngine.Debug.LogWarning("OpJoinRandom: " + opParameters.ToStringFull());
            return this.OpCustom(OperationCode.JoinRandomGame, opParameters, true);
        }


        /// <summary>
        /// Leaves a room with option to come back later or "for good".
        /// </summary>
        /// <param name="becomeInactive">Async games can be re-joined (loaded) later on. Set to false, if you want to abandon a game entirely.</param>
        /// <returns>If the opteration can be send currently.</returns>
        public virtual bool OpLeaveRoom(bool becomeInactive)
        {
            Dictionary<byte, object> parameters = null;
            if (becomeInactive)
            {
                parameters = new Dictionary<byte, object>();
                parameters[ParameterCode.IsInactive] = becomeInactive;
            }
            return this.OpCustom(OperationCode.Leave, parameters, true);
        }

        /// <summary>Gets a list of games matching a SQL-like where clause.</summary>
        /// <remarks>
        /// Operation is only available in lobbies of type SqlLobby.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// Returned game list is stored in RoomInfoList.
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby#sql_lobby_type"/>
        /// <param name="lobby">The lobby to query. Has to be of type SqlLobby.</param>
        /// <param name="queryData">The sql query statement.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public virtual bool OpGetGameList(TypedLobby lobby, string queryData)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList()");
            }

            if (lobby == null)
            {
                if (this.DebugOut >= DebugLevel.INFO)
                {
                    this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. Lobby cannot be null.");
                }
                return false;
            }

            if (lobby.Type != LobbyType.SqlLobby)
            {
                if (this.DebugOut >= DebugLevel.INFO)
                {
                    this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. LobbyType must be SqlLobby.");
                }
                return false;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)ParameterCode.LobbyName] = lobby.Name;
            opParameters[(byte)ParameterCode.LobbyType] = (byte)lobby.Type;
            opParameters[(byte)ParameterCode.Data] = queryData;

            return this.OpCustom(OperationCode.GetGameList, opParameters, true);
        }

        /// <summary>
        /// Request the rooms and online status for a list of friends (each client must set a unique username via OpAuthenticate).
        /// </summary>
        /// <remarks>
        /// Used on Master Server to find the rooms played by a selected list of users.
        /// Users identify themselves by using OpAuthenticate with a unique username.
        /// The list of usernames must be fetched from some other source (not provided by Photon).
        ///
        /// The server response includes 2 arrays of info (each index matching a friend from the request):
        /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
        /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
        /// </remarks>
        /// <param name="friendsToFind">Array of friend's names (make sure they are unique).</param>
        /// <returns>If the operation could be sent (requires connection).</returns>
        public virtual bool OpFindFriends(string[] friendsToFind)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (friendsToFind != null && friendsToFind.Length > 0)
            {
                opParameters[ParameterCode.FindFriendsRequestList] = friendsToFind;
            }

            return this.OpCustom(OperationCode.FindFriends, opParameters, true);
        }

        public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
        {
            return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), null);
        }

        /// <summary>
        /// Sets properties of a player / actor.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="actorNr">The payer ID (a.k.a. actorNumber) of the player to attach these properties to.</param>
        /// <param name="actorProperties">The properties to add or update.</param>
        /// <param name="expectedProperties">If set, these must be in the current properties-set (on the server) to set actorProperties: CAS.</param>
        /// <param name="webForward">Set to true, to forward the set properties to a WebHook, defined for this app (in Dashboard).</param>
        /// <returns>If the operation could be sent (requires connection).</returns>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
            }

            if (actorNr <= 0 || actorProperties == null)
            {
                if (this.DebugOut >= DebugLevel.INFO)
                {
                    this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
                }
                return false;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add(ParameterCode.Properties, actorProperties);
            opParameters.Add(ParameterCode.ActorNr, actorNr);
            opParameters.Add(ParameterCode.Broadcast, true);
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            if (webForward)
            {
                opParameters[ParameterCode.EventForward] = true;
            }

            return this.OpCustom((byte)OperationCode.SetProperties, opParameters, true, 0, false);
        }


        protected internal void OpSetPropertyOfRoom(byte propCode, object value)
        {
            Hashtable properties = new Hashtable();
            properties[propCode] = value;
            this.OpSetPropertiesOfRoom(properties, expectedProperties: null, webForward: false);
        }

        public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
        {
            return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), expectedProperties: null, webForward: false);
        }

        /// <summary>
        /// Sets properties of a room.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="gameProperties">The properties to add or update.</param>
        /// <param name="expectedProperties">The properties expected when update occurs. (CAS : "Check And Swap")</param>
        /// <param name="webForward">"WebFlag" to indicate if request should be forwarded as "PathProperties" webhook or not.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add(ParameterCode.Properties, gameProperties);
            opParameters.Add(ParameterCode.Broadcast, true);
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            if (webForward)
            {
                opParameters[ParameterCode.EventForward] = true;
            }

            return this.OpCustom((byte)OperationCode.SetProperties, opParameters, true, 0, false);
        }

        /// <summary>
        /// Sends this app's appId and appVersion to identify this application server side.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        /// This operation makes use of encryption, if that is established before.
        /// See: EstablishEncryption(). Check encryption with IsEncryptionAvailable.
        /// This operation is allowed only once per connection (multiple calls will have ErrorCode != Ok).
        /// </remarks>
        /// <param name="appId">Your application's name or ID to authenticate. This is assigned by Photon Cloud (webpage).</param>
        /// <param name="appVersion">The client's version (clients with differing client appVersions are separated and players don't meet).</param>
        /// <param name="authValues">Contains all values relevant for authentication. Even without account system (external Custom Auth), the clients are allowed to identify themselves.</param>
        /// <param name="regionCode">Optional region code, if the client should connect to a specific Photon Cloud Region.</param>
        /// <param name="getLobbyStatistics">Set to true on Master Server to receive "Lobby Statistics" events.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (getLobbyStatistics)
            {
                // must be sent in operation, even if a Token is available
                opParameters[ParameterCode.LobbyStats] = true;
            }

            // shortcut, if we have a Token
            if (authValues != null && authValues.Token != null)
            {
                opParameters[ParameterCode.Secret] = authValues.Token;
                return this.OpCustom(OperationCode.Authenticate, opParameters, true, (byte)0, false);   // we don't have to encrypt, when we have a token (which is encrypted)
            }


            // without a token, we send a complete op auth

            opParameters[ParameterCode.AppVersion] = appVersion;
            opParameters[ParameterCode.ApplicationId] = appId;

            if (!string.IsNullOrEmpty(regionCode))
            {
                opParameters[ParameterCode.Region] = regionCode;
            }

            if (authValues != null)
            {

                if (!string.IsNullOrEmpty(authValues.UserId))
                {
                    opParameters[ParameterCode.UserId] = authValues.UserId;
                }

                if (authValues.AuthType != CustomAuthenticationType.None)
                {
                    if (!this.IsProtocolSecure && !this.IsEncryptionAvailable)
                    {
                        this.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
                        return false;
                    }

                    opParameters[ParameterCode.ClientAuthenticationType] = (byte) authValues.AuthType;
                    if (!string.IsNullOrEmpty(authValues.Token))
                    {
                        opParameters[ParameterCode.Secret] = authValues.Token;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                        {
                            opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                        }
                        if (authValues.AuthPostData != null)
                        {
                            opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                        }
                    }
                }
            }

            bool sent = this.OpCustom(OperationCode.Authenticate, opParameters, true, (byte) 0, this.IsEncryptionAvailable);
            if (!sent)
            {
                this.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected.");
            }
            return sent;
        }


        /// <summary>
        /// Sends this app's appId and appVersion to identify this application server side.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        /// This operation makes use of encryption, if that is established before.
        /// See: EstablishEncryption(). Check encryption with IsEncryptionAvailable.
        /// This operation is allowed only once per connection (multiple calls will have ErrorCode != Ok).
        /// </remarks>
        /// <param name="appId">Your application's name or ID to authenticate. This is assigned by Photon Cloud (webpage).</param>
        /// <param name="appVersion">The client's version (clients with differing client appVersions are separated and players don't meet).</param>
        /// <param name="authValues">Optional authentication values. The client can set no values or a UserId or some parameters for Custom Authentication by a server.</param>
        /// <param name="regionCode">Optional region code, if the client should connect to a specific Photon Cloud Region.</param>
        /// <param name="encryptionMode"></param>
        /// <param name="expectedProtocol"></param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public virtual bool OpAuthenticateOnce(string appId, string appVersion, AuthenticationValues authValues, string regionCode, EncryptionMode encryptionMode, ConnectionProtocol expectedProtocol)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
            }


            var opParameters = new Dictionary<byte, object>();

            // shortcut, if we have a Token
            if (authValues != null && authValues.Token != null)
            {
                opParameters[ParameterCode.Secret] = authValues.Token;
                return this.OpCustom(OperationCode.AuthenticateOnce, opParameters, true, (byte)0, false);   // we don't have to encrypt, when we have a token (which is encrypted)
            }

            if (encryptionMode == EncryptionMode.DatagramEncryption && expectedProtocol != ConnectionProtocol.Udp)
            {
                Debug.LogWarning("Expected protocol set to UDP, due to encryption mode DatagramEncryption. Changing protocol in PhotonServerSettings from: " + PhotonNetwork.PhotonServerSettings.Protocol);
                PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
                expectedProtocol = ConnectionProtocol.Udp;
            }

            opParameters[ParameterCode.ExpectedProtocol] = (byte)expectedProtocol;
            opParameters[ParameterCode.EncryptionMode] = (byte)encryptionMode;

            opParameters[ParameterCode.AppVersion] = appVersion;
            opParameters[ParameterCode.ApplicationId] = appId;

            if (!string.IsNullOrEmpty(regionCode))
            {
                opParameters[ParameterCode.Region] = regionCode;
            }

            if (authValues != null)
            {
                if (!string.IsNullOrEmpty(authValues.UserId))
                {
                    opParameters[ParameterCode.UserId] = authValues.UserId;
                }

                if (authValues.AuthType != CustomAuthenticationType.None)
                {
                    opParameters[ParameterCode.ClientAuthenticationType] = (byte)authValues.AuthType;
                    if (!string.IsNullOrEmpty(authValues.Token))
                    {
                        opParameters[ParameterCode.Secret] = authValues.Token;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                        {
                            opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                        }
                        if (authValues.AuthPostData != null)
                        {
                            opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                        }
                    }
                }
            }

            return this.OpCustom(OperationCode.AuthenticateOnce, opParameters, true, (byte)0, this.IsEncryptionAvailable);
        }

        /// <summary>
        /// Operation to handle this client's interest groups (for events in room).
        /// </summary>
        /// <remarks>
        /// Note the difference between passing null and byte[0]:
        ///   null won't add/remove any groups.
        ///   byte[0] will add/remove all (existing) groups.
        /// First, removing groups is executed. This way, you could leave all groups and join only the ones provided.
        ///
        /// Changes become active not immediately but when the server executes this operation (approximately RTT/2).
        /// </remarks>
        /// <param name="groupsToRemove">Groups to remove from interest. Null will not remove any. A byte[0] will remove all.</param>
        /// <param name="groupsToAdd">Groups to add to interest. Null will not add any. A byte[0] will add all current.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            if (this.DebugOut >= DebugLevel.ALL)
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (groupsToRemove != null)
            {
                opParameters[(byte)ParameterCode.Remove] = groupsToRemove;
            }
            if (groupsToAdd != null)
            {
                opParameters[(byte)ParameterCode.Add] = groupsToAdd;
            }

            return this.OpCustom((byte)OperationCode.ChangeGroups, opParameters, true, 0);
        }


        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="raiseEventOptions">Contains (slightly) less often used options. If you pass null, the default options will be used.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
        {
            this.opParameters.Clear(); // re-used private variable to avoid many new Dictionary() calls (garbage collection)
            this.opParameters[(byte)ParameterCode.Code] = (byte)eventCode;
            if (customEventContent != null)
            {
                this.opParameters[(byte) ParameterCode.Data] = customEventContent;
            }

            if (raiseEventOptions == null)
            {
                raiseEventOptions = RaiseEventOptions.Default;
            }
            else
            {
                if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
                {
                    this.opParameters[(byte) ParameterCode.Cache] = (byte) raiseEventOptions.CachingOption;
                }
                if (raiseEventOptions.Receivers != ReceiverGroup.Others)
                {
                    this.opParameters[(byte) ParameterCode.ReceiverGroup] = (byte) raiseEventOptions.Receivers;
                }
                if (raiseEventOptions.InterestGroup != 0)
                {
                    this.opParameters[(byte) ParameterCode.Group] = (byte) raiseEventOptions.InterestGroup;
                }
                if (raiseEventOptions.TargetActors != null)
                {
                    this.opParameters[(byte) ParameterCode.ActorList] = raiseEventOptions.TargetActors;
                }
                if (raiseEventOptions.ForwardToWebhook)
                {
                    this.opParameters[(byte) ParameterCode.EventForward] = true; //TURNBASED
                }
            }

            return this.OpCustom((byte) OperationCode.RaiseEvent, this.opParameters, sendReliable, raiseEventOptions.SequenceChannel, raiseEventOptions.Encrypt);
        }


        #if PHOTON_LIB_MIN_4_1_2
        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="raiseEventOptions">Contains (slightly) less often used options. If you pass null, the default options will be used.</param>
        /// <param name="sendOptions">Send options wrap up reliability, sequencing and channel.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            this.opParameters.Clear(); // re-used private variable to avoid many new Dictionary() calls (garbage collection)
            this.opParameters[(byte)ParameterCode.Code] = (byte)eventCode;
            if (customEventContent != null)
            {
                this.opParameters[(byte)ParameterCode.Data] = customEventContent;
            }

            if (raiseEventOptions == null)
            {
                raiseEventOptions = RaiseEventOptions.Default;
            }
            else
            {
                if (sendOptions.Channel != raiseEventOptions.SequenceChannel || sendOptions.Encrypt != raiseEventOptions.Encrypt)
                {
                    // TODO: This should be a one-time warning. 
                    // NOTE: Later on, it will be impossible to mix up SendOptions and RaiseEventOptions, as they won't have overlapping settings.
                    this.Listener.DebugReturn(DebugLevel.WARNING, "You are using RaiseEventOptions and SendOptions with conflicting settings. Please check channel and encryption value.");
                }

                if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
                {
                    this.opParameters[(byte)ParameterCode.Cache] = (byte)raiseEventOptions.CachingOption;
                }
                if (raiseEventOptions.Receivers != ReceiverGroup.Others)
                {
                    this.opParameters[(byte)ParameterCode.ReceiverGroup] = (byte)raiseEventOptions.Receivers;
                }
                if (raiseEventOptions.InterestGroup != 0)
                {
                    this.opParameters[(byte)ParameterCode.Group] = (byte)raiseEventOptions.InterestGroup;
                }
                if (raiseEventOptions.TargetActors != null)
                {
                    this.opParameters[(byte)ParameterCode.ActorList] = raiseEventOptions.TargetActors;
                }
                //if (raiseEventOptions.Flags.HttpForward)
                //{
                //    this.opParameters[(byte)ParameterCode.EventForward] = raiseEventOptions.Flags.WebhookFlags; //TURNBASED
                //}
            }

            return this.SendOperation(OperationCode.RaiseEvent, this.opParameters, sendOptions);
        }
        #endif

        /// <summary>
        /// Internally used operation to set some "per server" settings. This is for the Master Server.
        /// </summary>
        /// <param name="receiveLobbyStats">Set to true, to get Lobby Statistics (lists of existing lobbies).</param>
        /// <returns>False if the operation could not be sent.</returns>
        public virtual bool OpSettings(bool receiveLobbyStats)
        {
            if (this.DebugOut >= DebugLevel.ALL)
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "OpSettings()");
            }

            // re-used private variable to avoid many new Dictionary() calls (garbage collection)
            this.opParameters.Clear();

            // implementation for Master Server:
            if (receiveLobbyStats)
            {
                this.opParameters[(byte)0] = receiveLobbyStats;
            }

            if (this.opParameters.Count == 0)
            {
                // no need to send op in case we set the default values
                return true;
            }
            return this.OpCustom((byte)OperationCode.ServerSettings, this.opParameters, true);
        }
    }



    internal class OpJoinRandomRoomParams
    {
        public Hashtable ExpectedCustomRoomProperties;
        public byte ExpectedMaxPlayers;
        public MatchmakingMode MatchingType;
        public TypedLobby TypedLobby;
        public string SqlLobbyFilter;
        public string[] ExpectedUsers;
    }

    internal class EnterRoomParams
    {
        public string RoomName;
        public RoomOptions RoomOptions;
        public TypedLobby Lobby;
        public Hashtable PlayerProperties;
        public bool OnGameServer = true; // defaults to true! better send more parameter than too few (GS needs all)
        public bool CreateIfNotExists;
        public bool RejoinOnly;
        public string[] ExpectedUsers;
    }


    /// <summary>
    /// ErrorCode defines the default codes associated with Photon client/server communication.
    /// </summary>
    public class ErrorCode
    {
        /// <summary>(0) is always "OK", anything else an error or specific situation.</summary>
        public const int Ok = 0;

        // server - Photon low(er) level: <= 0

        /// <summary>
        /// (-3) Operation can't be executed yet (e.g. OpJoin can't be called before being authenticated, RaiseEvent cant be used before getting into a room).
        /// </summary>
        /// <remarks>
        /// Before you call any operations on the Cloud servers, the automated client workflow must complete its authorization.
        /// In PUN, wait until State is: JoinedLobby (with AutoJoinLobby = true) or ConnectedToMaster (AutoJoinLobby = false)
        /// </remarks>
        public const int OperationNotAllowedInCurrentState = -3;

        /// <summary>(-2) The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
        [Obsolete("Use InvalidOperation.")]
        public const int InvalidOperationCode = -2;

        /// <summary>(-2) The operation you called could not be executed on the server.</summary>
        /// <remarks>
        /// Make sure you are connected to the server you expect.
        ///
        /// This code is used in several cases:
        /// The arguments/parameters of the operation might be out of range, missing entirely or conflicting.
        /// The operation you called is not implemented on the server (application). Server-side plugins affect the available operations.
        /// </remarks>
        public const int InvalidOperation = -2;

        /// <summary>(-1) Something went wrong in the server. Try to reproduce and contact Exit Games.</summary>
        public const int InternalServerError = -1;

        // server - PhotonNetwork: 0x7FFF and down
        // logic-level error codes start with short.max

        /// <summary>(32767) Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).</summary>
        public const int InvalidAuthentication = 0x7FFF;

        /// <summary>(32766) GameId (name) already in use (can't create another). Change name.</summary>
        public const int GameIdAlreadyExists = 0x7FFF - 1;

        /// <summary>(32765) Game is full. This rarely happens when some player joined the room before your join completed.</summary>
        public const int GameFull = 0x7FFF - 2;

        /// <summary>(32764) Game is closed and can't be joined. Join another game.</summary>
        public const int GameClosed = 0x7FFF - 3;

        [Obsolete("No longer used, cause random matchmaking is no longer a process.")]
        public const int AlreadyMatched = 0x7FFF - 4;

        /// <summary>(32762) Not in use currently.</summary>
        public const int ServerFull = 0x7FFF - 5;

        /// <summary>(32761) Not in use currently.</summary>
        public const int UserBlocked = 0x7FFF - 6;

        /// <summary>(32760) Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.</summary>
        public const int NoRandomMatchFound = 0x7FFF - 7;

        /// <summary>(32758) Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.</summary>
        public const int GameDoesNotExist = 0x7FFF - 9;

        /// <summary>(32757) Authorization on the Photon Cloud failed becaus the concurrent users (CCU) limit of the app's subscription is reached.</summary>
        /// <remarks>
        /// Unless you have a plan with "CCU Burst", clients might fail the authentication step during connect.
        /// Affected client are unable to call operations. Please note that players who end a game and return
        /// to the master server will disconnect and re-connect, which means that they just played and are rejected
        /// in the next minute / re-connect.
        /// This is a temporary measure. Once the CCU is below the limit, players will be able to connect an play again.
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int MaxCcuReached = 0x7FFF - 10;

        /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
        /// <remarks>
        /// Some subscription plans for the Photon Cloud are region-bound. Servers of other regions can't be used then.
        /// Check your master server address and compare it with your Photon Cloud Dashboard's info.
        /// https://www.photonengine.com/dashboard
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int InvalidRegion = 0x7FFF - 11;

        /// <summary>
        /// (32755) Custom Authentication of the user failed due to setup reasons (see Cloud Dashboard) or the provided user data (like username or token). Check error message for details.
        /// </summary>
        public const int CustomAuthenticationFailed = 0x7FFF - 12;

        /// <summary>(32753) The Authentication ticket expired. Usually, this is refreshed behind the scenes. Connect (and authorize) again.</summary>
        public const int AuthenticationTicketExpired = 0x7FF1;

        /// <summary>
        /// (32752) A server-side plugin (or webhook) failed to execute and reported an error. Check the OperationResponse.DebugMessage.
        /// </summary>
        public const int PluginReportedError = 0x7FFF - 15;

        /// <summary>
        /// (32751) CreateRoom/JoinRoom/Join operation fails if expected plugin does not correspond to loaded one.
        /// </summary>
        public const int PluginMismatch = 0x7FFF - 16;

        /// <summary>
        /// (32750) for join requests. Indicates the current peer already called join and is joined to the room.
        /// </summary>
        public const int JoinFailedPeerAlreadyJoined = 32750; // 0x7FFF - 17,

        /// <summary>
        /// (32749)  for join requests. Indicates the list of InactiveActors already contains an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedFoundInactiveJoiner = 32749; // 0x7FFF - 18,

        /// <summary>
        /// (32748) for join requests. Indicates the list of Actors (active and inactive) did not contain an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedWithRejoinerNotFound = 32748; // 0x7FFF - 19,

        /// <summary>
        /// (32747) for join requests. Note: for future use - Indicates the requested UserId was found in the ExcludedList.
        /// </summary>
        public const int JoinFailedFoundExcludedUserId = 32747; // 0x7FFF - 20,

        /// <summary>
        /// (32746) for join requests. Indicates the list of ActiveActors already contains an actor with the requested ActorNr or UserId.
        /// </summary>
        public const int JoinFailedFoundActiveJoiner = 32746; // 0x7FFF - 21,

        /// <summary>
        /// (32745)  for SetProerties and Raisevent (if flag HttpForward is true) requests. Indicates the maximum allowd http requests per minute was reached.
        /// </summary>
        public const int HttpLimitReached = 32745; // 0x7FFF - 22,

        /// <summary>
        /// (32744) for WebRpc requests. Indicates the the call to the external service failed.
        /// </summary>
        public const int ExternalHttpCallFailed = 32744; // 0x7FFF - 23,

        /// <summary>
        /// (32742) Server error during matchmaking with slot reservation. E.g. the reserved slots can not exceed MaxPlayers.
        /// </summary>
        public const int SlotError = 32742; // 0x7FFF - 25,

        /// <summary>
        /// (32741) Server will react with this error if invalid encryption parameters provided by token
        /// </summary>
        public const int InvalidEncryptionParameters = 32741; // 0x7FFF - 24,

}


    /// <summary>
    /// Class for constants. These (byte) values define "well known" properties for an Actor / Player.
    /// </summary>
    /// <remarks>
    /// Pun uses these constants internally.
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public class ActorProperties
    {
        /// <summary>(255) Name of a player/actor.</summary>
        public const byte PlayerName = 255; // was: 1

        /// <summary>(254) Tells you if the player is currently in this game (getting events live).</summary>
        /// <remarks>A server-set value for async games, where players can leave the game and return later.</remarks>
        public const byte IsInactive = 254;

        /// <summary>(253) UserId of the player. Sent when room gets created with RoomOptions.PublishUserId = true.</summary>
        public const byte UserId = 253;
    }


    /// <summary>
    /// Class for constants. These (byte) values are for "well known" room/game properties used in Photon Loadbalancing.
    /// </summary>
    /// <remarks>
    /// Pun uses these constants internally.
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public class GamePropertyKey
    {
        /// <summary>(255) Max number of players that "fit" into this room. 0 is for "unlimited".</summary>
        public const byte MaxPlayers = 255;

        /// <summary>(254) Makes this room listed or not in the lobby on master.</summary>
        public const byte IsVisible = 254;

        /// <summary>(253) Allows more players to join a room (or not).</summary>
        public const byte IsOpen = 253;

        /// <summary>(252) Current count of players in the room. Used only in the lobby on master.</summary>
        public const byte PlayerCount = 252;

        /// <summary>(251) True if the room is to be removed from room listing (used in update to room list in lobby on master)</summary>
        public const byte Removed = 251;

        /// <summary>(250) A list of the room properties to pass to the RoomInfo list in a lobby. This is used in CreateRoom, which defines this list once per room.</summary>
        public const byte PropsListedInLobby = 250;

        /// <summary>(249) Equivalent of Operation Join parameter CleanupCacheOnLeave.</summary>
        public const byte CleanupCacheOnLeave = 249;

        /// <summary>(248) Code for MasterClientId, which is synced by server. When sent as op-parameter this is (byte)203. As room property this is (byte)248.</summary>
        /// <remarks>Tightly related to ParameterCode.MasterClientId.</remarks>
        public const byte MasterClientId = (byte)248;

        /// <summary>(247) Code for ExpectedUsers in a room. Matchmaking keeps a slot open for the players with these userIDs.</summary>
        public const byte ExpectedUsers = (byte)247;

        /// <summary>(246) Player Time To Live. How long any player can be inactive (due to disconnect or leave) before the user gets removed from the playerlist (freeing a slot).</summary>
        public const byte PlayerTtl = (byte)246;

        /// <summary>(245) Room Time To Live. How long a room stays available (and in server-memory), after the last player becomes inactive. After this time, the room gets persisted or destroyed.</summary>
        public const byte EmptyRoomTtl = (byte)245;
}


    /// <summary>
    /// Class for constants. These values are for events defined by Photon Loadbalancing.
    /// </summary>
    /// <remarks>They start at 255 and go DOWN. Your own in-game events can start at 0. Pun uses these constants internally.</remarks>
    public class EventCode
    {
        /// <summary>(230) Initial list of RoomInfos (in lobby on Master)</summary>
        public const byte GameList = 230;

        /// <summary>(229) Update of RoomInfos to be merged into "initial" list (in lobby on Master)</summary>
        public const byte GameListUpdate = 229;

        /// <summary>(228) Currently not used. State of queueing in case of server-full</summary>
        public const byte QueueState = 228;

        /// <summary>(227) Currently not used. Event for matchmaking</summary>
        public const byte Match = 227;

        /// <summary>(226) Event with stats about this application (players, rooms, etc)</summary>
        public const byte AppStats = 226;

        /// <summary>(224) This event provides a list of lobbies with their player and game counts.</summary>
        public const byte LobbyStats = 224;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;

        /// <summary>(255) Event Join: someone joined the game. The new actorNumber is provided as well as the properties of that actor (if set in OpJoin).</summary>
        public const byte Join = (byte)255;

        /// <summary>(254) Event Leave: The player who left the game can be identified by the actorNumber.</summary>
        public const byte Leave = (byte)254;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        public const byte PropertiesChanged = (byte)253;

        /// <summary>(253) When you call OpSetProperties with the broadcast option "on", this event is fired. It contains the properties being set.</summary>
        [Obsolete("Use PropertiesChanged now.")]
        public const byte SetProperties = (byte)253;

        /// <summary>(252) When player left game unexpected and the room has a playerTtl != 0, this event is fired to let everyone know about the timeout.</summary>
        /// Obsolete. Replaced by Leave. public const byte Disconnect = LiteEventCode.Disconnect;

        /// <summary>(251) Sent by Photon Cloud when a plugin-call or webhook-call failed. Usually, the execution on the server continues, despite the issue. Contains: ParameterCode.Info.</summary>
        /// <seealso cref="https://doc.photonengine.com/en-us/pun/current/gameplay/web-extensions/webhooks#options"/>
        public const byte ErrorInfo = 251;

        /// <summary>(250) Sent by Photon whent he event cache slice was changed. Done by OpRaiseEvent.</summary>
        public const byte CacheSliceChanged = 250;

        /// <summary>(223) Sent by Photon to update a token before it times out.</summary>
        public const byte AuthEvent = 223;
    }


    /// <summary>Class for constants. Codes for parameters of Operations and Events.</summary>
    /// <remarks>Pun uses these constants internally.</remarks>
    public class ParameterCode
    {
        /// <summary>(237) A bool parameter for creating games. If set to true, no room events are sent to the clients on join and leave. Default: false (and not sent).</summary>
        public const byte SuppressRoomEvents = 237;

        /// <summary>(236) Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
        public const byte EmptyRoomTTL = 236;

        /// <summary>(235) Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
        public const byte PlayerTTL = 235;

        /// <summary>(234) Optional parameter of OpRaiseEvent and OpSetCustomProperties to forward the event/operation to a web-service.</summary>
        public const byte EventForward = 234;

        /// <summary>(233) Optional parameter of OpLeave in async games. If false, the player does abandons the game (forever). By default players become inactive and can re-join.</summary>
        [Obsolete("Use: IsInactive")]
        public const byte IsComingBack = (byte)233;

        /// <summary>(233) Used in EvLeave to describe if a user is inactive (and might come back) or not. In rooms with PlayerTTL, becoming inactive is the default case.</summary>
        public const byte IsInactive = (byte)233;

        /// <summary>(232) Used when creating rooms to define if any userid can join the room only once.</summary>
        public const byte CheckUserOnJoin = (byte)232;

        /// <summary>(231) Code for "Check And Swap" (CAS) when changing properties.</summary>
        public const byte ExpectedValues = (byte)231;

        /// <summary>(230) Address of a (game) server to use.</summary>
        public const byte Address = 230;

        /// <summary>(229) Count of players in this application in a rooms (used in stats event)</summary>
        public const byte PeerCount = 229;

        /// <summary>(228) Count of games in this application (used in stats event)</summary>
        public const byte GameCount = 228;

        /// <summary>(227) Count of players on the master server (in this app, looking for rooms)</summary>
        public const byte MasterPeerCount = 227;

        /// <summary>(225) User's ID</summary>
        public const byte UserId = 225;

        /// <summary>(224) Your application's ID: a name on your own Photon or a GUID on the Photon Cloud</summary>
        public const byte ApplicationId = 224;

        /// <summary>(223) Not used currently (as "Position"). If you get queued before connect, this is your position</summary>
        public const byte Position = 223;

        /// <summary>(223) Modifies the matchmaking algorithm used for OpJoinRandom. Allowed parameter values are defined in enum MatchmakingMode.</summary>
        public const byte MatchMakingType = 223;

        /// <summary>(222) List of RoomInfos about open / listed rooms</summary>
        public const byte GameList = 222;

        /// <summary>(221) Internally used to establish encryption</summary>
        public const byte Secret = 221;

        /// <summary>(220) Version of your application</summary>
        public const byte AppVersion = 220;

        /// <summary>(210) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureNodeInfo = 210;	// only used within events, so use: EventCode.AzureNodeInfo

        /// <summary>(209) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureLocalNodeId = 209;

        /// <summary>(208) Internally used in case of hosting by Azure</summary>
        [Obsolete("TCP routing was removed after becoming obsolete.")]
        public const byte AzureMasterNodeId = 208;

        /// <summary>(255) Code for the gameId/roomName (a unique name per room). Used in OpJoin and similar.</summary>
        public const byte RoomName = (byte)255;

        /// <summary>(250) Code for broadcast parameter of OpSetProperties method.</summary>
        public const byte Broadcast = (byte)250;

        /// <summary>(252) Code for list of players in a room. Currently not used.</summary>
        public const byte ActorList = (byte)252;

        /// <summary>(254) Code of the Actor of an operation. Used for property get and set.</summary>
        public const byte ActorNr = (byte)254;

        /// <summary>(249) Code for property set (Hashtable).</summary>
        public const byte PlayerProperties = (byte)249;

        /// <summary>(245) Code of data/custom content of an event. Used in OpRaiseEvent.</summary>
        public const byte CustomEventContent = (byte)245;

        /// <summary>(245) Code of data of an event. Used in OpRaiseEvent.</summary>
        public const byte Data = (byte)245;

        /// <summary>(244) Code used when sending some code-related parameter, like OpRaiseEvent's event-code.</summary>
        /// <remarks>This is not the same as the Operation's code, which is no longer sent as part of the parameter Dictionary in Photon 3.</remarks>
        public const byte Code = (byte)244;

        /// <summary>(248) Code for property set (Hashtable).</summary>
        public const byte GameProperties = (byte)248;

        /// <summary>
        /// (251) Code for property-set (Hashtable). This key is used when sending only one set of properties.
        /// If either ActorProperties or GameProperties are used (or both), check those keys.
        /// </summary>
        public const byte Properties = (byte)251;

        /// <summary>(253) Code of the target Actor of an operation. Used for property set. Is 0 for game</summary>
        public const byte TargetActorNr = (byte)253;

        /// <summary>(246) Code to select the receivers of events (used in Lite, Operation RaiseEvent).</summary>
        public const byte ReceiverGroup = (byte)246;

        /// <summary>(247) Code for caching events while raising them.</summary>
        public const byte Cache = (byte)247;

        /// <summary>(241) Bool parameter of CreateRoom Operation. If true, server cleans up roomcache of leaving players (their cached events get removed).</summary>
        public const byte CleanupCacheOnLeave = (byte)241;

        /// <summary>(240) Code for "group" operation-parameter (as used in Op RaiseEvent).</summary>
        public const byte Group = 240;

        /// <summary>(239) The "Remove" operation-parameter can be used to remove something from a list. E.g. remove groups from player's interest groups.</summary>
        public const byte Remove = 239;

        /// <summary>(239) Used in Op Join to define if UserIds of the players are broadcast in the room. Useful for FindFriends and reserving slots for expected users.</summary>
        public const byte PublishUserId = 239;

        /// <summary>(238) The "Add" operation-parameter can be used to add something to some list or set. E.g. add groups to player's interest groups.</summary>
        public const byte Add = 238;

        /// <summary>(218) Content for EventCode.ErrorInfo and internal debug operations.</summary>
        public const byte Info = 218;

        /// <summary>(217) This key's (byte) value defines the target custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationType = 217;

        /// <summary>(216) This key's (string) value provides parameters sent to the custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationParams = 216;

        /// <summary>(215) Makes the server create a room if it doesn't exist. OpJoin uses this to always enter a room, unless it exists and is full/closed.</summary>
        // public const byte CreateIfNotExists = 215;

        /// <summary>(215) The JoinMode enum defines which variant of joining a room will be executed: Join only if available, create if not exists or re-join.</summary>
        /// <remarks>Replaces CreateIfNotExists which was only a bool-value.</remarks>
        public const byte JoinMode = 215;

        /// <summary>(214) This key's (string or byte[]) value provides parameters sent to the custom authentication service setup in Photon Dashboard. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationData = 214;

        /// <summary>(203) Code for MasterClientId, which is synced by server. When sent as op-parameter this is code 203.</summary>
        /// <remarks>Tightly related to GamePropertyKey.MasterClientId.</remarks>
        public const byte MasterClientId = (byte)203;

        /// <summary>(1) Used in Op FindFriends request. Value must be string[] of friends to look up.</summary>
        public const byte FindFriendsRequestList = (byte)1;

        /// <summary>(1) Used in Op FindFriends response. Contains bool[] list of online states (false if not online).</summary>
        public const byte FindFriendsResponseOnlineList = (byte)1;

        /// <summary>(2) Used in Op FindFriends response. Contains string[] of room names ("" where not known or no room joined).</summary>
        public const byte FindFriendsResponseRoomIdList = (byte)2;

        /// <summary>(213) Used in matchmaking-related methods and when creating a room to name a lobby (to join or to attach a room to).</summary>
        public const byte LobbyName = (byte)213;

        /// <summary>(212) Used in matchmaking-related methods and when creating a room to define the type of a lobby. Combined with the lobby name this identifies the lobby.</summary>
        public const byte LobbyType = (byte)212;

        /// <summary>(211) This (optional) parameter can be sent in Op Authenticate to turn on Lobby Stats (info about lobby names and their user- and game-counts). See: PhotonNetwork.Lobbies</summary>
        public const byte LobbyStats = (byte)211;

        /// <summary>(210) Used for region values in OpAuth and OpGetRegions.</summary>
        public const byte Region = (byte)210;

        /// <summary>(209) Path of the WebRPC that got called. Also known as "WebRpc Name". Type: string.</summary>
        public const byte UriPath = 209;

        /// <summary>(208) Parameters for a WebRPC as: Dictionary&lt;string, object&gt;. This will get serialized to JSon.</summary>
        public const byte WebRpcParameters = 208;

        /// <summary>(207) ReturnCode for the WebRPC, as sent by the web service (not by Photon, which uses ErrorCode). Type: byte.</summary>
        public const byte WebRpcReturnCode = 207;

        /// <summary>(206) Message returned by WebRPC server. Analog to Photon's debug message. Type: string.</summary>
        public const byte WebRpcReturnMessage = 206;

        /// <summary>(205) Used to define a "slice" for cached events. Slices can easily be removed from cache. Type: int.</summary>
        public const byte CacheSliceIndex = 205;

        /// <summary>(204) Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin mismatch returning error code PluginMismatch 32751(0x7FFF - 16).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        public const byte Plugins = 204;

        /// <summary>(202) Used by the server in Operation Responses, when it sends the nickname of the client (the user's nickname).</summary>
        public const byte NickName = 202;

        /// <summary>(201) Informs user about name of plugin load to game</summary>
        public const byte PluginName = 201;

        /// <summary>(200) Informs user about version of plugin load to game</summary>
        public const byte PluginVersion = 200;

        /// <summary>(195) Protocol which will be used by client to connect master/game servers. Used for nameserver.</summary>
        public const byte ExpectedProtocol = 195;

        /// <summary>(194) Set of custom parameters which are sent in auth request.</summary>
        public const byte CustomInitData = 194;

        /// <summary>(193) How are we going to encrypt data.</summary>
        public const byte EncryptionMode = 193;

        /// <summary>(192) Parameter of Authentication, which contains encryption keys (depends on AuthMode and EncryptionMode).</summary>
        public const byte EncryptionData = 192;

        /// <summary>(191) An int parameter summarizing several boolean room-options with bit-flags.</summary>
        public const byte RoomOptionFlags = 191;
    }


    /// <summary>
    /// Class for constants. Contains operation codes.
    /// Pun uses these constants internally.
    /// </summary>
    public class OperationCode
    {
        [Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")]
        public const byte ExchangeKeysForEncryption = 250;

        /// <summary>(255) Code for OpJoin, to get into a room.</summary>
		[Obsolete]
        public const byte Join = 255;

        /// <summary>(231) Authenticates this peer and connects to a virtual application</summary>
        public const byte AuthenticateOnce = 231;

        /// <summary>(230) Authenticates this peer and connects to a virtual application</summary>
        public const byte Authenticate = 230;

        /// <summary>(229) Joins lobby (on master)</summary>
        public const byte JoinLobby = 229;

        /// <summary>(228) Leaves lobby (on master)</summary>
        public const byte LeaveLobby = 228;

        /// <summary>(227) Creates a game (or fails if name exists)</summary>
        public const byte CreateGame = 227;

        /// <summary>(226) Join game (by name)</summary>
        public const byte JoinGame = 226;

        /// <summary>(225) Joins random game (on master)</summary>
        public const byte JoinRandomGame = 225;

        // public const byte CancelJoinRandom = 224; // obsolete, cause JoinRandom no longer is a "process". now provides result immediately

        /// <summary>(254) Code for OpLeave, to get out of a room.</summary>
        public const byte Leave = (byte)254;

        /// <summary>(253) Raise event (in a room, for other actors/players)</summary>
        public const byte RaiseEvent = (byte)253;

        /// <summary>(252) Set Properties (of room or actor/player)</summary>
        public const byte SetProperties = (byte)252;

        /// <summary>(251) Get Properties</summary>
        public const byte GetProperties = (byte)251;

        /// <summary>(248) Operation code to change interest groups in Rooms (Lite application and extending ones).</summary>
        public const byte ChangeGroups = (byte)248;

        /// <summary>(222) Request the rooms and online status for a list of friends (by name, which should be unique).</summary>
        public const byte FindFriends = 222;

        /// <summary>(221) Request statistics about a specific list of lobbies (their user and game count).</summary>
        public const byte GetLobbyStats = 221;

        /// <summary>(220) Get list of regional servers from a NameServer.</summary>
        public const byte GetRegions = 220;

        /// <summary>(219) WebRpc Operation.</summary>
        public const byte WebRpc = 219;

        /// <summary>(218) Operation to set some server settings. Used with different parameters on various servers.</summary>
        public const byte ServerSettings = 218;

        /// <summary>(217) Get the game list matching a supplied sql filter (SqlListLobby only) </summary>
        public const byte GetGameList = 217;
    }

    /// <summary>Defines possible values for OpJoinRoom and OpJoinOrCreate. It tells the server if the room can be only be joined normally, created implicitly or found on a web-service for Turnbased games.</summary>
    /// <remarks>These values are not directly used by a game but implicitly set.</remarks>
    public enum JoinMode : byte
    {
        /// <summary>Regular join. The room must exist.</summary>
        Default = 0,

        /// <summary>Join or create the room if it's not existing. Used for OpJoinOrCreate for example.</summary>
        CreateIfNotExists = 1,

        /// <summary>The room might be out of memory and should be loaded (if possible) from a Turnbased web-service.</summary>
        JoinOrRejoin = 2,

        /// <summary>Only re-join will be allowed. If the user is not yet in the room, this will fail.</summary>
        RejoinOnly = 3,
    }

    /// <summary>
    /// Options for matchmaking rules for OpJoinRandom.
    /// </summary>
    public enum MatchmakingMode : byte
    {
        /// <summary>Fills up rooms (oldest first) to get players together as fast as possible. Default.</summary>
        /// <remarks>Makes most sense with MaxPlayers > 0 and games that can only start with more players.</remarks>
        FillRoom = 0,

        /// <summary>Distributes players across available rooms sequentially but takes filter into account. Without filter, rooms get players evenly distributed.</summary>
        SerialMatching = 1,

        /// <summary>Joins a (fully) random room. Expected properties must match but aside from this, any available room might be selected.</summary>
        RandomMatching = 2
    }


    /// <summary>
    /// Lite - OpRaiseEvent lets you chose which actors in the room should receive events.
    /// By default, events are sent to "Others" but you can overrule this.
    /// </summary>
    public enum ReceiverGroup : byte
    {
        /// <summary>Default value (not sent). Anyone else gets my event.</summary>
        Others = 0,

        /// <summary>Everyone in the current room (including this peer) will get this event.</summary>
        All = 1,

        /// <summary>The server sends this event only to the actor with the lowest actorNumber.</summary>
        /// <remarks>The "master client" does not have special rights but is the one who is in this room the longest time.</remarks>
        MasterClient = 2,
    }

    /// <summary>
    /// Lite - OpRaiseEvent allows you to cache events and automatically send them to joining players in a room.
    /// Events are cached per event code and player: Event 100 (example!) can be stored once per player.
    /// Cached events can be modified, replaced and removed.
    /// </summary>
    /// <remarks>
    /// Caching works only combination with ReceiverGroup options Others and All.
    /// </remarks>
    public enum EventCaching : byte
    {
        /// <summary>Default value (not sent).</summary>
        DoNotCache = 0,

        /// <summary>Will merge this event's keys with those already cached.</summary>
        [Obsolete]
        MergeCache = 1,

        /// <summary>Replaces the event cache for this eventCode with this event's content.</summary>
        [Obsolete]
        ReplaceCache = 2,

        /// <summary>Removes this event (by eventCode) from the cache.</summary>
        [Obsolete]
        RemoveCache = 3,

        /// <summary>Adds an event to the room's cache</summary>
        AddToRoomCache = 4,

        /// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
        AddToRoomCacheGlobal = 5,

        /// <summary>Remove fitting event from the room's cache.</summary>
        RemoveFromRoomCache = 6,

        /// <summary>Removes events of players who already left the room (cleaning up).</summary>
        RemoveFromRoomCacheForActorsLeft = 7,

        /// <summary>Increase the index of the sliced cache.</summary>
        SliceIncreaseIndex = 10,

        /// <summary>Set the index of the sliced cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SliceSetIndex = 11,

        /// <summary>Purge cache slice with index. Exactly one slice is removed from cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeIndex = 12,

        /// <summary>Purge cache slices with specified index and anything lower than that. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeUpToIndex = 13,
    }

    /// <summary>
    /// Flags for "types of properties", being used as filter in OpGetProperties.
    /// </summary>
    [Flags]
    public enum PropertyTypeFlag : byte
    {
        /// <summary>(0x00) Flag type for no property type.</summary>
        None = 0x00,

        /// <summary>(0x01) Flag type for game-attached properties.</summary>
        Game = 0x01,

        /// <summary>(0x02) Flag type for actor related propeties.</summary>
        Actor = 0x02,

        /// <summary>(0x01) Flag type for game AND actor properties. Equal to 'Game'</summary>
        GameAndActor = Game | Actor
    }


    /// <summary>Wraps up common room properties needed when you create rooms. Read the individual entries for more details.</summary>
    /// <remarks>This directly maps to the fields in the Room class.</remarks>
    public class RoomOptions
    {
        /// <summary>Defines if this room is listed in the lobby. If not, it also is not joined randomly.</summary>
        /// <remarks>
        /// A room that is not visible will be excluded from the room lists that are sent to the clients in lobbies.
        /// An invisible room can be joined by name but is excluded from random matchmaking.
        ///
        /// Use this to "hide" a room and simulate "private rooms". Players can exchange a roomname and create it
        /// invisble to avoid anyone else joining it.
        /// </remarks>
        public bool IsVisible { get { return this.isVisibleField; } set { this.isVisibleField = value; } }
        private bool isVisibleField = true;

        /// <summary>Defines if this room can be joined at all.</summary>
        /// <remarks>
        /// If a room is closed, no player can join this. As example this makes sense when 3 of 4 possible players
        /// start their gameplay early and don't want anyone to join during the game.
        /// The room can still be listed in the lobby (set IsVisible to control lobby-visibility).
        /// </remarks>
        public bool IsOpen { get { return this.isOpenField; } set { this.isOpenField = value; } }
        private bool isOpenField = true;

        /// <summary>Max number of players that can be in the room at any time. 0 means "no limit".</summary>
        public byte MaxPlayers;


        /// <summary>Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
        public int PlayerTtl;


        /// <summary>Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
        public int EmptyRoomTtl;

        ///// <summary>Activates UserId checks on joining - allowing a users to be only once in the room.</summary>
        ///// <remarks>
        ///// Turnbased rooms should be created with this check turned on! They should also use custom authentication.
        ///// Disabled by default for backwards-compatibility.
        ///// </remarks>
        //public bool CheckUserOnJoin { get { return this.checkUserOnJoinField; } set { this.checkUserOnJoinField = value; } }
        //private bool checkUserOnJoinField = false;

        /// <summary>Removes a user's events and properties from the room when a user leaves.</summary>
        /// <remarks>
        /// This makes sense when in rooms where players can't place items in the room and just vanish entirely.
        /// When you disable this, the event history can become too long to load if the room stays in use indefinitely.
        /// Default: true. Cleans up the cache and props of leaving users.
        /// </remarks>
        public bool CleanupCacheOnLeave { get { return this.cleanupCacheOnLeaveField; } set { this.cleanupCacheOnLeaveField = value; } }
        private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

        /// <summary>The room's custom properties to set. Use string keys!</summary>
        /// <remarks>
        /// Custom room properties are any key-values you need to define the game's setup.
        /// The shorter your keys are, the better.
        /// Example: Map, Mode (could be "m" when used with "Map"), TileSet (could be "t").
        /// </remarks>
        public Hashtable CustomRoomProperties;

        /// <summary>Defines the custom room properties that get listed in the lobby.</summary>
        /// <remarks>
        /// Name the custom room properties that should be available to clients that are in a lobby.
        /// Use with care. Unless a custom property is essential for matchmaking or user info, it should
        /// not be sent to the lobby, which causes traffic and delays for clients in the lobby.
        ///
        /// Default: No custom properties are sent to the lobby.
        /// </remarks>
        public string[] CustomRoomPropertiesForLobby = new string[0];

        /// <summary>Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin missmatch returning error code PluginMismatch 32757(0x7FFF - 10).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        public string[] Plugins;

        /// <summary>
        /// Tells the server to skip room events for joining and leaving players.
        /// </summary>
        /// <remarks>
        /// Using this makes the client unaware of the other players in a room.
        /// That can save some traffic if you have some server logic that updates players
        /// but it can also limit the client's usability.
        ///
        /// PUN will break if you use this, so it's not settable.
        /// </remarks>
        public bool SuppressRoomEvents { get { return this.suppressRoomEventsField; } /*set { this.suppressRoomEventsField = value; }*/ }
        private bool suppressRoomEventsField = false;

        /// <summary>
        /// Defines if the UserIds of players get "published" in the room. Useful for FindFriends, if players want to play another game together.
        /// </summary>
        /// <remarks>
        /// When you set this to true, Photon will publish the UserIds of the players in that room.
        /// In that case, you can use PhotonPlayer.UserId, to access any player's userID.
        /// This is useful for FindFriends and to set "expected users" to reserve slots in a room (see PhotonNetwork.JoinRoom e.g.).
        /// </remarks>
        public bool PublishUserId { get { return this.publishUserIdField; } set { this.publishUserIdField = value; } }
        private bool publishUserIdField = false;

        /// <summary>Optionally, properties get deleted, when null gets assigned as value. Defaults to off / false.</summary>
        /// <remarks>
        /// When Op SetProperties is setting a key's value to null, the server and clients should remove the key/value from the Custom Properties.
        /// By default, the server keeps the keys (and null values) and sends them to joining players.
        ///
        /// Important: Only when SetProperties does a "broadcast", the change (key, value = null) is sent to clients to update accordingly.
        /// This applies to Custom Properties for rooms and actors/players.
        /// </remarks>
        public bool DeleteNullProperties { get { return this.deleteNullPropertiesField; } set { this.deleteNullPropertiesField = value; } }
        private bool deleteNullPropertiesField = false;

    #region Obsoleted Naming

    [Obsolete("Use property with uppercase naming instead.")]
    public bool isVisible { get { return this.isVisibleField; } set { this.isVisibleField = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public bool isOpen { get { return this.isOpenField; } set { this.isOpenField = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public byte maxPlayers { get { return this.MaxPlayers; } set { this.MaxPlayers = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public bool cleanupCacheOnLeave { get { return this.cleanupCacheOnLeaveField; } set { this.cleanupCacheOnLeaveField = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public Hashtable customRoomProperties { get { return this.CustomRoomProperties; } set { this.CustomRoomProperties = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public string[] customRoomPropertiesForLobby { get { return this.CustomRoomPropertiesForLobby; } set { this.CustomRoomPropertiesForLobby = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public string[] plugins { get { return this.Plugins; } set { this.Plugins = value; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public bool suppressRoomEvents { get { return this.suppressRoomEventsField; } }
    [Obsolete("Use property with uppercase naming instead.")]
    public bool publishUserId { get { return this.publishUserIdField; } set { this.publishUserIdField = value; } }

    #endregion
}


    /// <summary>Aggregates several less-often used options for operation RaiseEvent. See field descriptions for usage details.</summary>
    public class RaiseEventOptions
    {
        /// <summary>Default options: CachingOption: DoNotCache, InterestGroup: 0, targetActors: null, receivers: Others, sequenceChannel: 0.</summary>
        public readonly static RaiseEventOptions Default = new RaiseEventOptions();

		/// <summary>
		/// Reset this instance. For better memory handling than instanciating a new one always,
		/// </summary>
		public void Reset()
		{
			this.CachingOption = Default.CachingOption;
			this.InterestGroup = Default.InterestGroup;
			this.TargetActors = Default.TargetActors;
			this.Receivers = Default.Receivers;
			this.SequenceChannel = Default.SequenceChannel;
			this.ForwardToWebhook = Default.ForwardToWebhook;
			this.Encrypt = Default.Encrypt;
		}

        /// <summary>Defines if the server should simply send the event, put it in the cache or remove events that are like this one.</summary>
        /// <remarks>
        /// When using option: SliceSetIndex, SlicePurgeIndex or SlicePurgeUpToIndex, set a CacheSliceIndex. All other options except SequenceChannel get ignored.
        /// </remarks>
        public EventCaching CachingOption;

        /// <summary>The number of the Interest Group to send this to. 0 goes to all users but to get 1 and up, clients must subscribe to the group first.</summary>
        public byte InterestGroup;

        /// <summary>A list of PhotonPlayer.IDs to send this event to. You can implement events that just go to specific users this way.</summary>
        public int[] TargetActors;

        /// <summary>Sends the event to All, MasterClient or Others (default). Be careful with MasterClient, as the client might disconnect before it got the event and it gets lost.</summary>
        public ReceiverGroup Receivers;

        /// <summary>Events are ordered per "channel". If you have events that are independent of others, they can go into another sequence or channel.</summary>
        public byte SequenceChannel;

        /// <summary>Events can be forwarded to Webhooks, which can evaluate and use the events to follow the game's state.</summary>
        public bool ForwardToWebhook;

        ///// <summary>Used along with CachingOption SliceSetIndex, SlicePurgeIndex or SlicePurgeUpToIndex if you want to set or purge a specific cache-slice.</summary>
        //public int CacheSliceIndex;

        public bool Encrypt;
    }

    /// <summary>
    /// Options of lobby types available. Lobby types might be implemented in certain Photon versions and won't be available on older servers.
    /// </summary>
    public enum LobbyType :byte
    {
        /// <summary>This lobby is used unless another is defined by game or JoinRandom. Room-lists will be sent and JoinRandomRoom can filter by matching properties.</summary>
        Default = 0,
        /// <summary>This lobby type lists rooms like Default but JoinRandom has a parameter for SQL-like "where" clauses for filtering. This allows bigger, less, or and and combinations.</summary>
        SqlLobby = 2,
        /// <summary>This lobby does not send lists of games. It is only used for OpJoinRandomRoom. It keeps rooms available for a while when there are only inactive users left.</summary>
        AsyncRandomLobby = 3
    }

    /// <summary>Refers to a specific lobby (and type) on the server.</summary>
    /// <remarks>
    /// The name and type are the unique identifier for a lobby.<br/>
    /// Join a lobby via PhotonNetwork.JoinLobby(TypedLobby lobby).<br/>
    /// The current lobby is stored in PhotonNetwork.lobby.
    /// </remarks>
    public class TypedLobby
    {
        /// <summary>Name of the lobby this game gets added to. Default: null, attached to default lobby. Lobbies are unique per lobbyName plus lobbyType, so the same name can be used when several types are existing.</summary>
        public string Name;
        /// <summary>Type of the (named)lobby this game gets added to</summary>
        public LobbyType Type;

        public static readonly TypedLobby Default = new TypedLobby();
        public bool IsDefault { get { return this.Type == LobbyType.Default && string.IsNullOrEmpty(this.Name); } }

        public TypedLobby()
        {
            this.Name = string.Empty;
            this.Type = LobbyType.Default;
        }

        public TypedLobby(string name, LobbyType type)
        {
            this.Name = name;
            this.Type = type;
        }

        public override string ToString()
        {
            return String.Format((string) "lobby '{0}'[{1}]", (object) this.Name, (object) this.Type);
        }
    }

    public class TypedLobbyInfo : TypedLobby
    {
        public int PlayerCount;
        public int RoomCount;

        public override string ToString()
        {
            return string.Format("TypedLobbyInfo '{0}'[{1}] rooms: {2} players: {3}", this.Name, this.Type, this.RoomCount, this.PlayerCount);
        }
    }


    /// <summary>
    /// Options for authentication modes. From "classic" auth on each server to AuthOnce (on NameServer).
    /// </summary>
    public enum AuthModeOption { Auth, AuthOnce, AuthOnceWss }


    /// <summary>
    /// Options for optional "Custom Authentication" services used with Photon. Used by OpAuthenticate after connecting to Photon.
    /// </summary>
    public enum CustomAuthenticationType : byte
    {
        /// <summary>Use a custom authentification service. Currently the only implemented option.</summary>
        Custom = 0,

        /// <summary>Authenticates users by their Steam Account. Set auth values accordingly!</summary>
        Steam = 1,

        /// <summary>Authenticates users by their Facebook Account. Set auth values accordingly!</summary>
        Facebook = 2,

        /// <summary>Authenticates users by their Oculus Account and token.</summary>
        Oculus = 3,

        /// <summary>Authenticates users by their PSN Account and token.</summary>
        PlayStation = 4,

        /// <summary>Authenticates users by their Xbox Account and XSTS token.</summary>
        Xbox = 5,

        /// <summary>Disables custom authentification. Same as not providing any AuthenticationValues for connect (more precisely for: OpAuthenticate).</summary>
        None = byte.MaxValue
    }


    /// <summary>
    /// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
    /// </summary>
    /// <remarks>
    /// On Photon, user authentication is optional but can be useful in many cases.
    /// If you want to FindFriends, a unique ID per user is very practical.
    ///
    /// There are basically three options for user authentification: None at all, the client sets some UserId
    /// or you can use some account web-service to authenticate a user (and set the UserId server-side).
    ///
    /// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
    /// values to Photon which will verify them before granting access or disconnecting the client.
    ///
    /// The AuthValues are sent in OpAuthenticate when you connect, so they must be set before you connect.
    /// Should you not set any AuthValues, PUN will create them and set the playerName as userId in them.
    /// If the AuthValues.userId is null or empty when it's sent to the server, then the Photon Server assigns a userId!
    ///
    /// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
    /// https://www.photonengine.com/dashboard
    /// </remarks>
    public class AuthenticationValues
    {
        /// <summary>See AuthType.</summary>
        private CustomAuthenticationType authType = CustomAuthenticationType.None;

        /// <summary>The type of custom authentication provider that should be used. Currently only "Custom" or "None" (turns this off).</summary>
        public CustomAuthenticationType AuthType
        {
            get { return authType; }
            set { authType = value; }
        }

        /// <summary>This string must contain any (http get) parameters expected by the used authentication service. By default, username and token.</summary>
        /// <remarks>Standard http get parameters are used here and passed on to the service that's defined in the server (Photon Cloud Dashboard).</remarks>
        public string AuthGetParameters { get; set; }

        /// <summary>Data to be passed-on to the auth service via POST. Default: null (not sent). Either string or byte[] (see setters).</summary>
        public object AuthPostData { get; private set; }

        /// <summary>After initial authentication, Photon provides a token for this client / user, which is subsequently used as (cached) validation.</summary>
        public string Token { get; set; }

        /// <summary>The UserId should be a unique identifier per user. This is for finding friends, etc..</summary>
        /// <remarks>See remarks of AuthValues for info about how this is set and used.</remarks>
        public string UserId { get; set; }


        /// <summary>Creates empty auth values without any info.</summary>
        public AuthenticationValues()
        {
        }

        /// <summary>Creates minimal info about the user. If this is authenticated or not, depends on the set AuthType.</summary>
        /// <param name="userId">Some UserId to set in Photon.</param>
        public AuthenticationValues(string userId)
        {
            this.UserId = userId;
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary. Each SetAuthPostData replaces any previous value.</remarks>
        /// <param name="stringData">String data to be used in the body of the POST request. Null or empty string will set AuthPostData to null.</param>
        public virtual void SetAuthPostData(string stringData)
        {
            this.AuthPostData = (string.IsNullOrEmpty(stringData)) ? null : stringData;
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary. Each SetAuthPostData replaces any previous value.</remarks>
        /// <param name="byteData">Binary token / auth-data to pass on.</param>
        public virtual void SetAuthPostData(byte[] byteData)
        {
            this.AuthPostData = byteData;
        }

        /// <summary>Sets data to be passed-on to the auth service as Json (Content-Type: "application/json") via Post.</summary>
        /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary. Each SetAuthPostData replaces any previous value.</remarks>
        /// <param name="dictData">A authentication-data dictionary will be converted to Json and passed to the Auth webservice via HTTP Post.</param>
        public virtual void SetAuthPostData(Dictionary<string, object> dictData)
        {
            this.AuthPostData = dictData;
        }

        /// <summary>Adds a key-value pair to the get-parameters used for Custom Auth.</summary>
        /// <remarks>This method does uri-encoding for you.</remarks>
        /// <param name="key">Key for the value to set.</param>
        /// <param name="value">Some value relevant for Custom Authentication.</param>
        public virtual void AddAuthParameter(string key, string value)
        {
            string ampersand = string.IsNullOrEmpty(this.AuthGetParameters) ? "" : "&";
            this.AuthGetParameters = string.Format("{0}{1}{2}={3}", this.AuthGetParameters, ampersand, System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value));
        }

        public override string ToString()
        {
            return string.Format("AuthenticationValues UserId: {0}, GetParameters: {1} Token available: {2}", this.UserId, this.AuthGetParameters, this.Token != null);
        }
    }
