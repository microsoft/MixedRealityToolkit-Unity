// ----------------------------------------------------------------------------
// <copyright file="Player.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Per client in a room, a Player is created. This client's Player is also
//   known as PhotonClient.LocalPlayer and the only one you might change
//   properties for.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

//#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
//#define UNITY
//#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    /// <summary>
    /// Summarizes a "player" within a room, identified (in that room) by ID (or "actorID").
    /// </summary>
    /// <remarks>
    /// Each player has a actorID, valid for that room. It's -1 until assigned by server (and client logic).
    /// </remarks>
    public class Player
    {
        /// <summary>
        /// Used internally to identify the masterclient of a room.
        /// </summary>
        protected internal Room RoomReference { get; set; }


        /// <summary>Backing field for property.</summary>
        private int actorID = -1;

        /// <summary>Identifier of this player in current room. Also known as: actorNumber or actorID. It's -1 outside of rooms.</summary>
        /// <remarks>The ID is assigned per room and only valid in that context. It will change even on leave and re-join. IDs are never re-used per room.</remarks>
        public int ID
        {
            get { return this.actorID; }
        }


        /// <summary>Only one player is controlled by each client. Others are not local.</summary>
        public readonly bool IsLocal;


        /// <summary>Background field for nickName.</summary>
        private string nickName;

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
                if (this.IsLocal && this.RoomReference != null && this.RoomReference.IsLocalClientInside)
                {
                    this.SetPlayerNameProperty();
                }
            }
        }


        /// <summary>
        /// The player with the lowest actorID is the master and could be used for special tasks.
        /// The LoadBalancingClient.LocalPlayer is not master unless in a room (this is the only player which exists outside of rooms, to store a nickname).
        /// </summary>
        public bool IsMasterClient
        {
            get
            {
                if (this.RoomReference == null)
                {
                    return false;
                }

                return this.ID == this.RoomReference.MasterClientId;
            }
        }

        /// <summary>In turnbased games, other players might be inactive in a room. True when another player is not in the current room.</summary>
        public bool IsInactive { get; set; }

        /// <summary>Read-only cache for custom properties of player. Set via Player.SetCustomProperties.</summary>
        /// <remarks>
        /// Don't modify the content of this Hashtable. Use SetCustomProperties and the
        /// properties of this class to modify values. When you use those, the client will
        /// sync values with the server.
        /// </remarks>
        public Hashtable CustomProperties { get; private set; }

        /// <summary>Creates a Hashtable with all properties (custom and "well known" ones).</summary>
        /// <remarks>Creates new Hashtables each time used, so if used more often, cache this.</remarks>
        public Hashtable AllProperties
        {
            get
            {
                Hashtable allProps = new Hashtable();
                allProps.Merge(this.CustomProperties);
                allProps[ActorProperties.PlayerName] = this.nickName;
                return allProps;
            }
        }

		/// <summary>Custom object associated with this Player. Not synchronized!</summary>
		public object Tag;


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
            return GetNextFor(this.ID);
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
            return GetNextFor(currentPlayer.ID);
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


        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="nickName">NickName of the player (a "well known property").</param>
        /// <param name="actorID">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        protected internal Player(string nickName, int actorID, bool isLocal) : this(nickName, actorID, isLocal, null)
        {
        }

        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="nickName">NickName of the player (a "well known property").</param>
        /// <param name="actorID">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        /// <param name="playerProperties">A Hashtable of custom properties to be synced. Must use String-typed keys and serializable datatypes as values.</param>
        protected internal Player(string nickName, int actorID, bool isLocal, Hashtable playerProperties)
        {
            this.IsLocal = isLocal;
            this.actorID = actorID;
            this.NickName = nickName;

            this.CustomProperties = new Hashtable();
            this.CacheProperties(playerProperties);
        }

        /// <summary>Caches properties for new Players or when updates of remote players are received. Use SetCustomProperties() for a synced update.</summary>
        /// <remarks>
        /// This only updates the CustomProperties and doesn't send them to the server.
        /// Mostly used when creating new remote players, where the server sends their properties.
        /// </remarks>
        public virtual void CacheProperties(Hashtable properties)
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

            if (properties.ContainsKey(ActorProperties.IsInactive))
            {
                this.IsInactive = (bool)properties[ActorProperties.IsInactive]; //TURNBASED new well-known propery for players
            }

            this.CustomProperties.MergeStringKeys(properties);
        }

        /// <summary>
        /// This Player's NickName and custom properties as string.
        /// </summary>
        public override string ToString()
        {
            return this.NickName + " " + SupportClass.DictionaryToString(this.CustomProperties);
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
            return this.ID;
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

            this.actorID = newID;
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
        /// <param name="propertiesToSet">Hashtable of Custom Properties that changes.</param>
        /// <param name="expectedProperties">Provide some keys/values to use as condition for setting the new values. Client must be in room.</param>
        /// <param name="webFlags">Defines if this SetCustomProperties-operation gets forwarded to your WebHooks. Client must be in room.</param>
        public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            Hashtable customProps = propertiesToSet.StripToStringKeys() as Hashtable;

            // merge (and delete null-values), unless we use CAS (expected props)
            if (expectedProperties == null)
            {
                this.CustomProperties.Merge(customProps);
                this.CustomProperties.StripKeysWithNullValues();
            }

            // send (sync) these new values if in room
            if (this.RoomReference != null && this.RoomReference.IsLocalClientInside)
            {
                this.RoomReference.LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfActor(this.actorID, customProps, expectedProperties, webFlags);
            }
        }


        /// <summary>Uses OpSetPropertiesOfActor to sync this player's NickName (server is being updated with this.NickName).</summary>
        private void SetPlayerNameProperty()
        {
            if (this.RoomReference != null && this.RoomReference.IsLocalClientInside)
            {
                Hashtable properties = new Hashtable();
                properties[ActorProperties.PlayerName] = this.nickName;
                this.RoomReference.LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfActor(this.ID, properties);
            }
        }
    }
}