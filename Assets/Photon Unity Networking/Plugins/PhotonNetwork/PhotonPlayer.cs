// ----------------------------------------------------------------------------
// <copyright file="PhotonPlayer.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Represents a player, identified by actorID (a.k.a. ActorNumber).
//   Caches properties of a player.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary>
/// Summarizes a "player" within a room, identified (in that room) by actorID.
/// </summary>
/// <remarks>
/// Each player has an actorId (or ID), valid for that room. It's -1 until it's assigned by server.
/// Each client can set it's player's custom properties with SetCustomProperties, even before being in a room.
/// They are synced when joining a room.
/// </remarks>
/// \ingroup publicApi
public class PhotonPlayer : IComparable<PhotonPlayer>, IComparable<int>, IEquatable<PhotonPlayer>, IEquatable<int>
{
    /// <summary>This player's actorID</summary>
    public int ID
    {
        get { return this.actorID; }
    }

    /// <summary>Identifier of this player in current room.</summary>
    private int actorID = -1;

    private string nameField = "";

    /// <summary>Nickname of this player.</summary>
    /// <remarks>Set the PhotonNetwork.playerName to make the name synchronized in a room.</remarks>
    public string name {
        get
        {
            return this.nameField;
        }
        set
        {
            if (!isLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
                return;
            }
            if (string.IsNullOrEmpty(value) || value.Equals(this.nameField))
            {
                return;
            }

            this.nameField = value;
            PhotonNetwork.playerName = value;   // this will sync the local player's name in a room
        }
    }

    /// <summary>UserId of the player, available when the room got created with RoomOptions.PublishUserId = true.</summary>
    /// <remarks>Useful for PhotonNetwork.FindFriends and blocking slots in a room for expected players (e.g. in PhotonNetwork.CreateRoom).</remarks>
    public string userId { get; internal set; }

    /// <summary>Only one player is controlled by each client. Others are not local.</summary>
    public readonly bool isLocal = false;

    /// <summary>
    /// True if this player is the Master Client of the current room.
    /// </summary>
    /// <remarks>
    /// See also: PhotonNetwork.masterClient.
    /// </remarks>
    public bool isMasterClient
    {
        get { return (PhotonNetwork.networkingPeer.mMasterClientId == this.ID); }
    }


    /// <summary>Players might be inactive in a room when PlayerTTL for a room is > 0. If true, the player is not getting events from this room (now) but can return later.</summary>
    public bool isInactive { get; set; }    // needed for rejoins


    /// <summary>Read-only cache for custom properties of player. Set via PhotonPlayer.SetCustomProperties.</summary>
    /// <remarks>
    /// Don't modify the content of this Hashtable. Use SetCustomProperties and the
    /// properties of this class to modify values. When you use those, the client will
    /// sync values with the server.
    /// </remarks>
    /// <see cref="SetCustomProperties"/>
    public Hashtable customProperties { get; internal set; }

    /// <summary>Creates a Hashtable with all properties (custom and "well known" ones).</summary>
    /// <remarks>If used more often, this should be cached.</remarks>
    public Hashtable allProperties
    {
        get
        {
            Hashtable allProps = new Hashtable();
            allProps.Merge(this.customProperties);
            allProps[ActorProperties.PlayerName] = this.name;
            return allProps;
        }
    }

    /// <summary>Can be used to store a reference that's useful to know "by player".</summary>
    /// <remarks>Example: Set a player's character as Tag by assigning the GameObject on Instantiate.</remarks>
    public object TagObject;


    /// <summary>
    /// Creates a PhotonPlayer instance.
    /// </summary>
    /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
    /// <param name="actorID">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
    /// <param name="name">Name of the player (a "well known property").</param>
    public PhotonPlayer(bool isLocal, int actorID, string name)
    {
        this.customProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;
        this.nameField = name;
    }

    /// <summary>
    /// Internally used to create players from event Join
    /// </summary>
    internal protected PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
    {
        this.customProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;

        this.InternalCacheProperties(properties);
    }

    /// <summary>
    /// Makes PhotonPlayer comparable
    /// </summary>
    public override bool Equals(object p)
    {
        PhotonPlayer pp = p as PhotonPlayer;
        return (pp != null && this.GetHashCode() == pp.GetHashCode());
    }

    public override int GetHashCode()
    {
        return this.ID;
    }

    /// <summary>
    /// Used internally, to update this client's playerID when assigned.
    /// </summary>
    internal void InternalChangeLocalID(int newID)
    {
        if (!this.isLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
            return;
        }

        this.actorID = newID;
    }

    /// <summary>
    /// Caches custom properties for this player.
    /// </summary>
    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties == null || properties.Count == 0 || this.customProperties.Equals(properties))
        {
            return;
        }

        if (properties.ContainsKey(ActorProperties.PlayerName))
        {
            this.nameField = (string)properties[ActorProperties.PlayerName];
        }
        if (properties.ContainsKey(ActorProperties.UserId))
        {
            this.userId = (string)properties[ActorProperties.UserId];
        }
        if (properties.ContainsKey(ActorProperties.IsInactive))
        {
            this.isInactive = (bool)properties[ActorProperties.IsInactive]; //TURNBASED new well-known propery for players
        }

        this.customProperties.MergeStringKeys(properties);
        this.customProperties.StripKeysWithNullValues();
    }


    /// <summary>
    /// Updates the this player's Custom Properties with new/updated key-values.
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
        bool inOnlineRoom = this.actorID > 0 && !PhotonNetwork.offlineMode;


        if (inOnlineRoom)
        {
            PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(this.actorID, customProps, customPropsToCheck, webForward);
        }

        if (!inOnlineRoom || noCas)
        {
            this.InternalCacheProperties(customProps);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, customProps);
        }
    }

    /// <summary>
    /// Try to get a specific player by id.
    /// </summary>
    /// <param name="ID">ActorID</param>
    /// <returns>The player with matching actorID or null, if the actorID is not in use.</returns>
    public static PhotonPlayer Find(int ID)
    {
        if (PhotonNetwork.networkingPeer != null)
        {
            return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
        }
        return null;
    }

    public PhotonPlayer Get(int id)
    {
        return PhotonPlayer.Find(id);
    }

    public PhotonPlayer GetNext()
    {
        return GetNextFor(this.ID);
    }

    public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return GetNextFor(currentPlayer.ID);
    }

    public PhotonPlayer GetNextFor(int currentPlayerId)
    {
        if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
        {
            return null;
        }

        Dictionary<int, PhotonPlayer> players = PhotonNetwork.networkingPeer.mActors;
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

	#region IComparable implementation

	public int CompareTo (PhotonPlayer other)
	{
		if ( other == null)
		{
			return 0;
		}

		return this.GetHashCode().CompareTo(other.GetHashCode());
	}

	public int CompareTo (int other)
	{
		return this.GetHashCode().CompareTo(other);
	}

	#endregion

	#region IEquatable implementation

	public bool Equals (PhotonPlayer other)
	{
		if ( other == null)
		{
			return false;
		}
		
		return this.GetHashCode().Equals(other.GetHashCode());
	}

	public bool Equals (int other)
	{	
		return this.GetHashCode().Equals(other);
	}

	#endregion

    /// <summary>
    /// Brief summary string of the PhotonPlayer. Includes name or player.ID and if it's the Master Client.
    /// </summary>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(this.name))
        {
            return string.Format("#{0:00}{1}{2}",  this.ID, this.isInactive ? " (inactive)" : " ", this.isMasterClient ? "(master)":"");
        }

        return string.Format("'{0}'{1}{2}", this.name, this.isInactive ? " (inactive)" : " ", this.isMasterClient ? "(master)" : "");
    }

    /// <summary>
    /// String summary of the PhotonPlayer: player.ID, name and all custom properties of this user.
    /// </summary>
    /// <remarks>
    /// Use with care and not every frame!
    /// Converts the customProperties to a String on every single call.
    /// </remarks>
    public string ToStringFull()
    {
        return string.Format("#{0:00} '{1}'{2} {3}", this.ID, this.name, this.isInactive ? " (inactive)" : "", this.customProperties.ToStringFull());
    }
}
