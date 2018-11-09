// ----------------------------------------------------------------------------
// <copyright file="PhotonClasses.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Wraps up smaller classes that don't need their own file. </summary>
///
///
/// \defgroup publicApi Public API
/// \brief Groups the most important classes that you need to understand early on.
///
/// \defgroup optionalGui Optional Gui Elements
/// \brief Useful GUI elements for PUN.
#pragma warning restore 1587

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 || UNITY_5_4_OR_NEWER
#define UNITY_MIN_5_3
#endif

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;


/// <summary>Defines the OnPhotonSerializeView method to make it easy to implement correctly for observable scripts.</summary>
/// \ingroup publicApi
public interface IPunObservable
{
    /// <summary>
    /// Called by PUN several times per second, so that your script can write and read synchronization data for the PhotonView.
    /// </summary>
    /// <remarks>
    /// This method will be called in scripts that are assigned as Observed component of a PhotonView.<br/>
    /// PhotonNetwork.sendRateOnSerialize affects how often this method is called.<br/>
    /// PhotonNetwork.sendRate affects how often packages are sent by this client.<br/>
    ///
    /// Implementing this method, you can customize which data a PhotonView regularly synchronizes.
    /// Your code defines what is being sent (content) and how your data is used by receiving clients.
    ///
    /// Unlike other callbacks, <i>OnPhotonSerializeView only gets called when it is assigned
    /// to a PhotonView</i> as PhotonView.observed script.
    ///
    /// To make use of this method, the PhotonStream is essential. It will be in "writing" mode" on the
    /// client that controls a PhotonView (PhotonStream.isWriting == true) and in "reading mode" on the
    /// remote clients that just receive that the controlling client sends.
    ///
    /// If you skip writing any value into the stream, PUN will skip the update. Used carefully, this can
    /// conserve bandwidth and messages (which have a limit per room/second).
    ///
    /// Note that OnPhotonSerializeView is not called on remote clients when the sender does not send
    /// any update. This can't be used as "x-times per second Update()".
    /// </remarks>
    /// \ingroup publicApi
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
}

/// <summary>
/// This interface is used as definition of all callback methods of PUN, except OnPhotonSerializeView. Preferably, implement them individually.
/// </summary>
/// <remarks>
/// This interface is available for completeness, more than for actually implementing it in a game.
/// You can implement each method individually in any MonoMehaviour, without implementing IPunCallbacks.
///
/// PUN calls all callbacks by name. Don't use implement callbacks with fully qualified name.
/// Example: IPunCallbacks.OnConnectedToPhoton won't get called by Unity's SendMessage().
///
/// PUN will call these methods on any script that implements them, analog to Unity's events and callbacks.
/// The situation that triggers the call is described per method.
///
/// OnPhotonSerializeView is NOT called like these callbacks! It's usage frequency is much higher and it is implemented in: IPunObservable.
/// </remarks>
/// \ingroup publicApi
public interface IPunCallbacks
{
    /// <summary>
    /// Called when the initial connection got established but before you can use the server. OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.
    /// </summary>
    /// <remarks>
    /// This callback is only useful to detect if the server can be reached at all (technically).
    /// Most often, it's enough to implement OnFailedToConnectToPhoton() and OnDisconnectedFromPhoton().
    ///
    /// <i>OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.</i>
    ///
    /// When this is called, the low level connection is established and PUN will send your AppId, the user, etc in the background.
    /// This is not called for transitions from the masterserver to game servers.
    /// </remarks>
    void OnConnectedToPhoton();

    /// <summary>
    /// Called when the local user/client left a room.
    /// </summary>
    /// <remarks>
    /// When leaving a room, PUN brings you back to the Master Server.
    /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
    /// </remarks>
    void OnLeftRoom();

    /// <summary>
    /// Called after switching to a new MasterClient when the current one leaves.
    /// </summary>
    /// <remarks>
    /// This is not called when this client enters a room.
    /// The former MasterClient is still in the player list when this method get called.
    /// </remarks>
    void OnMasterClientSwitched(PhotonPlayer newMasterClient);

    /// <summary>
    /// Called when a CreateRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely because the room name is already in use (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is a string debug msg.</param>
    void OnPhotonCreateRoomFailed(object[] codeAndMsg);

    /// <summary>
    /// Called when a JoinRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is string debug msg.</param>
    void OnPhotonJoinRoomFailed(object[] codeAndMsg);

    /// <summary>
    /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
    /// </summary>
    /// <remarks>
    /// This callback is only called on the client which created a room (see PhotonNetwork.CreateRoom).
    ///
    /// As any client might close (or drop connection) anytime, there is a chance that the
    /// creator of a room does not execute OnCreatedRoom.
    ///
    /// If you need specific room properties or a "start signal", it is safer to implement
    /// OnMasterClientSwitched() and to make the new MasterClient check the room's state.
    /// </remarks>
    void OnCreatedRoom();

    /// <summary>
    /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
    /// </summary>
    /// <remarks>
    /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
    ///
    /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
    /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
    /// </remarks>
    void OnJoinedLobby();

    /// <summary>
    /// Called after leaving a lobby.
    /// </summary>
    /// <remarks>
    /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
    /// automatically refer to the default lobby.
    /// </remarks>
    void OnLeftLobby();

    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// This is called when no connection could be established at all.
    /// It differs from OnConnectionFail, which is called when an existing connection fails.
    /// </remarks>
    void OnFailedToConnectToPhoton(DisconnectCause cause);

    /// <summary>
    /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
    /// The reason for the error is provided as DisconnectCause.
    /// </remarks>
    void OnConnectionFail(DisconnectCause cause);

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    /// </remarks>
    void OnDisconnectedFromPhoton();

    /// <summary>
    /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
    /// </summary>
    /// <remarks>
    /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
    /// </remarks>
    void OnPhotonInstantiate(PhotonMessageInfo info);

    /// <summary>
    /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server
    /// or when a response is received for PhotonNetwork.GetCustomRoomList().
    /// </summary>
    /// <remarks>
    /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
    /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
    ///
    /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
    /// </remarks>
    void OnReceivedRoomListUpdate();

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    void OnJoinedRoom();

    /// <summary>
    /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// If your game starts with a certain number of players, this callback can be useful to check the
    /// Room.playerCount and find out if you can start.
    /// </remarks>
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer);

    /// <summary>
    /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
    /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
    /// of several seconds.
    /// </remarks>
    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer);

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.<br/>
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
    void OnPhotonRandomJoinFailed(object[] codeAndMsg);

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    /// <remarks>
    /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
    ///
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
    /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
    /// </remarks>
    void OnConnectedToMaster();

    /// <summary>
    /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
    /// </summary>
    /// <remarks>
    /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
    /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
    /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
    /// </remarks>
    void OnPhotonMaxCccuReached();

    /// <summary>
    /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
    /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
    /// </remarks>
    /// <param name="propertiesThatChanged"></param>
    void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged);

    /// <summary>
    /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: object[] playerAndUpdatedProps, which contains two entries.<br/>
    /// [0] is the affected PhotonPlayer.<br/>
    /// [1] is the Hashtable of properties that changed.<br/>
    ///
    /// We are using a object[] due to limitations of Unity's GameObject.SendMessage (which has only one optional parameter).
    ///
    /// Changing properties must be done by PhotonPlayer.SetCustomProperties, which causes this callback locally, too.
    ///
    /// Example:<pre>
    /// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
    ///     PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
    ///     Hashtable props = playerAndUpdatedProps[1] as Hashtable;
    ///     //...
    /// }</pre>
    /// </remarks>
    /// <param name="playerAndUpdatedProps">Contains PhotonPlayer and the properties that changed See remarks.</param>
    void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps);

    /// <summary>
    /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
    /// </summary>
    /// <remarks>
    /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
    /// the room a user is in (if any).
    /// </remarks>
    void OnUpdatedFriendList();

    /// <summary>
    /// Called when the custom authentication failed. Followed by disconnect!
    /// </summary>
    /// <remarks>
    /// Custom Authentication can fail due to user-input, bad tokens/secrets.
    /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
    ///
    /// During development of a game, it might also fail due to wrong configuration on the server side.
    /// In those cases, logging the debugMessage is very important.
    ///
    /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.photonengine.com/dashboard)),
    /// this won't be called!
    /// </remarks>
    /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
    void OnCustomAuthenticationFailed(string debugMessage);

    /// <summary>
    /// Called when your Custom Authentication service responds with additional data.
    /// </summary>
    /// <remarks>
    /// Custom Authentication services can include some custom data in their response.
    /// When present, that data is made available in this callback as Dictionary.
    /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
    /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
    ///
    /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
    /// </remarks>
    /// <see cref="https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/custom-authentication"/>
    void OnCustomAuthenticationResponse(Dictionary<string, object> data);

    /// <summary>
    /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
    /// </summary>
    /// <remarks>
    /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.<br/>
    /// The content of the response is what your web-service sent. You can create a WebRpcResponse from it.<br/>
    /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);<br/>
    ///
    /// Please note: Class OperationResponse is in a namespace which needs to be "used":<br/>
    /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
    ///
    /// The OperationResponse.ReturnCode by Photon is:<pre>
    ///  0 for "OK"
    /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
    /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)</pre>
    /// </remarks>
    void OnWebRpcResponse(OperationResponse response);

    /// <summary>
    /// Called when another player requests ownership of a PhotonView from you (the current owner).
    /// </summary>
    /// <remarks>
    /// The parameter viewAndPlayer contains:
    ///
    /// PhotonView view = viewAndPlayer[0] as PhotonView;
    ///
    /// PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;
    /// </remarks>
    /// <param name="viewAndPlayer">The PhotonView is viewAndPlayer[0] and the requesting player is viewAndPlayer[1].</param>
    void OnOwnershipRequest(object[] viewAndPlayer);

    /// <summary>
    /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
    /// </summary>
    /// <remarks>
    /// This callback has two preconditions:
    /// EnableLobbyStatistics must be set to true, before this client connects.
    /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
    /// </remarks>
    void OnLobbyStatisticsUpdate();

	/// <summary>
	/// Called when a remote Photon Player activity changed. This will be called ONLY if PlayerTtl is greater than 0.
	/// </summary>
    /// <remarks>
	/// Use PhotonPlayer.IsInactive to check a player's current activity state.
	///
	/// Example: void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) {...}
	///
	/// This callback has precondition:
	/// PlayerTtl must be greater than 0.
	/// </remarks>
	void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer);

	/// <summary>
	/// Called when ownership of a PhotonView is transfered to another player.
	/// </summary>
	/// <remarks>
	/// The parameter viewAndPlayers contains:
	///
	/// PhotonView view = viewAndPlayers[0] as PhotonView;
	///
	/// PhotonPlayer newOwner = viewAndPlayers[1] as PhotonPlayer;
	///
	/// PhotonPlayer oldOwner = viewAndPlayers[2] as PhotonPlayer;
	/// </remarks>
	/// <example>void OnOwnershipTransfered(object[] viewAndPlayers) {} //</example>
	void OnOwnershipTransfered(object[] viewAndPlayers);
}

/// <summary>
/// Defines all the methods that a Object Pool must implement, so that PUN can use it.
/// </summary>
/// <remarks>
/// To use a Object Pool for instantiation, you can set PhotonNetwork.ObjectPool.
/// That is used for all objects, as long as ObjectPool is not null.
/// The pool has to return a valid non-null GameObject when PUN calls Instantiate.
/// Also, the position and rotation must be applied.
///
/// Please note that pooled GameObjects don't get the usual Awake and Start calls.
/// OnEnable will be called (by your pool) but the networking values are not updated yet
/// when that happens. OnEnable will have outdated values for PhotonView (isMine, etc.).
/// You might have to adjust scripts.
///
/// PUN will call OnPhotonInstantiate (see IPunCallbacks). This should be used to
/// setup the re-used object with regards to networking values / ownership.
/// </remarks>
public interface IPunPrefabPool
{
    /// <summary>
    /// This is called when PUN wants to create a new instance of an entity prefab. Must return valid GameObject with PhotonView.
    /// </summary>
    /// <param name="prefabId">The id of this prefab.</param>
    /// <param name="position">The position we want the instance instantiated at.</param>
    /// <param name="rotation">The rotation we want the instance to take.</param>
    /// <returns>The newly instantiated object, or null if a prefab with <paramref name="prefabId"/> was not found.</returns>
    GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation);

    /// <summary>
    /// This is called when PUN wants to destroy the instance of an entity prefab.
    /// </summary>
	/// <remarks>
	/// A pool needs some way to find out which type of GameObject got returned via Destroy().
	/// It could be a tag or name or anything similar.
	/// </remarks>
    /// <param name="gameObject">The instance to destroy.</param>
    void Destroy(GameObject gameObject);
}


namespace Photon
{
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    /// <summary>
    /// This class adds the property photonView, while logging a warning when your game still uses the networkView.
    /// </summary>
    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        /// <summary>Cache field for the PhotonView on this GameObject.</summary>
        private PhotonView pvCache = null;

        /// <summary>A cached reference to a PhotonView on this GameObject.</summary>
        /// <remarks>
        /// If you intend to work with a PhotonView in a script, it's usually easier to write this.photonView.
        ///
        /// If you intend to remove the PhotonView component from the GameObject but keep this Photon.MonoBehaviour,
        /// avoid this reference or modify this code to use PhotonView.Get(obj) instead.
        /// </remarks>
        public PhotonView photonView
        {
            get
            {
                if (pvCache == null)
                {
                    pvCache = PhotonView.Get(this);
                }
                return pvCache;
            }
        }

        #if !UNITY_MIN_5_3
        /// <summary>
        /// This property is only here to notify developers when they use the outdated value.
        /// </summary>
        /// <remarks>
        /// If Unity 5.x logs a compiler warning "Use the new keyword if hiding was intended" or
        /// "The new keyword is not required", you may suffer from an Editor issue.
        /// Try to modify networkView with a if-def condition:
        ///
        /// #if UNITY_EDITOR
        /// new
        /// #endif
        /// public PhotonView networkView
        /// </remarks>
        [Obsolete("Use a photonView")]
        public new PhotonView networkView
        {
            get
            {
                Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
                return PhotonView.Get(this);
            }
        }
        #endif
    }


    /// <summary>
    /// This class provides a .photonView and all callbacks/events that PUN can call. Override the events/methods you want to use.
    /// </summary>
    /// <remarks>
    /// By extending this class, you can implement individual methods as override.
    ///
    /// Visual Studio and MonoDevelop should provide the list of methods when you begin typing "override".
    /// <b>Your implementation does not have to call "base.method()".</b>
    ///
    /// This class implements IPunCallbacks, which is used as definition of all PUN callbacks.
    /// Don't implement IPunCallbacks in your classes. Instead, implent PunBehaviour or individual methods.
    /// </remarks>
    /// \ingroup publicApi
    // the documentation for the interface methods becomes inherited when Doxygen builds it.
    public class PunBehaviour : Photon.MonoBehaviour, IPunCallbacks
    {
        /// <summary>
        /// Called when the initial connection got established but before you can use the server. OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.
        /// </summary>
        /// <remarks>
        /// This callback is only useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnFailedToConnectToPhoton() and OnDisconnectedFromPhoton().
        ///
        /// <i>OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.</i>
        ///
        /// When this is called, the low level connection is established and PUN will send your AppId, the user, etc in the background.
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        public virtual void OnConnectedToPhoton()
        {
        }

        /// <summary>
        /// Called when the local user/client left a room.
        /// </summary>
        /// <remarks>
        /// When leaving a room, PUN brings you back to the Master Server.
        /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
        /// </remarks>
        public virtual void OnLeftRoom()
        {
        }

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        public virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {
        }

        /// <summary>
        /// Called when a CreateRoom() call failed. The parameter provides ErrorCode and message (as array).
        /// </summary>
        /// <remarks>
        /// Most likely because the room name is already in use (some other client was faster than you).
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is a short ErrorCode and codeAndMsg[1] is a string debug msg.</param>
        public virtual void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called when a JoinRoom() call failed. The parameter provides ErrorCode and message (as array).
        /// </summary>
        /// <remarks>
        /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see PhotonNetwork.CreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", it is safer to implement
        /// OnMasterClientSwitched() and to make the new MasterClient check the room's state.
        /// </remarks>
        public virtual void OnCreatedRoom()
        {
        }

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
        /// </summary>
        /// <remarks>
        /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
        ///
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
        /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
        /// </remarks>
        public virtual void OnJoinedLobby()
        {
        }

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        public virtual void OnLeftLobby()
        {
        }

        /// <summary>
        /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// This is called when no connection could be established at all.
        /// It differs from OnConnectionFail, which is called when an existing connection fails.
        /// </remarks>
        public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        /// <remarks>
        /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
        /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
        /// </remarks>
        public virtual void OnDisconnectedFromPhoton()
        {
        }

        /// <summary>
        /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
        /// The reason for the error is provided as DisconnectCause.
        /// </remarks>
        public virtual void OnConnectionFail(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
        /// </summary>
        /// <remarks>
        /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
        /// </remarks>
        public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
        {
        }

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server
        /// or when a response is received for PhotonNetwork.GetCustomRoomList().
        /// </summary>
        /// <remarks>
        /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        ///
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        public virtual void OnReceivedRoomListUpdate()
        {
        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public virtual void OnJoinedRoom()
        {
        }

        /// <summary>
        /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
        }

        /// <summary>
        /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
        /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
        /// of several seconds.
        /// </remarks>
        public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.<br/>
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        public virtual void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
        /// </summary>
        /// <remarks>
        /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
        ///
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
        /// </remarks>
        public virtual void OnConnectedToMaster()
        {
        }

        /// <summary>
        /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
        /// </summary>
        /// <remarks>
        /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
        /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
        /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
        /// </remarks>
        public virtual void OnPhotonMaxCccuReached()
        {
        }

        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="propertiesThatChanged"></param>
        public virtual void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
        }

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: object[] playerAndUpdatedProps, which contains two entries.<br/>
        /// [0] is the affected PhotonPlayer.<br/>
        /// [1] is the Hashtable of properties that changed.<br/>
        ///
        /// We are using a object[] due to limitations of Unity's GameObject.SendMessage (which has only one optional parameter).
        ///
        /// Changing properties must be done by PhotonPlayer.SetCustomProperties, which causes this callback locally, too.
        ///
        /// Example:<pre>
        /// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
        ///     PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        ///     Hashtable props = playerAndUpdatedProps[1] as Hashtable;
        ///     //...
        /// }</pre>
        /// </remarks>
        /// <param name="playerAndUpdatedProps">Contains PhotonPlayer and the properties that changed See remarks.</param>
        public virtual void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {
        }

        /// <summary>
        /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
        /// </summary>
        /// <remarks>
        /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
        /// the room a user is in (if any).
        /// </remarks>
        public virtual void OnUpdatedFriendList()
        {
        }

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.photonengine.com/dashboard)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
        public virtual void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        /// <summary>
        /// Called when your Custom Authentication service responds with additional data.
        /// </summary>
        /// <remarks>
        /// Custom Authentication services can include some custom data in their response.
        /// When present, that data is made available in this callback as Dictionary.
        /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
        /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
        ///
        /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/custom-authentication"/>
        public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        /// <summary>
        /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
        /// </summary>
        /// <remarks>
        /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.
        /// The content of the response is what your web-service sent. You can create a WebResponse instance from it.
        /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);
        ///
        /// Please note: Class OperationResponse is in a namespace which needs to be "used":
        /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
        ///
        /// The OperationResponse.ReturnCode by Photon is:<pre>
        ///  0 for "OK"
        /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
        /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)</pre>
        /// </remarks>
        public virtual void OnWebRpcResponse(OperationResponse response)
        {
        }

        /// <summary>
        /// Called when another player requests ownership of a PhotonView from you (the current owner).
        /// </summary>
        /// <remarks>
        /// The parameter viewAndPlayer contains:
        ///
        /// PhotonView view = viewAndPlayer[0] as PhotonView;
        ///
        /// PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;
        /// </remarks>
        /// <param name="viewAndPlayer">The PhotonView is viewAndPlayer[0] and the requesting player is viewAndPlayer[1].</param>
        public virtual void OnOwnershipRequest(object[] viewAndPlayer)
        {
        }

        /// <summary>
        /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
        /// </summary>
        /// <remarks>
        /// This callback has two preconditions:
        /// EnableLobbyStatistics must be set to true, before this client connects.
        /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
        /// </remarks>
        public virtual void OnLobbyStatisticsUpdate()
        {
        }

        /// <summary>
        /// Called when a remote Photon Player activity changed. This will be called ONLY if PlayerTtl is greater than 0.
        /// </summary>
        /// <remarks>
        /// Use PhotonPlayer.IsInactive to check a player's current activity state.
        ///
        /// Example: void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) {...}
        ///
        /// This callback has precondition:
        /// PlayerTtl must be greater than 0.
        /// </remarks>
		public virtual void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
		{
		}

        /// <summary>
        /// Called when ownership of a PhotonView is transfered to another player.
        /// </summary>
        /// <remarks>
        /// The parameter viewAndPlayers contains:
        ///
        /// PhotonView view = viewAndPlayers[0] as PhotonView;
        ///
        /// PhotonPlayer newOwner = viewAndPlayers[1] as PhotonPlayer;
        ///
        /// PhotonPlayer oldOwner = viewAndPlayers[2] as PhotonPlayer;
        /// </remarks>
        /// <example>void OnOwnershipTransfered(object[] viewAndPlayers) {} //</example>
		public virtual void OnOwnershipTransfered(object[] viewAndPlayers)
		{
		}
    }
}


/// <summary>
/// Container class for info about a particular message, RPC or update.
/// </summary>
/// \ingroup publicApi
public struct PhotonMessageInfo
{
    private readonly int timeInt;
    /// <summary>The sender of a message / event. May be null.</summary>
    public readonly PhotonPlayer sender;
    public readonly PhotonView photonView;

    public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
    {
        this.sender = player;
        this.timeInt = timestamp;
        this.photonView = view;
    }

    public double timestamp
    {
        get
        {
            uint u = (uint)this.timeInt;
            double t = u;
            return t / 1000;
        }
    }

    public override string ToString()
    {
        return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.sender);
    }
}



/// <summary>Defines Photon event-codes as used by PUN.</summary>
internal class PunEvent
{
    public const byte RPC = 200;
    public const byte SendSerialize = 201;
    public const byte Instantiation = 202;
    public const byte CloseConnection = 203;
    public const byte Destroy = 204;
    public const byte RemoveCachedRPCs = 205;
    public const byte SendSerializeReliable = 206;  // TS: added this but it's not really needed anymore
    public const byte DestroyPlayer = 207;  // TS: added to make others remove all GOs of a player
    public const byte AssignMaster = 208;  // TS: added to assign someone master client (overriding the current)
    public const byte OwnershipRequest = 209;
    public const byte OwnershipTransfer = 210;
    public const byte VacantViewIds = 211;
	public const byte levelReload = 212;
}

/// <summary>
/// This container is used in OnPhotonSerializeView() to either provide incoming data of a PhotonView or for you to provide it.
/// </summary>
/// <remarks>
/// The isWriting property will be true if this client is the "owner" of the PhotonView (and thus the GameObject).
/// Add data to the stream and it's sent via the server to the other players in a room.
/// On the receiving side, isWriting is false and the data should be read.
///
/// Send as few data as possible to keep connection quality up. An empty PhotonStream will not be sent.
///
/// Use either Serialize() for reading and writing or SendNext() and ReceiveNext(). The latter two are just explicit read and
/// write methods but do about the same work as Serialize(). It's a matter of preference which methods you use.
/// </remarks>
/// <seealso cref="PhotonNetworkingMessage"/>
/// \ingroup publicApi
public class PhotonStream
{
    bool write = false;
    private Queue<object> writeData;
    private object[] readData;
    internal byte currentItem = 0; //Used to track the next item to receive.

    /// <summary>
    /// Creates a stream and initializes it. Used by PUN internally.
    /// </summary>
    public PhotonStream(bool write, object[] incomingData)
    {
        this.write = write;
        if (incomingData == null)
        {
            this.writeData = new Queue<object>(10);
        }
        else
        {
            this.readData = incomingData;
        }
    }

    public void SetReadStream(object[] incomingData, byte pos = 0)
    {
        this.readData = incomingData;
        this.currentItem = pos;
        this.write = false;
    }

    internal void ResetWriteStream()
    {
        writeData.Clear();
    }

    /// <summary>If true, this client should add data to the stream to send it.</summary>
    public bool isWriting
    {
        get { return this.write; }
    }

    /// <summary>If true, this client should read data send by another client.</summary>
    public bool isReading
    {
        get { return !this.write; }
    }

    /// <summary>Count of items in the stream.</summary>
    public int Count
    {
        get
        {
            return (this.isWriting) ? this.writeData.Count : this.readData.Length;
        }
    }

    /// <summary>Read next piece of data from the stream when isReading is true.</summary>
    public object ReceiveNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }

        object obj = this.readData[this.currentItem];
        this.currentItem++;
        return obj;
    }

    /// <summary>Read next piece of data from the stream without advancing the "current" item.</summary>
    public object PeekNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }

        object obj = this.readData[this.currentItem];
        //this.currentItem++;
        return obj;
    }

    /// <summary>Add another piece of data to send it when isWriting is true.</summary>
    public void SendNext(object obj)
    {
        if (!this.write)
        {
            Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
            return;
        }

        this.writeData.Enqueue(obj);
    }

    /// <summary>Turns the stream into a new object[].</summary>
    public object[] ToArray()
    {
        return this.isWriting ? this.writeData.ToArray() : this.readData;
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref bool myBool)
    {
        if (this.write)
        {
            this.writeData.Enqueue(myBool);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                myBool = (bool)this.readData[currentItem];
                this.currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref int myInt)
    {
        if (write)
        {
            this.writeData.Enqueue(myInt);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                myInt = (int)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref string value)
    {
        if (write)
        {
            this.writeData.Enqueue(value);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                value = (string)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref char value)
    {
        if (write)
        {
            this.writeData.Enqueue(value);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                value = (char)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref short value)
    {
        if (write)
        {
            this.writeData.Enqueue(value);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                value = (short)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref float obj)
    {
        if (write)
        {
            this.writeData.Enqueue(obj);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                obj = (float)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref PhotonPlayer obj)
    {
        if (write)
        {
            this.writeData.Enqueue(obj);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                obj = (PhotonPlayer)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Vector3 obj)
    {
        if (write)
        {
            this.writeData.Enqueue(obj);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                obj = (Vector3)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Vector2 obj)
    {
        if (write)
        {
            this.writeData.Enqueue(obj);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                obj = (Vector2)this.readData[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Quaternion obj)
    {
        if (write)
        {
            this.writeData.Enqueue(obj);
        }
        else
        {
            if (this.readData.Length > currentItem)
            {
                obj = (Quaternion)this.readData[currentItem];
                currentItem++;
            }
        }
    }
}


#if UNITY_5_0 || !UNITY_5 && !UNITY_5_3_OR_NEWER
/// <summary>Empty implementation of the upcoming HelpURL of Unity 5.1. This one is only for compatibility of attributes.</summary>
/// <remarks>http://feedback.unity3d.com/suggestions/override-component-documentation-slash-help-link</remarks>
public class HelpURL : Attribute
{
    public HelpURL(string url)
    {
    }
}
#endif


#if !UNITY_MIN_5_3
// in Unity 5.3 and up, we have to use a SceneManager. This section re-implements it for older Unity versions

#if UNITY_EDITOR
namespace UnityEditor.SceneManagement
{
    /// <summary>Minimal implementation of the EditorSceneManager for older Unity, up to v5.2.</summary>
    public class EditorSceneManager
    {
        public static int loadedSceneCount
        {
            get { return string.IsNullOrEmpty(UnityEditor.EditorApplication.currentScene) ? -1 : 1; }
        }

        public static void OpenScene(string name)
        {
            UnityEditor.EditorApplication.OpenScene(name);
        }

        public static void SaveOpenScenes()
        {
            UnityEditor.EditorApplication.SaveScene();
        }

        public static void SaveCurrentModifiedScenesIfUserWantsTo()
        {
            UnityEditor.EditorApplication.SaveCurrentSceneIfUserWantsTo();
        }
    }
}
#endif

namespace UnityEngine.SceneManagement
{
	public enum LoadSceneMode
	{
		Single,
		Additive
	}

    /// <summary>Minimal implementation of the SceneManager for older Unity, up to v5.2.</summary>
    public class SceneManager
    {
        public static void LoadScene(string name)
        {
            Application.LoadLevel(name);
        }

        public static void LoadScene(int buildIndex)
        {
            Application.LoadLevel(buildIndex);
        }

		public static AsyncOperation LoadSceneAsync(string name,LoadSceneMode mode =  LoadSceneMode.Single)
		{
			if (mode == UnityEngine.SceneManagement.LoadSceneMode.Single) {
				return Application.LoadLevelAsync (name);
			} else {
				return Application.LoadLevelAdditiveAsync(name);
			}
		}

		public static AsyncOperation LoadSceneAsync(int buildIndex,LoadSceneMode mode =  LoadSceneMode.Single)
		{
			if (mode == UnityEngine.SceneManagement.LoadSceneMode.Single) {
				return Application.LoadLevelAsync (buildIndex);
			} else {
				return Application.LoadLevelAdditiveAsync(buildIndex);
			}
		}

    }
}

#endif


public class SceneManagerHelper
{
    public static string ActiveSceneName
    {
        get
        {
            #if UNITY_MIN_5_3
            UnityEngine.SceneManagement.Scene s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return s.name;
            #else
            return Application.loadedLevelName;
            #endif
        }
    }

    public static int ActiveSceneBuildIndex
    {
        get
        {
            #if UNITY_MIN_5_3
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            #else
            return Application.loadedLevel;
            #endif
        }
    }


#if UNITY_EDITOR
    public static string EditorActiveSceneName
    {
        get
        {
            #if UNITY_MIN_5_3
            return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            #else
            return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.EditorApplication.currentScene);
            #endif
        }
    }
#endif
}


/// <summary>Reads an operation response of a WebRpc and provides convenient access to most common values.</summary>
/// <remarks>
/// See method PhotonNetwork.WebRpc.<br/>
/// Create a WebRpcResponse to access common result values.<br/>
/// The operationResponse.OperationCode should be: OperationCode.WebRpc.<br/>
/// </remarks>
public class WebRpcResponse
{
    /// <summary>Name of the WebRpc that was called.</summary>
    public string Name { get; private set; }
    /// <summary>ReturnCode of the WebService that answered the WebRpc.</summary>
    /// <remarks>
    /// 0 is commonly used to signal success.<br/>
    /// -1 tells you: Got no ReturnCode from WebRpc service.<br/>
    /// Other ReturnCodes are defined by the individual WebRpc and service.
    /// </remarks>
    public int ReturnCode { get; private set; }
    /// <summary>Might be empty or null.</summary>
    public string DebugMessage { get; private set; }
    /// <summary>Other key/values returned by the webservice that answered the WebRpc.</summary>
    public Dictionary<string, object> Parameters { get; private set; }

    /// <summary>An OperationResponse for a WebRpc is needed to read it's values.</summary>
    public WebRpcResponse(OperationResponse response)
    {
        object value;
        response.Parameters.TryGetValue(ParameterCode.UriPath, out value);
        this.Name = value as string;

        response.Parameters.TryGetValue(ParameterCode.WebRpcReturnCode, out value);
        this.ReturnCode = (value != null) ? (byte)value : -1;

        response.Parameters.TryGetValue(ParameterCode.WebRpcParameters, out value);
        this.Parameters = value as Dictionary<string, object>;

        response.Parameters.TryGetValue(ParameterCode.WebRpcReturnMessage, out value);
        this.DebugMessage = value as string;
    }

    /// <summary>Turns the response into an easier to read string.</summary>
    /// <returns>String resembling the result.</returns>
    public string ToStringFull()
    {
        return string.Format("{0}={2}: {1} \"{3}\"", Name, SupportClassPun.DictionaryToString(Parameters), ReturnCode, DebugMessage);
    }
}

/**
public class PBitStream
{
    List<byte> streamBytes;
    private int currentByte;
    private int totalBits = 0;

    public int ByteCount
    {
        get { return BytesForBits(this.totalBits); }
    }

    public int BitCount
    {
        get { return this.totalBits; }
        private set { this.totalBits = value; }
    }

    public PBitStream()
    {
        this.streamBytes = new List<byte>(1);
    }

    public PBitStream(int bitCount)
    {
        this.streamBytes = new List<byte>(BytesForBits(bitCount));
    }

    public PBitStream(IEnumerable<byte> bytes, int bitCount)
    {
        this.streamBytes = new List<byte>(bytes);
        this.BitCount = bitCount;
    }

    public static int BytesForBits(int bitCount)
    {
        if (bitCount <= 0)
        {
            return 0;
        }

        return ((bitCount - 1) / 8) + 1;
    }

    public void Add(bool val)
    {
        int bytePos = this.totalBits / 8;
        if (bytePos > this.streamBytes.Count-1 || this.totalBits == 0)
        {
            this.streamBytes.Add(0);
        }

        if (val)
        {
            int currentByteBit = 7 - (this.totalBits % 8);
            this.streamBytes[bytePos] |= (byte)(1 << currentByteBit);
        }

        this.totalBits++;
    }

    public byte[] ToBytes()
    {
        return this.streamBytes.ToArray();
    }

    public int Position { get; set; }

    public bool GetNext()
    {
        if (this.Position > this.totalBits)
        {
            throw new Exception("End of PBitStream reached. Can't read more.");
        }

        return Get(this.Position++);
    }

    public bool Get(int bitIndex)
    {
        int byteIndex = bitIndex / 8;
        int bitInByIndex = 7 - (bitIndex % 8);
        return ((this.streamBytes[byteIndex] & (byte)(1 << bitInByIndex)) > 0);
    }

    public void Set(int bitIndex, bool value)
    {
        int byteIndex = bitIndex / 8;
        int bitInByIndex = 7 - (bitIndex % 8);
        this.streamBytes[byteIndex] |= (byte)(1 << bitInByIndex);
    }
}
**/
