// ----------------------------------------------------------------------------
// <copyright file="Player.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Per client in a room, a Player is created. This client's Player is also
//   known as PhotonClient.LocalPlayer and the only one you might change
//   properties for.
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

    #if SUPPORTED_UNITY
    using UnityEngine;
    #endif
    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>
    /// Summarizes a "player" within a room, identified (in that room) by ID (or "actorNumber").
    /// </summary>
    /// <remarks>
    /// Each player has a actorNumber, valid for that room. It's -1 until assigned by server (and client logic).
    /// </remarks>
    public class Player
    {
        /// <summary>
        /// Used internally to identify the masterclient of a room.
        /// </summary>
        protected internal Room RoomReference { get; set; }


        /// <summary>Backing field for property.</summary>
        private int actorNumber = -1;

        /// <summary>Identifier of this player in current room. Also known as: actorNumber or actorNumber. It's -1 outside of rooms.</summary>
        /// <remarks>The ID is assigned per room and only valid in that context. It will change even on leave and re-join. IDs are never re-used per room.</remarks>
        public int ActorNumber
        {
            get { return this.actorNumber; }
        }


        /// <summary>Only one player is controlled by each client. Others are not local.</summary>
        public readonly bool IsLocal;


        /// <summary>Background field for nickName.</summary>
		private string nickName = string.Empty;

        /// <summary>Non-unique nickname of this player. Synced automatically in a room.</summary>
        /// <remarks>
        /// A player might change his own playername in a room (it's only a property).
        /// Setting this value updates the server and other players (using an operation).
        /// </remarks>
        public string NickName
        {
            get
            {
                return this.nickName;
            }
            set
            {
                if (!string.IsNullOrEmpty(this.nickName) && this.nickName.Equals(value))
                {
                    return;
                }

                this.nickName = value;

                // update a room, if we changed our nickName (locally, while being in a room)
                if (this.IsLocal && this.RoomReference != null)
                {
                    this.SetPlayerNameProperty();
                }
            }
        }

        /// <summary>UserId of the player, available when the room got created with RoomOptions.PublishUserId = true.</summary>
        /// <remarks>Useful for PhotonNetwork.FindFriends and blocking slots in a room for expected players (e.g. in PhotonNetwork.CreateRoom).</remarks>
        public string UserId { get; internal set; }

        /// <summary>
        /// True if this player is the Master Client of the current room.
        /// </summary>
        /// <remarks>
        /// See also: PhotonNetwork.MasterClient.
        /// </remarks>
        public bool IsMasterClient
        {
            get
            {
                if (this.RoomReference == null)
                {
                    return false;
                }

                return this.ActorNumber == this.RoomReference.MasterClientId;
            }
        }

        /// <summary>If this player is active in the room (and getting events which are currently being sent).</summary>
        /// <remarks>
        /// Inactive players keep their spot in a room but otherwise behave as if offline (no matter what their actual connection status is).
        /// The room needs a PlayerTTL != 0. If a player is inactive for longer than PlayerTTL, the server will remove this player from the room.
        /// For a client "rejoining" a room, is the same as joining it: It gets properties, cached events and then the live events.
        /// </remarks>
        public bool IsInactive { get; protected internal set; }

        /// <summary>Read-only cache for custom properties of player. Set via Player.SetCustomProperties.</summary>
        /// <remarks>
        /// Don't modify the content of this Hashtable. Use SetCustomProperties and the
        /// properties of this class to modify values. When you use those, the client will
        /// sync values with the server.
        /// </remarks>
        /// <see cref="SetCustomProperties"/>
        public Hashtable CustomProperties { get; set; }

        /// <summary>Can be used to store a reference that's useful to know "by player".</summary>
        /// <remarks>Example: Set a player's character as Tag by assigning the GameObject on Instantiate.</remarks>
        public object TagObject;


        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="nickName">NickName of the player (a "well known property").</param>
        /// <param name="actorNumber">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        protected internal Player(string nickName, int actorNumber, bool isLocal) : this(nickName, actorNumber, isLocal, null)
        {
        }

        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="nickName">NickName of the player (a "well known property").</param>
        /// <param name="actorNumber">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        /// <param name="playerProperties">A Hashtable of custom properties to be synced. Must use String-typed keys and serializable datatypes as values.</param>
        protected internal Player(string nickName, int actorNumber, bool isLocal, Hashtable playerProperties)
        {
            this.IsLocal = isLocal;
            this.actorNumber = actorNumber;
            this.NickName = nickName;

            this.CustomProperties = new Hashtable();
            this.InternalCacheProperties(playerProperties);
        }


        /// <summary>
        /// Get a Player by ActorNumber (Player.ID).
        /// </summary>
        /// <param name="id">ActorNumber of the a player in this room.</param>
        /// <returns>Player or null.</returns>
        public Player Get(int id)
        {
            if (this.RoomReference == null)
            {
                return null;
            }

            return this.RoomReference.GetPlayer(id);
        }

        /// <summary>Gets this Player's next Player, as sorted by ActorNumber (Player.ID). Wraps around.</summary>
        /// <returns>Player or null.</returns>
        public Player GetNext()
        {
            return GetNextFor(this.ActorNumber);
        }

        /// <summary>Gets a Player's next Player, as sorted by ActorNumber (Player.ID). Wraps around.</summary>
        /// <remarks>Useful when you pass something to the next player. For example: passing the turn to the next player.</remarks>
        /// <param name="currentPlayer">The Player for which the next is being needed.</param>
        /// <returns>Player or null.</returns>
        public Player GetNextFor(Player currentPlayer)
        {
            if (currentPlayer == null)
            {
                return null;
            }
            return GetNextFor(currentPlayer.ActorNumber);
        }

        /// <summary>Gets a Player's next Player, as sorted by ActorNumber (Player.ID). Wraps around.</summary>
        /// <remarks>Useful when you pass something to the next player. For example: passing the turn to the next player.</remarks>
        /// <param name="currentPlayerId">The ActorNumber (Player.ID) for which the next is being needed.</param>
        /// <returns>Player or null.</returns>
        public Player GetNextFor(int currentPlayerId)
        {
            if (this.RoomReference == null || this.RoomReference.Players == null || this.RoomReference.Players.Count < 2)
            {
                return null;
            }

            Dictionary<int, Player> players = this.RoomReference.Players;
            int nextHigherId = int.MaxValue;    // we look for the next higher ID
            int lowestId = currentPlayerId;     // if we are the player with the highest ID, there is no higher and we return to the lowest player's id

            foreach (int playerid in players.Keys)
            {
                if (playerid < lowestId)
                {
                    lowestId = playerid;        // less than any other ID (which must be at least less than this player's id).
                }
                else if (playerid > currentPlayerId && playerid < nextHigherId)
                {
                    nextHigherId = playerid;    // more than our ID and less than those found so far.
                }
            }

            //UnityEngine.Debug.LogWarning("Debug. " + currentPlayerId + " lower: " + lowestId + " higher: " + nextHigherId + " ");
            //UnityEngine.Debug.LogWarning(this.RoomReference.GetPlayer(currentPlayerId));
            //UnityEngine.Debug.LogWarning(this.RoomReference.GetPlayer(lowestId));
            //if (nextHigherId != int.MaxValue) UnityEngine.Debug.LogWarning(this.RoomReference.GetPlayer(nextHigherId));
            return (nextHigherId != int.MaxValue) ? players[nextHigherId] : players[lowestId];
        }


        /// <summary>Caches properties for new Players or when updates of remote players are received. Use SetCustomProperties() for a synced update.</summary>
        /// <remarks>
        /// This only updates the CustomProperties and doesn't send them to the server.
        /// Mostly used when creating new remote players, where the server sends their properties.
        /// </remarks>
        public virtual void InternalCacheProperties(Hashtable properties)
        {
            if (properties == null || properties.Count == 0 || this.CustomProperties.Equals(properties))
            {
                return;
            }

            if (properties.ContainsKey(ActorProperties.PlayerName))
            {
                string nameInServersProperties = (string)properties[ActorProperties.PlayerName];
                if (nameInServersProperties != null)
                {
                    if (this.IsLocal)
                    {
                        // the local playername is different than in the properties coming from the server
                        // so the local nickName was changed and the server is outdated -> update server
                        // update property instead of using the outdated nickName coming from server
                        if (!nameInServersProperties.Equals(this.nickName))
                        {
                            this.SetPlayerNameProperty();
                        }
                    }
                    else
                    {
                        this.NickName = nameInServersProperties;
                    }
                }
            }
            if (properties.ContainsKey(ActorProperties.UserId))
            {
                this.UserId = (string)properties[ActorProperties.UserId];
            }
            if (properties.ContainsKey(ActorProperties.IsInactive))
            {
                this.IsInactive = (bool)properties[ActorProperties.IsInactive]; //TURNBASED new well-known propery for players
            }

            this.CustomProperties.MergeStringKeys(properties);
            this.CustomProperties.StripKeysWithNullValues();
        }


        /// <summary>
        /// Brief summary string of the Player. Includes name or player.ID and if it's the Master Client.
        /// </summary>
        public override string ToString()
        {
            return (string.IsNullOrEmpty(this.NickName) ? this.ActorNumber.ToString() : this.nickName) + " " + SupportClass.DictionaryToString(this.CustomProperties);
        }

        /// <summary>
        /// String summary of the Player: player.ID, name and all custom properties of this user.
        /// </summary>
        /// <remarks>
        /// Use with care and not every frame!
        /// Converts the customProperties to a String on every single call.
        /// </remarks>
        public string ToStringFull()
        {
            return string.Format("#{0:00} '{1}'{2} {3}", this.ActorNumber, this.NickName, this.IsInactive ? " (inactive)" : "", this.CustomProperties.ToStringFull());
        }

        /// <summary>
        /// If players are equal (by GetHasCode, which returns this.ID).
        /// </summary>
        public override bool Equals(object p)
        {
            Player pp = p as Player;
            return (pp != null && this.GetHashCode() == pp.GetHashCode());
        }

        /// <summary>
        /// Accompanies Equals, using the ID (actorNumber) as HashCode to return.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ActorNumber;
        }

        /// <summary>
        /// Used internally, to update this client's playerID when assigned (doesn't change after assignment).
        /// </summary>
        protected internal void ChangeLocalID(int newID)
        {
            if (!this.IsLocal)
            {
                //Debug.LogError("ERROR You should never change Player IDs!");
                return;
            }

            this.actorNumber = newID;
        }



        /// <summary>
        /// Updates and synchronizes this Player's Custom Properties. Optionally, expectedProperties can be provided as condition.
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
        /// <param name="propertiesToSet">Hashtable of Custom Properties to be set. </param>
        /// <param name="expectedValues">If non-null, these are the property-values the server will check as condition for this update.</param>
        /// <param name="webFlags">Defines if this SetCustomProperties-operation gets forwarded to your WebHooks. Client must be in room.</param>
        public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, WebFlags webFlags = null)
        {
            if (propertiesToSet == null)
            {
                return;
            }

            Hashtable customProps = propertiesToSet.StripToStringKeys() as Hashtable;
            Hashtable customPropsToCheck = expectedValues.StripToStringKeys() as Hashtable;


            // no expected values -> set and callback
            bool noCas = customPropsToCheck == null || customPropsToCheck.Count == 0;


            if (noCas)
            {
                this.CustomProperties.Merge(customProps);
                this.CustomProperties.StripKeysWithNullValues();
            }

            if (this.RoomReference != null)
            {
                if (this.RoomReference.IsOffline)
                {
                    // invoking callbacks
                    this.RoomReference.LoadBalancingClient.InRoomCallbackTargets.OnPlayerPropertiesUpdate(this, customProps);
                }
                else
                {
                    // send (sync) these new values if in online room
                    this.RoomReference.LoadBalancingClient.LoadBalancingPeer.OpSetPropertiesOfActor(this.actorNumber, customProps, customPropsToCheck, webFlags);
                }
            }
        }


        /// <summary>Uses OpSetPropertiesOfActor to sync this player's NickName (server is being updated with this.NickName).</summary>
        private void SetPlayerNameProperty()
        {
            if (this.RoomReference != null)
            {
                Hashtable properties = new Hashtable();
                properties[ActorProperties.PlayerName] = this.nickName;
                this.RoomReference.LoadBalancingClient.LoadBalancingPeer.OpSetPropertiesOfActor(this.ActorNumber, properties);
            }
        }
    }
}