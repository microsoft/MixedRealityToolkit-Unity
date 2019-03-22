// ----------------------------------------------------------------------------
// <copyright file="Room.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   The Room class resembles the properties known about the room in which
//   a game/match happens.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>
    /// This class represents a room a client joins/joined.
    /// </summary>
    /// <remarks>
    /// Contains a list of current players, their properties and those of this room, too.
    /// A room instance has a number of "well known" properties like IsOpen, MaxPlayers which can be changed.
    /// Your own, custom properties can be set via SetCustomProperties() while being in the room.
    ///
    /// Typically, this class should be extended by a game-specific implementation with logic and extra features.
    /// </remarks>
    public class Room : RoomInfo
    {
        /// <summary>
        /// A reference to the LoadBalancingClient which is currently keeping the connection and state.
        /// </summary>
        public LoadBalancingClient LoadBalancingClient { get; set; }

        /// <summary>The name of a room. Unique identifier (per region and virtual appid) for a room/match.</summary>
        /// <remarks>The name can't be changed once it's set by the server.</remarks>
        public new string Name
        {
            get
            {
                return this.name;
            }

            internal set
            {
                this.name = value;
            }
        }

        private bool isOffline;

        public bool IsOffline
        {
            get
            {
                return isOffline;
            }

            private set
            {
                isOffline = value;
            }
        }

        /// <summary>
        /// Defines if the room can be joined.
        /// </summary>
        /// <remarks>
        /// This does not affect listing in a lobby but joining the room will fail if not open.
        /// If not open, the room is excluded from random matchmaking.
        /// Due to racing conditions, found matches might become closed while users are trying to join.
        /// Simply re-connect to master and find another.
        /// Use property "IsVisible" to not list the room.
        ///
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public new bool IsOpen
        {
            get
            {
                return this.isOpen;
            }

            set
            {
                if (value != this.isOpen)
                {
                    if (!this.isOffline)
                    {
                        this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.IsOpen, value } });
                    }
                }

                this.isOpen = value;
            }
        }

        /// <summary>
        /// Defines if the room is listed in its lobby.
        /// </summary>
        /// <remarks>
        /// Rooms can be created invisible, or changed to invisible.
        /// To change if a room can be joined, use property: open.
        ///
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public new bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (value != this.isVisible)
                {
                    if (!this.isOffline)
                    {
                        this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.IsVisible, value } });
                    }
                }

                this.isVisible = value;
            }
        }

        /// <summary>
        /// Sets a limit of players to this room. This property is synced and shown in lobby, too.
        /// If the room is full (players count == maxplayers), joining this room will fail.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public new byte MaxPlayers
        {
            get
            {
                return this.maxPlayers;
            }

            set
            {
                if (value != this.maxPlayers)
                {
                    if (!this.isOffline)
                    {
                        this.LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.MaxPlayers, value } });
                    }
                }

                this.maxPlayers = value;
            }
        }

        /// <summary>The count of players in this Room (using this.Players.Count).</summary>
        public new byte PlayerCount
        {
            get
            {
                if (this.Players == null)
                {
                    return 0;
                }

                return (byte)this.Players.Count;
            }
        }

        /// <summary>While inside a Room, this is the list of players who are also in that room.</summary>
        private Dictionary<int, Player> players = new Dictionary<int, Player>();

        /// <summary>While inside a Room, this is the list of players who are also in that room.</summary>
        public Dictionary<int, Player> Players
        {
            get
            {
                return this.players;
            }

            private set
            {
                this.players = value;
            }
        }

        /// <summary>
        /// List of users who are expected to join this room. In matchmaking, Photon blocks a slot for each of these UserIDs out of the MaxPlayers.
        /// </summary>
        /// <remarks>
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// Define expected players in the PhotonNetwork methods: CreateRoom, JoinRoom and JoinOrCreateRoom.
        /// </remarks>
        public string[] ExpectedUsers
        {
            get { return this.expectedUsers; }
        }

        /// <summary>Player Time To Live. How long any player can be inactive (due to disconnect or leave) before the user gets removed from the playerlist (freeing a slot).</summary>
        public int PlayerTtl
        {
            get { return this.playerTtl; }

            set
            {
                if (value != this.playerTtl)
                {
                    if (!this.isOffline)
                    {
                        this.LoadBalancingClient.OpSetPropertyOfRoom(GamePropertyKey.PlayerTtl, value);  // TODO: implement Offline Mode
                    }
                }

                this.playerTtl = value;
            }
        }

        /// <summary>Room Time To Live. How long a room stays available (and in server-memory), after the last player becomes inactive. After this time, the room gets persisted or destroyed.</summary>
        public int EmptyRoomTtl
        {
            get { return this.emptyRoomTtl; }

            set
            {
                if (value != this.emptyRoomTtl)
                {
                    if (!this.isOffline)
                    {
                        this.LoadBalancingClient.OpSetPropertyOfRoom(GamePropertyKey.EmptyRoomTtl, value);  // TODO: implement Offline Mode
                    }
                }

                this.emptyRoomTtl = value;
            }
        }

        /// <summary>
        /// The ID (actorNumber, actorNumber) of the player who's the master of this Room.
        /// Note: This changes when the current master leaves the room.
        /// </summary>
        public int MasterClientId { get { return this.masterClientId; } }

        /// <summary>
        /// Gets a list of custom properties that are in the RoomInfo of the Lobby.
        /// This list is defined when creating the room and can't be changed afterwards. Compare: LoadBalancingClient.OpCreateRoom()
        /// </summary>
        /// <remarks>You could name properties that are not set from the beginning. Those will be synced with the lobby when added later on.</remarks>
        public string[] PropertiesListedInLobby
        {
            get
            {
                return this.propertiesListedInLobby;
            }

            private set
            {
                this.propertiesListedInLobby = value;
            }
        }

        /// <summary>
        /// Gets if this room uses autoCleanUp to remove all (buffered) RPCs and instantiated GameObjects when a player leaves.
        /// </summary>
        public bool AutoCleanUp
        {
            get
            {
                return this.autoCleanUp;
            }
        }


        /// <summary>Creates a Room (representation) with given name and properties and the "listing options" as provided by parameters.</summary>
        /// <param name="roomName">Name of the room (can be null until it's actually created on server).</param>
        /// <param name="options">Room options.</param>
        public Room(string roomName, RoomOptions options, bool isOffline = false) : base(roomName, options != null ? options.CustomRoomProperties : null)
        {
            // base() sets name and (custom)properties. here we set "well known" properties
            if (options != null)
            {
                this.isVisible = options.IsVisible;
                this.isOpen = options.IsOpen;
                this.maxPlayers = options.MaxPlayers;
                this.propertiesListedInLobby = options.CustomRoomPropertiesForLobby;
                //this.playerTtl = options.PlayerTtl;       // set via well known properties
                //this.emptyRoomTtl = options.EmptyRoomTtl; // set via well known properties
            }

            this.isOffline = isOffline;
        }


        protected internal override void InternalCacheProperties(Hashtable propertiesToCache)
        {
            int oldMasterId = this.masterClientId;

            base.InternalCacheProperties(propertiesToCache);    // important: updating the properties fields has no way to do callbacks on change

            if (oldMasterId != 0 && this.masterClientId != oldMasterId)
            {
                this.LoadBalancingClient.InRoomCallbackTargets.OnMasterClientSwitched(this.GetPlayer(this.masterClientId));
            }
        }

        /// <summary>
        /// Updates and synchronizes this Room's Custom Properties. Optionally, expectedProperties can be provided as condition.
        /// </summary>
        /// <remarks>
        /// Custom Properties are a set of string keys and arbitrary values which is synchronized
        /// for the players in a Room. They are available when the client enters the room, as
        /// they are in the response of OpJoin and OpCreate.
        ///
        /// Custom Properties either relate to the (current) Room or a Player (in that Room).
        ///
        /// Both classes locally cache the current key/values and make them available as
        /// property: CustomProperties. This is provided only to read them.
        /// You must use the method SetCustomProperties to set/modify them.
        ///
        /// Any client can set any Custom Properties anytime (when in a room).
        /// It's up to the game logic to organize how they are best used.
        ///
        /// You should call SetCustomProperties only with key/values that are new or changed. This reduces
        /// traffic and performance.
        ///
        /// Unless you define some expectedProperties, setting key/values is always permitted.
        /// In this case, the property-setting client will not receive the new values from the server but
        /// instead update its local cache in SetCustomProperties.
        ///
        /// If you define expectedProperties, the server will skip updates if the server property-cache
        /// does not contain all expectedProperties with the same values.
        /// In this case, the property-setting client will get an update from the server and update it's
        /// cached key/values at about the same time as everyone else.
        ///
        /// The benefit of using expectedProperties can be only one client successfully sets a key from
        /// one known value to another.
        /// As example: Store who owns an item in a Custom Property "ownedBy". It's 0 initally.
        /// When multiple players reach the item, they all attempt to change "ownedBy" from 0 to their
        /// actorNumber. If you use expectedProperties {"ownedBy", 0} as condition, the first player to
        /// take the item will have it (and the others fail to set the ownership).
        ///
        /// Properties get saved with the game state for Turnbased games (which use IsPersistent = true).
        /// </remarks>
        /// <param name="propertiesToSet">Hashtable of Custom Properties that changes.</param>
        /// <param name="expectedProperties">Provide some keys/values to use as condition for setting the new values. Client must be in room.</param>
        /// <param name="webFlags">Defines if this SetCustomProperties-operation gets forwarded to your WebHooks. Client must be in room.</param>
        public virtual void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            Hashtable customProps = propertiesToSet.StripToStringKeys() as Hashtable;

            // merge (and delete null-values), unless we use CAS (expected props)
            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                this.CustomProperties.Merge(customProps);
                this.CustomProperties.StripKeysWithNullValues();
            }

            if (this.isOffline)
            {
                // invoking callbacks
                this.LoadBalancingClient.InRoomCallbackTargets.OnRoomPropertiesUpdate(propertiesToSet);
            }
            else
            {
                // send (sync) these new values if in online room
                this.LoadBalancingClient.LoadBalancingPeer.OpSetPropertiesOfRoom(customProps, expectedProperties, webFlags);
            }
        }

        /// <summary>
        /// Enables you to define the properties available in the lobby if not all properties are needed to pick a room.
        /// </summary>
        /// <remarks>
        /// Limit the amount of properties sent to users in the lobby to improve speed and stability.
        /// </remarks>
        /// <param name="propertiesListedInLobby">An array of custom room property names to forward to the lobby.</param>
        public void SetPropertiesListedInLobby(string[] propertiesListedInLobby)
        {
            Hashtable customProps = new Hashtable();
            customProps[GamePropertyKey.PropsListedInLobby] = propertiesListedInLobby;

            bool sent = this.LoadBalancingClient.OpSetPropertiesOfRoom(customProps);

            if (sent)
            {
                this.propertiesListedInLobby = propertiesListedInLobby;
            }
        }


        /// <summary>
        /// Removes a player from this room's Players Dictionary.
        /// This is internally used by the LoadBalancing API. There is usually no need to remove players yourself.
        /// This is not a way to "kick" players.
        /// </summary>
        protected internal virtual void RemovePlayer(Player player)
        {
            this.Players.Remove(player.ActorNumber);
            player.RoomReference = null;
        }

        /// <summary>
        /// Removes a player from this room's Players Dictionary.
        /// </summary>
        protected internal virtual void RemovePlayer(int id)
        {
            this.RemovePlayer(this.GetPlayer(id));
        }

        /// <summary>
        /// Asks the server to assign another player as Master Client of your current room.
        /// </summary>
        /// <remarks>
        /// RaiseEvent has the option to send messages only to the Master Client of a room.
        /// SetMasterClient affects which client gets those messages.
        ///
        /// This method calls an operation on the server to set a new Master Client, which takes a roundtrip.
        /// In case of success, this client and the others get the new Master Client from the server.
        ///
        /// SetMasterClient tells the server which current Master Client should be replaced with the new one.
        /// It will fail, if anything switches the Master Client moments earlier. There is no callback for this
        /// error. All clients should get the new Master Client assigned by the server anyways.
        ///
        /// See also: MasterClientId
        /// </remarks>
        /// <param name="masterClientPlayer">The player to become the next Master Client.</param>
        /// <returns>False when this operation couldn't be done currently. Requires a v4 Photon Server.</returns>
        public bool SetMasterClient(Player masterClientPlayer)
        {
            Hashtable newProps = new Hashtable() { { GamePropertyKey.MasterClientId, masterClientPlayer.ActorNumber } };
            Hashtable prevProps = new Hashtable() { { GamePropertyKey.MasterClientId, this.MasterClientId } };
            return this.LoadBalancingClient.OpSetPropertiesOfRoom(newProps, prevProps);
        }

        /// <summary>
        /// Checks if the player is in the room's list already and calls StorePlayer() if not.
        /// </summary>
        /// <param name="player">The new player - identified by ID.</param>
        /// <returns>False if the player could not be added (cause it was in the list already).</returns>
        public virtual bool AddPlayer(Player player)
        {
            if (!this.Players.ContainsKey(player.ActorNumber))
            {
                this.StorePlayer(player);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates a player reference in the Players dictionary (no matter if it existed before or not).
        /// </summary>
        /// <param name="player">The Player instance to insert into the room.</param>
        public virtual Player StorePlayer(Player player)
        {
            this.Players[player.ActorNumber] = player;
            player.RoomReference = this;

            // while initializing the room, the players are not guaranteed to be added in-order
            if (this.MasterClientId == 0 || player.ActorNumber < this.MasterClientId)
            {
                this.masterClientId = player.ActorNumber;
            }

            return player;
        }

        /// <summary>
        /// Tries to find the player with given actorNumber (a.k.a. ID).
        /// Only useful when in a Room, as IDs are only valid per Room.
        /// </summary>
        /// <param name="id">ID to look for.</param>
        /// <returns>The player with the ID or null.</returns>
        public virtual Player GetPlayer(int id)
        {
            Player result = null;
            this.Players.TryGetValue(id, out result);

            return result;
        }

        /// <summary>
        /// Attempts to remove all current expected users from the server's Slot Reservation list.
        /// </summary>
        /// <remarks>
        /// Note that this operation can conflict with new/other users joining. They might be
        /// adding users to the list of expected users before or after this client called ClearExpectedUsers.
        ///
        /// This room's expectedUsers value will update, when the server sends a successful update.
        ///
        /// Internals: This methods wraps up setting the ExpectedUsers property of a room.
        /// </remarks>
        public void ClearExpectedUsers()
        {
            Hashtable props = new Hashtable();
            props[GamePropertyKey.ExpectedUsers] = new string[0];
            Hashtable expected = new Hashtable();
            expected[GamePropertyKey.ExpectedUsers] = this.ExpectedUsers;
            this.LoadBalancingClient.OpSetPropertiesOfRoom(props, expected);
        }


        /// <summary>Returns a summary of this Room instance as string.</summary>
        /// <returns>Summary of this Room instance.</returns>
        public override string ToString()
        {
            return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", this.name, this.isVisible ? "visible" : "hidden", this.isOpen ? "open" : "closed", this.maxPlayers, this.PlayerCount);
        }

        /// <summary>Returns a summary of this Room instance as longer string, including Custom Properties.</summary>
        /// <returns>Summary of this Room instance.</returns>
        public new string ToStringFull()
        {
            return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", this.name, this.isVisible ? "visible" : "hidden", this.isOpen ? "open" : "closed", this.maxPlayers, this.PlayerCount, this.CustomProperties.ToStringFull());
        }
    }
}