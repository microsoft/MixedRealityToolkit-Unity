// ----------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Wraps up several of the commonly used enumerations. </summary>
#pragma warning restore 1587


using System;
using ExitGames.Client.Photon;


/// <summary>
/// This enum defines the set of MonoMessages Photon Unity Networking is using as callbacks. Implemented by PunBehaviour.
/// </summary>
/// <remarks>
/// Much like "Update()" in Unity, PUN will call methods in specific situations.
/// Often, these methods are triggered when network operations complete (example: when joining a room).
///
/// All those methods are defined and described in this enum and implemented by PunBehaviour
/// (which makes it easy to implement them as override).
///
/// Each entry is the name of such a method and the description tells you when it gets used by PUN.
///
/// Make sure to read the remarks per entry as some methods have optional parameters.
/// </remarks>
/// \ingroup publicApi
public enum PhotonNetworkingMessage
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
    ///
    /// Example: void OnConnectedToPhoton() { ... }
    /// </remarks>
    OnConnectedToPhoton,

    /// <summary>
    /// Called when the local user/client left a room.
    /// </summary>
    /// <remarks>
    /// When leaving a room, PUN brings you back to the Master Server.
    /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
    ///
    /// Example: void OnLeftRoom() { ... }
    /// </remarks>
    OnLeftRoom,

    /// <summary>
    /// Called after switching to a new MasterClient when the current one leaves.
    /// </summary>
    /// <remarks>
    /// This is not called when this client enters a room.
    /// The former MasterClient is still in the player list when this method get called.
    ///
    /// Example: void OnMasterClientSwitched(PhotonPlayer newMasterClient) { ... }
    /// </remarks>
    OnMasterClientSwitched,

    /// <summary>
    /// Called when a CreateRoom() call failed. Optional parameters provide ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely because the room name is already in use (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    ///
    /// Example: void OnPhotonCreateRoomFailed() { ... }
    ///
    /// Example: void OnPhotonCreateRoomFailed(object[] codeAndMsg) { // codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.  }
    /// </remarks>
    OnPhotonCreateRoomFailed,

    /// <summary>
    /// Called when a JoinRoom() call failed. Optional parameters provide ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    ///
    /// Example: void OnPhotonJoinRoomFailed() { ... }
    ///
    /// Example: void OnPhotonJoinRoomFailed(object[] codeAndMsg) { // codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.  }
    /// </remarks>
    OnPhotonJoinRoomFailed,

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
    ///
    /// Example: void OnCreatedRoom() { ... }
    /// </remarks>
    OnCreatedRoom,

    /// <summary>
    /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
    /// </summary>
    /// <remarks>
    /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
    ///
    /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
    /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
    ///
    /// Example: void OnJoinedLobby() { ... }
    /// </remarks>
    OnJoinedLobby,

    /// <summary>
    /// Called after leaving a lobby.
    /// </summary>
    /// <remarks>
    /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
    /// automatically refer to the default lobby.
    ///
    /// Example: void OnLeftLobby() { ... }
    /// </remarks>
    OnLeftLobby,

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    ///
    /// Example: void OnDisconnectedFromPhoton() { ... }
    /// </remarks>
    OnDisconnectedFromPhoton,

    /// <summary>
    /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
    /// The reason for the error is provided as StatusCode.
    ///
    /// Example: void OnConnectionFail(DisconnectCause cause) { ... }
    /// </remarks>
    OnConnectionFail,

    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// OnConnectionFail only gets called when a connection to a Photon server was established in the first place.
    ///
    /// Example: void OnFailedToConnectToPhoton(DisconnectCause cause) { ... }
    /// </remarks>
    OnFailedToConnectToPhoton,

    /// <summary>
    /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server
    /// or when a response is received for PhotonNetwork.GetCustomRoomList().
    /// </summary>
    /// <remarks>
    /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
    /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
    ///
    /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
    ///
    /// Example: void OnReceivedRoomListUpdate() { ... }
    /// </remarks>
    OnReceivedRoomListUpdate,

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can instead call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
    /// enough players are in the room to start playing.
    ///
    /// Example: void OnJoinedRoom() { ... }
    /// </remarks>
    OnJoinedRoom,

    /// <summary>
    /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// If your game starts with a certain number of players, this callback can be useful to check the
    /// Room.playerCount and find out if you can start.
    ///
    /// Example: void OnPhotonPlayerConnected(PhotonPlayer newPlayer) { ... }
    /// </remarks>
    OnPhotonPlayerConnected,

    /// <summary>
    /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
    /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
    /// of several seconds.
    ///
    /// Example: void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) { ... }
    /// </remarks>
    OnPhotonPlayerDisconnected,

    /// <summary>
    /// Called after a JoinRandom() call failed. Optional parameters provide ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available.
    /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    ///
    /// Example: void OnPhotonRandomJoinFailed() { ... }
    ///
    /// Example: void OnPhotonRandomJoinFailed(object[] codeAndMsg) { // codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.  }
    /// </remarks>
    OnPhotonRandomJoinFailed,

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    /// <remarks>
    /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
    ///
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
    /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
    ///
    /// Example: void OnConnectedToMaster() { ... }
    /// </remarks>
    OnConnectedToMaster,

    /// <summary>
    /// Implement to customize the data a PhotonView regularly synchronizes. Called every 'network-update' when observed by PhotonView.
    /// </summary>
    /// <remarks>
    /// This method will be called in scripts that are assigned as Observed component of a PhotonView.
    /// PhotonNetwork.sendRateOnSerialize affects how often this method is called.
    /// PhotonNetwork.sendRate affects how often packages are sent by this client.
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
    ///
    /// Example: void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { ... }
    /// </remarks>
    OnPhotonSerializeView,

    /// <summary>
    /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
    /// </summary>
    /// <remarks>
    /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
    ///
    /// Example: void OnPhotonInstantiate(PhotonMessageInfo info) { ... }
    /// </remarks>
    OnPhotonInstantiate,

    /// <summary>
    /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
    /// </summary>
    /// <remarks>
    /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
    /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
    /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
    ///
    /// Example: void OnPhotonMaxCccuReached() { ... }
    /// </remarks>
    OnPhotonMaxCccuReached,

    /// <summary>
    /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.
    /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
    ///
    /// Example: void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged) { ... }
    /// </remarks>
    OnPhotonCustomRoomPropertiesChanged,

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
    OnPhotonPlayerPropertiesChanged,

    /// <summary>
    /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
    /// </summary>
    /// <remarks>
    /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
    /// the room a user is in (if any).
    ///
    /// Example: void OnUpdatedFriendList() { ... }
    /// </remarks>
    OnUpdatedFriendList,

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
    ///
    /// Example: void OnCustomAuthenticationFailed(string debugMessage) { ... }
    /// </remarks>
    OnCustomAuthenticationFailed,

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
    OnCustomAuthenticationResponse,

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
    /// The OperationResponse.ReturnCode by Photon is:
    ///  0 for "OK"
    /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
    /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)
    ///
    /// Example: void OnWebRpcResponse(OperationResponse response) { ... }
    /// </remarks>
    OnWebRpcResponse,

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
    /// <example>void OnOwnershipRequest(object[] viewAndPlayer) {} //</example>
    OnOwnershipRequest,

    /// <summary>
    /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
    /// </summary>
    /// <remarks>
    /// This callback has two preconditions:
    /// EnableLobbyStatistics must be set to true, before this client connects.
    /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
    /// </remarks>
    OnLobbyStatisticsUpdate,


	/// <summary>
	/// Called when a remote Photon Player activity changed. This will be called ONLY is PlayerTtl is greater then 0.
	///
	/// Use PhotonPlayer.IsInactive to check the current activity state
	///
	/// Example: void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) {...}
	/// </summary>
	/// <remarks>
	/// This callback has precondition:
	/// PlayerTtl must be greater then 0
	/// </remarks>
	OnPhotonPlayerActivityChanged,


	/// <summary>
	/// Called when a PhotonView Owner is transfered to a Player.
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
	OnOwnershipTransfered,
}


/// <summary>Used to define the level of logging output created by the PUN classes. Either log errors, info (some more) or full.</summary>
/// \ingroup publicApi
public enum PhotonLogLevel
{
    /// <summary>Show only errors. Minimal output. Note: Some might be "runtime errors" which you have to expect.</summary>
    ErrorsOnly,
    /// <summary>Logs some of the workflow, calls and results.</summary>
    Informational,
    /// <summary>Every available log call gets into the console/log. Only use for debugging.</summary>
    Full
}


/// <summary>Enum of "target" options for RPCs. These define which remote clients get your RPC call. </summary>
/// \ingroup publicApi
public enum PhotonTargets
{
    /// <summary>Sends the RPC to everyone else and executes it immediately on this client. Player who join later will not execute this RPC.</summary>
    All,
    /// <summary>Sends the RPC to everyone else. This client does not execute the RPC. Player who join later will not execute this RPC.</summary>
    Others,
    /// <summary>Sends the RPC to MasterClient only. Careful: The MasterClient might disconnect before it executes the RPC and that might cause dropped RPCs.</summary>
    MasterClient,
    /// <summary>Sends the RPC to everyone else and executes it immediately on this client. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    AllBuffered,
    /// <summary>Sends the RPC to everyone. This client does not execute the RPC. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    OthersBuffered,
    /// <summary>Sends the RPC to everyone (including this client) through the server.</summary>
    /// <remarks>
    /// This client executes the RPC like any other when it received it from the server.
    /// Benefit: The server's order of sending the RPCs is the same on all clients.
    /// </remarks>
    AllViaServer,
    /// <summary>Sends the RPC to everyone (including this client) through the server and buffers it for players joining later.</summary>
    /// <remarks>
    /// This client executes the RPC like any other when it received it from the server.
    /// Benefit: The server's order of sending the RPCs is the same on all clients.
    /// </remarks>
    AllBufferedViaServer
}


/// <summary>Currently available <a href="https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/regions">Photon Cloud regions</a> as enum.</summary>
/// <remarks>
/// This is used in PhotonNetwork.ConnectToRegion.
/// </remarks>
public enum CloudRegionCode
{
    /// <summary>European servers in Amsterdam.</summary>
    eu = 0,
    /// <summary>US servers (East Coast).</summary>
    us = 1,
    /// <summary>Asian servers in Singapore.</summary>
    asia = 2,
    /// <summary>Japanese servers in Tokyo.</summary>
    jp = 3,
    /// <summary>Australian servers in Melbourne.</summary>
    au = 5,
    ///<summary>USA West, San José, usw</summary>
    usw = 6,
    ///<summary>South America, Sao Paulo, sa</summary>
    sa = 7,
    ///<summary>Canada East, Montreal, cae</summary>
    cae = 8,
    ///<summary>South Korea, Seoul, kr</summary>
    kr = 9,
    ///<summary>India, Chennai, in</summary>
    @in = 10,
    /// <summary>Russia, ru</summary>
    ru = 11,
    /// <summary>Russia East, rue</summary>
    rue = 12,

    /// <summary>No region selected.</summary>
    none = 4
};


/// <summary>
/// Available regions as enum of flags. To be used as "enabled" flags for Best Region pinging.
/// </summary>
/// <remarks>Note that these enum values skip CloudRegionCode.none and their values are in strict order (power of 2).</remarks>
[Flags]
public enum CloudRegionFlag
{
    eu =    1 << 0,
    us =    1 << 1,
    asia =  1 << 2,
    jp =    1 << 3,
    au =    1 << 4,
    usw =   1 << 5,
    sa =    1 << 6,
    cae =   1 << 7,
    kr =    1 << 8,
    @in =   1 << 9,
    ru =    1 << 10,
    rue =   1 << 11
};


/// <summary>
/// High level connection state of the client. Better use the more detailed <see cref="ClientState"/>.
/// </summary>
public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Disconnecting,
    InitializingApplication
}


/// <summary>
/// Defines how the communication gets encrypted.
/// </summary>
public enum EncryptionMode
{
    /// <summary>
    /// This is the default encryption mode: Messages get encrypted only on demand (when you send operations with the "encrypt" parameter set to true).
    /// </summary>
    PayloadEncryption,
    /// <summary>
    /// With this encryption mode for UDP, the connection gets setup and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is skipped.
    /// </summary>
    /// <remarks>
    /// This mode requires AuthOnce or AuthOnceWss as AuthMode!
    /// </remarks>
    DatagramEncryption = 10,
}


public static class EncryptionDataParameters
{
    /// <summary>
    /// Key for encryption mode
    /// </summary>
    public const byte Mode = 0;
    /// <summary>
    /// Key for first secret
    /// </summary>
    public const byte Secret1 = 1;
    /// <summary>
    /// Key for second secret
    /// </summary>
    public const byte Secret2 = 2;
}