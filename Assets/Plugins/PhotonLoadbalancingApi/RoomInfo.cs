// ----------------------------------------------------------------------------
// <copyright file="RoomInfo.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   This class resembles info about available rooms, as sent by the Master
//   server's lobby. Consider all values as readonly.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System.Collections;

    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    /// <summary>
    /// Used for Room listings of the lobby (not yet joining). Offers the basic info about a
    /// room: name, player counts, properties, etc.
    /// </summary>
    /// <remarks>
    /// This class resembles info about available rooms, as sent by the Master server's lobby.
    /// Consider all values as readonly. None are synced (only updated by events by server).
    /// </remarks>
    public class RoomInfo
    {

        /// <summary>Used internally in lobby, to mark rooms that are no longer listed (for being full, closed or hidden).</summary>
        protected internal bool removedFromList;

        /// <summary>Backing field for property.</summary>
        private Hashtable customProperties = new Hashtable();

        /// <summary>Backing field for property.</summary>
        protected byte maxPlayers = 0;

        /// <summary>Backing field for property.</summary>
        protected string[] expectedUsersField;

        /// <summary>Backing field for property.</summary>
        protected bool isOpen = true;

        /// <summary>Backing field for property.</summary>
        protected bool isVisible = true;

        /// <summary>Backing field for property.</summary>
        protected string name;

        /// <summary>Backing field for master client id (actorNumber). defined by server in room props and ev leave.</summary>
        protected internal int masterClientIdField;

        protected internal bool serverSideMasterClient { get; private set; }

        /// <summary>Backing field for property.</summary>
        protected string[] propsListedInLobby;

        /// <summary>Read-only "cache" of custom properties of a room. Set via Room.SetCustomProperties.</summary>
        /// <remarks>All keys are string-typed and the values depend on the game/application.</remarks>
        public Hashtable CustomProperties
        {
            get
            {
                return this.customProperties;
            }
        }

        /// <summary>The name of a room. Unique identifier for a room/match (per AppId + game-Version).</summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Count of players currently in room. This property is overwritten by the Room class (used when you're in a Room).
        /// </summary>
        public int PlayerCount { get; private set; }

        /// <summary>
        /// State if the local client is already in the game or still going to join it on gameserver (in lobby: false).
        /// </summary>
        public bool IsLocalClientInside { get; set; }

        /// <summary>
        /// The limit of players for this room. This property is shown in lobby, too.
        /// If the room is full (players count == maxplayers), joining this room will fail.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public byte MaxPlayers
        {
            get
            {
                return this.maxPlayers;
            }
        }

        /// <summary>
        /// Defines if the room can be joined.
        /// This does not affect listing in a lobby but joining the room will fail if not open.
        /// If not open, the room is excluded from random matchmaking.
        /// Due to racing conditions, found matches might become closed even while you join them.
        /// Simply re-connect to master and find another.
        /// Use property "IsVisible" to not list the room.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
        }

        /// <summary>
        /// Defines if the room is listed in its lobby.
        /// Rooms can be created invisible, or changed to invisible.
        /// To change if a room can be joined, use property: open.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
        }

        /// <summary>
        /// Constructs a RoomInfo to be used in room listings in lobby.
        /// </summary>
        /// <param name="roomName">Name of the room and unique ID at the same time.</param>
        /// <param name="roomProperties">Properties for this room.</param>
        protected internal RoomInfo(string roomName, Hashtable roomProperties)
        {
            this.CacheProperties(roomProperties);

            this.name = roomName;
        }

        /// <summary>
        /// Makes RoomInfo comparable (by name).
        /// </summary>
        public override bool Equals(object other)
        {
            RoomInfo otherRoomInfo = other as RoomInfo;
            return (otherRoomInfo != null && this.name.Equals(otherRoomInfo.name));
        }

        /// <summary>
        /// Accompanies Equals, using the name's HashCode as return.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }


        /// <summary>Returns most interesting room values as string.</summary>
        /// <returns>Summary of this RoomInfo instance.</returns>
        public override string ToString()
        {
            return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", this.name, this.isVisible ? "visible" : "hidden", this.isOpen ? "open" : "closed", this.maxPlayers, this.PlayerCount);
        }

        /// <summary>Returns most interesting room values as string, including custom properties.</summary>
        /// <returns>Summary of this RoomInfo instance.</returns>
        public string ToStringFull()
        {
            return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", this.name, this.isVisible ? "visible" : "hidden", this.isOpen ? "open" : "closed", this.maxPlayers, this.PlayerCount, SupportClass.DictionaryToString(this.customProperties));
        }


        /// <summary>Copies "well known" properties to fields (isVisible, etc) and caches the custom properties (string-keys only) in a local hashtable.</summary>
        /// <param name="propertiesToCache">New or updated properties to store in this RoomInfo.</param>
        protected internal virtual void CacheProperties(Hashtable propertiesToCache)
        {
            if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customProperties.Equals(propertiesToCache))
            {
                return;
            }

            // check of this game was removed from the list. in that case, we don't
            // need to read any further properties
            // list updates will remove this game from the game listing
            if (propertiesToCache.ContainsKey(GamePropertyKey.Removed))
            {
                this.removedFromList = (bool)propertiesToCache[GamePropertyKey.Removed];
                if (this.removedFromList)
                {
                    return;
                }
            }

            // fetch the "well known" properties of the room, if available
            if (propertiesToCache.ContainsKey(GamePropertyKey.MaxPlayers))
            {
                this.maxPlayers = (byte)propertiesToCache[GamePropertyKey.MaxPlayers];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.IsOpen))
            {
                this.isOpen = (bool)propertiesToCache[GamePropertyKey.IsOpen];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.IsVisible))
            {
                this.isVisible = (bool)propertiesToCache[GamePropertyKey.IsVisible];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.PlayerCount))
            {
                this.PlayerCount = (int)((byte)propertiesToCache[GamePropertyKey.PlayerCount]);
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.MasterClientId))
            {
                this.serverSideMasterClient = true;
                this.masterClientIdField = (int)propertiesToCache[GamePropertyKey.MasterClientId];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.PropsListedInLobby))
            {
                this.propsListedInLobby = propertiesToCache[GamePropertyKey.PropsListedInLobby] as string[];
            }

            if (propertiesToCache.ContainsKey((byte)GamePropertyKey.ExpectedUsers))
            {
                this.expectedUsersField = (string[])propertiesToCache[GamePropertyKey.ExpectedUsers];
            }

            // merge the custom properties (from your application) to the cache (only string-typed keys will be kept)
            this.customProperties.MergeStringKeys(propertiesToCache);
        }
    }
}
