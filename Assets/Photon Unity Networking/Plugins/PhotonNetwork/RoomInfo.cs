// ----------------------------------------------------------------------------
// <copyright file="RoomInfo.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   This class resembles info about available rooms, as sent by the Master
//   server's lobby. Consider all values as readonly.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using ExitGames.Client.Photon;


/// <summary>
/// A simplified room with just the info required to list and join, used for the room listing in the lobby.
/// The properties are not settable (open, MaxPlayers, etc).
/// </summary>
/// <remarks>
/// This class resembles info about available rooms, as sent by the Master server's lobby.
/// Consider all values as readonly. None are synced (only updated by events by server).
/// </remarks>
/// \ingroup publicApi
public class RoomInfo
{
    /// <summary>Used internally in lobby, to mark rooms that are no longer listed.</summary>
    public bool removedFromList { get; internal set; }

    /// <summary>Backing field for property.</summary>
    private Hashtable customPropertiesField = new Hashtable();

    /// <summary>Backing field for property.</summary>
    protected byte maxPlayersField = 0;

    /// <summary>Backing field for property.</summary>
    protected int emptyRoomTtlField = 0;

    /// <summary>Backing field for property.</summary>
    protected int playerTtlField = 0;

    /// <summary>Backing field for property.</summary>
    protected string[] expectedUsersField;

    /// <summary>Backing field for property.</summary>
    protected bool openField = true;

    /// <summary>Backing field for property.</summary>
    protected bool visibleField = true;

    /// <summary>Backing field for property. False unless the GameProperty is set to true (else it's not sent).</summary>
    protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

    /// <summary>Backing field for property.</summary>
    protected string nameField;

    /// <summary>Backing field for master client id (actorNumber). defined by server in room props and ev leave.</summary>
    protected internal int masterClientIdField;

    protected internal bool serverSideMasterClient { get; private set; }

    /// <summary>Read-only "cache" of custom properties of a room. Set via Room.SetCustomProperties (not available for RoomInfo class!).</summary>
    /// <remarks>All keys are string-typed and the values depend on the game/application.</remarks>
    /// <see cref="Room.SetCustomProperties"/>
    public Hashtable CustomProperties
    {
        get
        {
            return this.customPropertiesField;
        }
    }

    /// <summary>The name of a room. Unique identifier (per Loadbalancing group) for a room/match.</summary>
    public string Name
    {
        get
        {
            return this.nameField;
        }
    }

    /// <summary>
    /// Only used internally in lobby, to display number of players in room (while you're not in).
    /// </summary>
    public int PlayerCount { get; private set; }

    /// <summary>
    /// State if the local client is already in the game or still going to join it on gameserver (in lobby always false).
    /// </summary>
    public bool IsLocalClientInside { get; set; }

    /// <summary>
    /// Sets a limit of players to this room. This property is shown in lobby, too.
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
            return this.maxPlayersField;
        }
    }

    /// <summary>
    /// Defines if the room can be joined.
    /// This does not affect listing in a lobby but joining the room will fail if not open.
    /// If not open, the room is excluded from random matchmaking.
    /// Due to racing conditions, found matches might become closed before they are joined.
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
            return this.openField;
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
            return this.visibleField;
        }
    }

    /// <summary>
    /// Constructs a RoomInfo to be used in room listings in lobby.
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="properties"></param>
    protected internal RoomInfo(string roomName, Hashtable properties)
    {
        this.InternalCacheProperties(properties);

        this.nameField = roomName;
    }

    /// <summary>
    /// Makes RoomInfo comparable (by name).
    /// </summary>
    public override bool Equals(object other)
    {
        RoomInfo otherRoomInfo = other as RoomInfo;
        return (otherRoomInfo != null && this.Name.Equals(otherRoomInfo.nameField));
    }

    /// <summary>
    /// Accompanies Equals, using the name's HashCode as return.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return this.nameField.GetHashCode();
    }


    /// <summary>Simple printingin method.</summary>
    /// <returns>Summary of this RoomInfo instance.</returns>
    public override string ToString()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", this.nameField, this.visibleField ? "visible" : "hidden", this.openField ? "open" : "closed", this.maxPlayersField, this.PlayerCount);
    }

    /// <summary>Simple printingin method.</summary>
    /// <returns>Summary of this RoomInfo instance.</returns>
    public string ToStringFull()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", this.nameField, this.visibleField ? "visible" : "hidden", this.openField ? "open" : "closed", this.maxPlayersField, this.PlayerCount, this.customPropertiesField.ToStringFull());
    }

    /// <summary>Copies "well known" properties to fields (IsVisible, etc) and caches the custom properties (string-keys only) in a local hashtable.</summary>
    /// <param name="propertiesToCache">New or updated properties to store in this RoomInfo.</param>
    protected internal void InternalCacheProperties(Hashtable propertiesToCache)
    {
        if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customPropertiesField.Equals(propertiesToCache))
        {
            return;
        }

        // check of this game was removed from the list. in that case, we don't
        // need to read any further properties
        // list updates will remove this game from the game listing
        if (propertiesToCache.ContainsKey(GamePropertyKey.Removed))
        {
            this.removedFromList = (Boolean)propertiesToCache[GamePropertyKey.Removed];
            if (this.removedFromList)
            {
                return;
            }
        }

        // fetch the "well known" properties of the room, if available
        if (propertiesToCache.ContainsKey(GamePropertyKey.MaxPlayers))
        {
            this.maxPlayersField = (byte)propertiesToCache[GamePropertyKey.MaxPlayers];
        }

        if (propertiesToCache.ContainsKey(GamePropertyKey.IsOpen))
        {
            this.openField = (bool)propertiesToCache[GamePropertyKey.IsOpen];
        }

        if (propertiesToCache.ContainsKey(GamePropertyKey.IsVisible))
        {
            this.visibleField = (bool)propertiesToCache[GamePropertyKey.IsVisible];
        }

        if (propertiesToCache.ContainsKey(GamePropertyKey.PlayerCount))
        {
            this.PlayerCount = (int)((byte)propertiesToCache[GamePropertyKey.PlayerCount]);
        }

        if (propertiesToCache.ContainsKey(GamePropertyKey.CleanupCacheOnLeave))
        {
            this.autoCleanUpField = (bool)propertiesToCache[GamePropertyKey.CleanupCacheOnLeave];
        }

        if (propertiesToCache.ContainsKey(GamePropertyKey.MasterClientId))
        {
            this.serverSideMasterClient = true;
            bool isUpdate = this.masterClientIdField != 0;
            this.masterClientIdField = (int) propertiesToCache[GamePropertyKey.MasterClientId];
            if (isUpdate)
            {
                PhotonNetwork.networkingPeer.UpdateMasterClient();
            }
        }

        //if (propertiesToCache.ContainsKey(GamePropertyKey.PropsListedInLobby))
        //{
        //    // could be cached but isn't useful
        //}

        if (propertiesToCache.ContainsKey((byte)GamePropertyKey.ExpectedUsers))
        {
            this.expectedUsersField = (string[])propertiesToCache[GamePropertyKey.ExpectedUsers];
        }

        if (propertiesToCache.ContainsKey((byte)GamePropertyKey.EmptyRoomTtl))
        {
            this.emptyRoomTtlField = (int)propertiesToCache[GamePropertyKey.EmptyRoomTtl];
        }

        if (propertiesToCache.ContainsKey((byte)GamePropertyKey.PlayerTtl))
        {
            this.playerTtlField = (int)propertiesToCache[GamePropertyKey.PlayerTtl];
        }

        // merge the custom properties (from your application) to the cache (only string-typed keys will be kept)
        this.customPropertiesField.MergeStringKeys(propertiesToCache);
        this.customPropertiesField.StripKeysWithNullValues();
    }


    #region Obsoleted variable names

    [Obsolete("Please use CustomProperties (updated case for naming).")]
    public Hashtable customProperties { get { return this.CustomProperties; } }

    [Obsolete("Please use Name (updated case for naming).")]
    public string name { get { return this.Name; } }

    [Obsolete("Please use PlayerCount (updated case for naming).")]
    public int playerCount { get { return this.PlayerCount; } set { this.PlayerCount = value; } }

    [Obsolete("Please use IsLocalClientInside (updated case for naming).")]
    public bool isLocalClientInside { get { return this.IsLocalClientInside; } set { this.IsLocalClientInside = value; } }

    [Obsolete("Please use MaxPlayers (updated case for naming).")]
    public byte maxPlayers { get { return this.MaxPlayers; } }

    [Obsolete("Please use IsOpen (updated case for naming).")]
    public bool open { get { return this.IsOpen; } }

    [Obsolete("Please use IsVisible (updated case for naming).")]
    public bool visible { get { return this.IsVisible; } }

    #endregion
}
