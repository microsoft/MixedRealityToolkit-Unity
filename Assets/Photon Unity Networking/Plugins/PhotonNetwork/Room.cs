// ----------------------------------------------------------------------------
// <copyright file="Room.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Represents a room/game on the server and caches the properties of that.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using ExitGames.Client.Photon;
using UnityEngine;


/// <summary>
/// This class resembles a room that PUN joins (or joined).
/// The properties are settable as opposed to those of a RoomInfo and you can close or hide "your" room.
/// </summary>
/// \ingroup publicApi
public class Room : RoomInfo
{


    /// <summary>The name of a room. Unique identifier (per Loadbalancing group) for a room/match.</summary>
    public new string name
    {
        get
        {
            return this.nameField;
        }

        internal set
        {
            this.nameField = value;
        }
    }

    /// <summary>
    /// Defines if the room can be joined.
    /// This does not affect listing in a lobby but joining the room will fail if not open.
    /// If not open, the room is excluded from random matchmaking.
    /// Due to racing conditions, found matches might become closed before they are joined.
    /// Simply re-connect to master and find another.
    /// Use property "visible" to not list the room.
    /// </summary>
    public new bool open
    {
        get
        {
            return this.openField;
        }

        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                UnityEngine.Debug.LogWarning("Can't set open when not in that room.");
            }

            if (value != this.openField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.IsOpen, value } }, expectedProperties: null, webForward: false);
            }

            this.openField = value;
        }
    }

    /// <summary>
    /// Defines if the room is listed in its lobby.
    /// Rooms can be created invisible, or changed to invisible.
    /// To change if a room can be joined, use property: open.
    /// </summary>
    public new bool visible
    {
        get
        {
            return this.visibleField;
        }

        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                UnityEngine.Debug.LogWarning("Can't set visible when not in that room.");
            }

            if (value != this.visibleField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.IsVisible, value } }, expectedProperties: null, webForward: false);
            }

            this.visibleField = value;
        }
    }

    /// <summary>
    /// A list of custom properties that should be forwarded to the lobby and listed there.
    /// </summary>
    public string[] propertiesListedInLobby { get; private set; }

    /// <summary>
    /// Gets if this room uses autoCleanUp to remove all (buffered) RPCs and instantiated GameObjects when a player leaves.
    /// </summary>
    public bool autoCleanUp
    {
        get
        {
            return this.autoCleanUpField;
        }
    }


    /// <summary>
    /// Sets a limit of players to this room. This property is shown in lobby, too.
    /// If the room is full (players count == maxplayers), joining this room will fail.
    /// </summary>
    public new int maxPlayers
    {
        get
        {
            return (int)this.maxPlayersField;
        }

        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                UnityEngine.Debug.LogWarning("Can't set MaxPlayers when not in that room.");
            }

            if (value > 255)
            {
                UnityEngine.Debug.LogWarning("Can't set Room.MaxPlayers to: " + value + ". Using max value: 255.");
                value = 255;
            }

            if (value != this.maxPlayersField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.MaxPlayers, (byte)value } }, expectedProperties: null, webForward: false);
            }

            this.maxPlayersField = (byte)value;
        }
    }


    /// <summary>Count of players in this room.</summary>
    public new int playerCount
    {
        get
        {
            if (PhotonNetwork.playerList != null)
            {
                return PhotonNetwork.playerList.Length;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// List of users who are expected to join this room. In matchmaking, Photon blocks a slot for each of these UserIDs out of the MaxPlayers.
    /// </summary>
    /// <remarks>
    /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
    /// Define expected players in the PhotonNetwork methods: CreateRoom, JoinRoom and JoinOrCreateRoom.
    /// </remarks>
    public string[] expectedUsers
    {
        get { return this.expectedUsersField; }
    }

	/// <summary>The ID (actorNumber) of the current Master Client of this room.</summary>
    /// <remarks>See also: PhotonNetwork.masterClient.</remarks>
    protected internal int masterClientId
    {
        get
        {
            return this.masterClientIdField;
        }
        set
        {
            this.masterClientIdField = value;
        }
    }


    internal Room(string roomName, RoomOptions options) : base(roomName, null)
    {
        if (options == null)
        {
            options = new RoomOptions();
        }

        this.visibleField = options.IsVisible;
        this.openField = options.IsOpen;
        this.maxPlayersField = (byte)options.MaxPlayers;
        this.autoCleanUpField = false;  // defaults to false, unless set to true when room gets created.

        this.InternalCacheProperties(options.CustomRoomProperties);
        this.propertiesListedInLobby = options.CustomRoomPropertiesForLobby;
    }


    /// <summary>
    /// Updates the current room's Custom Properties with new/updated key-values.
    /// </summary>
    /// <remarks>
    /// Custom Properties are a key-value set (Hashtable) which is available to all players in a room.
    /// They can relate to the room or individual players and are useful when only the current value
    /// of something is of interest. For example: The map of a room.
    /// All keys must be strings.
    ///
    /// The Room and the PhotonPlayer class both have SetCustomProperties methods.
    /// Also, both classes offer access to current key-values by: customProperties.
    ///
    /// Always use SetCustomProperties to change values.
    /// To reduce network traffic, set only values that actually changed.
    /// New properties are added, existing values are updated.
    /// Other values will not be changed, so only provide values that changed or are new.
    ///
    /// To delete a named (custom) property of this room, use null as value.
    ///
    /// Locally, SetCustomProperties will update it's cache without delay.
    /// Other clients are updated through Photon (the server) with a fitting operation.
    ///
    /// <b>Check and Swap</b>
    ///
    /// SetCustomProperties have the option to do a server-side Check-And-Swap (CAS):
    /// Values only get updated if the expected values are correct.
    /// The expectedValues can be different key/values than the propertiesToSet. So you can
    /// check some key and set another key's value (if the check succeeds).
    ///
    /// If the client's knowledge of properties is wrong or outdated, it can't set values with CAS.
    /// This can be useful to keep players from concurrently setting values. For example: If all players
    /// try to pickup some card or item, only one should get it. With CAS, only the first SetProperties
    /// gets executed server-side and any other (sent at the same time) fails.
    ///
    /// The server will broadcast successfully changed values and the local "cache" of customProperties
    /// only gets updated after a roundtrip (if anything changed).
    ///
    /// You can do a "webForward": Photon will send the changed properties to a WebHook defined
    /// for your application.
    ///
    /// <b>OfflineMode</b>
    ///
    /// While PhotonNetwork.offlineMode is true, the expectedValues and webForward parameters are ignored.
    /// In OfflineMode, the local customProperties values are immediately updated (without the roundtrip).
    /// </remarks>
    /// <param name="propertiesToSet">The new properties to be set. </param>
    /// <param name="expectedValues">At least one property key/value set to check server-side. Key and value must be correct. Ignored in OfflineMode.</param>
    /// <param name="webForward">Set to true, to forward the set properties to a WebHook, defined for this app (in Dashboard). Ignored in OfflineMode.</param>
    public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
    {
        if (propertiesToSet == null)
        {
            return;
        }

        Hashtable customProps = propertiesToSet.StripToStringKeys() as Hashtable;
        Hashtable customPropsToCheck = expectedValues.StripToStringKeys() as Hashtable;


        // no expected values -> set and callback
        bool noCas = customPropsToCheck == null || customPropsToCheck.Count == 0;



        if (!PhotonNetwork.offlineMode)
        {
            PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(customProps, customPropsToCheck, webForward);
        }

        if (PhotonNetwork.offlineMode || noCas)
        {
            this.InternalCacheProperties(customProps);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, customProps);
        }
    }

    /// <summary>
    /// Enables you to define the properties available in the lobby if not all properties are needed to pick a room.
    /// </summary>
    /// <remarks>
    /// It makes sense to limit the amount of properties sent to users in the lobby as this improves speed and stability.
    /// </remarks>
    /// <param name="propsListedInLobby">An array of custom room property names to forward to the lobby.</param>
    public void SetPropertiesListedInLobby(string[] propsListedInLobby)
    {
        Hashtable customProps = new Hashtable();
        customProps[GamePropertyKey.PropsListedInLobby] = propsListedInLobby;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(customProps, expectedProperties: null, webForward: false);

        this.propertiesListedInLobby = propsListedInLobby;
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
        expected[GamePropertyKey.ExpectedUsers] = this.expectedUsers;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(props, expected, webForward: false);
    }


    /// <summary>Returns a summary of this Room instance as string.</summary>
    /// <returns>Summary of this Room instance.</returns>
    public override string ToString()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", this.nameField, this.visibleField ? "visible" : "hidden", this.openField ? "open" : "closed", this.maxPlayersField, this.playerCount);
    }

    /// <summary>Returns a summary of this Room instance as longer string, including Custom Properties.</summary>
    /// <returns>Summary of this Room instance.</returns>
    public new string ToStringFull()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", this.nameField, this.visibleField ? "visible" : "hidden", this.openField ? "open" : "closed", this.maxPlayersField, this.playerCount, this.customProperties.ToStringFull());
    }
}
