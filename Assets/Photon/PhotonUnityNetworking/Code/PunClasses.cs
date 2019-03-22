// ----------------------------------------------------------------------------
// <copyright file="PunClasses.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Wraps up smaller classes that don't need their own file.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


#pragma warning disable 1587
/// \defgroup publicApi Public API
/// \brief Groups the most important classes that you need to understand early on.
///
/// \defgroup optionalGui Optional Gui Elements
/// \brief Useful GUI elements for PUN.
///
/// \defgroup callbacks Callbacks
/// \brief Callback Interfaces
#pragma warning restore 1587


namespace Photon.Pun
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using ExitGames.Client.Photon;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Photon.Realtime;
    using SupportClassPun = ExitGames.Client.Photon.SupportClass;



    /// <summary>Replacement for RPC attribute with different name. Used to flag methods as remote-callable.</summary>
    public class PunRPC : Attribute
    {
    }

    /// <summary>Defines the OnPhotonSerializeView method to make it easy to implement correctly for observable scripts.</summary>
    /// \ingroup callbacks
    public interface IPunObservable
    {
        /// <summary>
        /// Called by PUN several times per second, so that your script can write and read synchronization data for the PhotonView.
        /// </summary>
        /// <remarks>
        /// This method will be called in scripts that are assigned as Observed component of a PhotonView.<br/>
        /// PhotonNetwork.SerializationRate affects how often this method is called.<br/>
        /// PhotonNetwork.SendRate affects how often packages are sent by this client.<br/>
        ///
        /// Implementing this method, you can customize which data a PhotonView regularly synchronizes.
        /// Your code defines what is being sent (content) and how your data is used by receiving clients.
        ///
        /// Unlike other callbacks, <i>OnPhotonSerializeView only gets called when it is assigned
        /// to a PhotonView</i> as PhotonView.observed script.
        ///
        /// To make use of this method, the PhotonStream is essential. It will be in "writing" mode" on the
        /// client that controls a PhotonView (PhotonStream.IsWriting == true) and in "reading mode" on the
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
    /// Example: IPunCallbacks.OnConnected won't get called by Unity's SendMessage().
    ///
    /// PUN will call these methods on any script that implements them, analog to Unity's events and callbacks.
    /// The situation that triggers the call is described per method.
    ///
    /// OnPhotonSerializeView is NOT called like these callbacks! It's usage frequency is much higher and it is implemented in: IPunObservable.
    /// </remarks>
    /// \ingroup callbacks
    public interface IPunOwnershipCallbacks
    {
        /// <summary>
        /// Called when another player requests ownership of a PhotonView from you (the current owner).
        /// </summary>
        /// <remarks>
        /// The parameter viewAndPlayer contains:
        ///
        /// PhotonView view = viewAndPlayer[0] as PhotonView;
        ///
        /// Player requestingPlayer = viewAndPlayer[1] as Player;
        /// </remarks>
        /// <param name="targetView">PhotonView for which ownership gets requested.</param>
        /// <param name="requestingPlayer">Player who requests ownership.</param>
        void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer);

        /// <summary>
        /// Called when ownership of a PhotonView is transfered to another player.
        /// </summary>
        /// <remarks>
        /// The parameter viewAndPlayers contains:
        ///
        /// PhotonView view = viewAndPlayers[0] as PhotonView;
        ///
        /// Player newOwner = viewAndPlayers[1] as Player;
        ///
        /// Player oldOwner = viewAndPlayers[2] as Player;
        /// </remarks>
        /// <example>void OnOwnershipTransfered(object[] viewAndPlayers) {} //</example>
        /// <param name="targetView">PhotonView for which ownership changed.</param>
        /// <param name="previousOwner">Player who was the previous owner (or null, if none).</param>
        void OnOwnershipTransfered(PhotonView targetView, Player previousOwner);
    }

    /// \ingroup callbacks
    public interface IPunInstantiateMagicCallback
    {
        void OnPhotonInstantiate(PhotonMessageInfo info);
    }

    /// <summary>
    /// Defines an interface for object pooling, used in PhotonNetwork.Instantiate and PhotonNetwork.Destroy.
    /// </summary>
    /// <remarks>
    /// To apply your custom IPunPrefabPool, set PhotonNetwork.PrefabPool.
    ///
    /// The pool has to return a valid, disabled GameObject when PUN calls Instantiate.
    /// Also, the position and rotation must be applied.
    ///
    /// Note that Awake and Start are only called once by Unity, so scripts on re-used GameObjects
    /// should make use of OnEnable and or OnDisable. When OnEnable gets called, the PhotonView
    /// is already updated to the new values.
    ///
    /// To be able to enable a GameObject, Instantiate must return an inactive object.
    ///
    /// Before PUN "destroys" GameObjects, it will disable them. 
    ///
    /// If a component implements IPunInstantiateMagicCallback, PUN will call OnPhotonInstantiate
    /// when the networked object gets instantiated. If no components implement this on a prefab,
    /// PUN will optimize the instantiation and no longer looks up IPunInstantiateMagicCallback
    /// via GetComponents.
    /// </remarks>
    public interface IPunPrefabPool
    {
        /// <summary>
        /// Called to get an instance of a prefab. Must return valid, disabled GameObject with PhotonView.
        /// </summary>
        /// <param name="prefabId">The id of this prefab.</param>
        /// <param name="position">The position for the instance.</param>
        /// <param name="rotation">The rotation for the instance.</param>
        /// <returns>A disabled instance to use by PUN or null if the prefabId is unknown.</returns>
        GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Called to destroy (or just return) the instance of a prefab. It's disabled and the pool may reset and cache it for later use in Instantiate.
        /// </summary>
        /// <remarks>
        /// A pool needs some way to find out which type of GameObject got returned via Destroy().
        /// It could be a tag, name, a component or anything similar.
        /// </remarks>
        /// <param name="gameObject">The instance to destroy.</param>
        void Destroy(GameObject gameObject);
    }


    /// <summary>
    /// This class adds the property photonView, while logging a warning when your game still uses the networkView.
    /// </summary>
    public class MonoBehaviourPun : MonoBehaviour
    {
        /// <summary>Cache field for the PhotonView on this GameObject.</summary>
        private PhotonView pvCache;

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
                if (this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
                return this.pvCache;
            }
        }
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
    /// This class implements all callback interfaces and extends <see cref="Photon.Pun.MonoBehaviourPun"/>.
    /// </remarks>
    /// \ingroup callbacks
    // the documentation for the interface methods becomes inherited when Doxygen builds it.
    public class MonoBehaviourPunCallbacks : MonoBehaviourPun, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks
    {
        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// Called to signal that the raw connection got established but before the client can call operation on the server.
        /// </summary>
        /// <remarks>
        /// After the (low level transport) connection is established, the client will automatically send
        /// the Authentication operation, which needs to get a response before the client can call other operations.
        ///
        /// Your logic should wait for either: OnRegionListReceived or OnConnectedToMaster.
        ///
        /// This callback is useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnDisconnected().
        ///
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        public virtual void OnConnected()
        {
        }

        /// <summary>
        /// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
        /// </summary>
        /// <remarks>
        /// When leaving a room, the LoadBalancingClient will disconnect the Game Server and connect to the Master Server.
        /// This wraps up multiple internal actions.
        ///
        /// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.
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
        public virtual void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        /// <summary>
        /// Called when the server couldn't create a room (OpCreateRoom failed).
        /// </summary>
        /// <remarks>
        /// The most common cause to fail creating a room, is when a title relies on fixed room-names and the room already exists.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when a previous OpJoinRoom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see OpCreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
        /// and make each new MasterClient check the room's state.
        /// </remarks>
        public virtual void OnCreatedRoom()
        {
        }

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
        /// </summary>
        /// <remarks>
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
        /// The room list gets available via OnRoomListUpdate.
        /// </remarks>
        public virtual void OnJoinedLobby()
        {
        }

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [OpCreateRoom](@ref OpCreateRoom) and [OpJoinRandomRoom](@ref OpJoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        public virtual void OnLeftLobby()
        {
        }

        /// <summary>
        /// Called after disconnecting from the Photon server. It could be a failure or intentional
        /// </summary>
        /// <remarks>
        /// The reason for this disconnect is provided as DisconnectCause.
        /// </remarks>
        public virtual void OnDisconnected(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called when the Name Server provided a list of regions for your title.
        /// </summary>
        /// <remarks>Check the RegionHandler class description, to make use of the provided values.</remarks>
        /// <param name="regionHandler">The currently used RegionHandler.</param>
        public virtual void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        public virtual void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        /// <summary>
        /// Called when the LoadBalancingClient entered a room, no matter if this client created it or simply joined.
        /// </summary>
        /// <remarks>
        /// When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
        ///
        /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
        ///
        /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).
        /// </remarks>
        public virtual void OnJoinedRoom()
        {
        }

        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public virtual void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        public virtual void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        /// <summary>
        /// Called when a previous OpJoinRandom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        ///
        /// When using multiple lobbies (via OpJoinLobby or a TypedLobby parameter), another lobby might have more/fitting rooms.<br/>
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
        /// </summary>
        /// <remarks>
        /// The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// </remarks>
        public virtual void OnConnectedToMaster()
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
        public virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Changing properties must be done by Player.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        ///
        /// <param name="targetPlayer">Contains Player that changed.</param>
        /// <param name="changedProps">Contains the properties that changed.</param>
        public virtual void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
        {
        }

        /// <summary>
        /// Called when the server sent the response to a FindFriends request.
        /// </summary>
        /// <remarks>
        /// After calling OpFindFriends, the Master Server will cache the friend list and send updates to the friend
        /// list. The friends includes the name, userId, online state and the room (if any) for each requested user/friend.
        ///
        /// Use the friendList to update your UI and store it, if the UI should highlight changes.
        /// </remarks>
        public virtual void OnFriendListUpdate(List<FriendInfo> friendList)
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
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/custom-authentication"/>
        public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data)
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
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://dashboard.photonengine.com)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development.</param>
        public virtual void OnCustomAuthenticationFailed (string debugMessage)
        {
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public virtual void OnWebRpcResponse(OperationResponse response)
        {
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public virtual void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
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
        public readonly Player Sender;
        public readonly PhotonView photonView;

        public PhotonMessageInfo(Player player, int timestamp, PhotonView view)
        {
            this.Sender = player;
            this.timeInt = timestamp;
            this.photonView = view;
        }

        public double timestamp
        {
            get
            {
                uint u = (uint) this.timeInt;
                double t = u;
                return t / 1000;
            }
        }

        public override string ToString()
        {
            return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.Sender);
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
        public const byte SendSerializeReliable = 206; // TS: added this but it's not really needed anymore
        public const byte DestroyPlayer = 207; // TS: added to make others remove all GOs of a player
        public const byte OwnershipRequest = 209;
        public const byte OwnershipTransfer = 210;
        public const byte VacantViewIds = 211;
    }


    /// <summary>
    /// This container is used in OnPhotonSerializeView() to either provide incoming data of a PhotonView or for you to provide it.
    /// </summary>
    /// <remarks>
    /// The IsWriting property will be true if this client is the "owner" of the PhotonView (and thus the GameObject).
    /// Add data to the stream and it's sent via the server to the other players in a room.
    /// On the receiving side, IsWriting is false and the data should be read.
    ///
    /// Send as few data as possible to keep connection quality up. An empty PhotonStream will not be sent.
    ///
    /// Use either Serialize() for reading and writing or SendNext() and ReceiveNext(). The latter two are just explicit read and
    /// write methods but do about the same work as Serialize(). It's a matter of preference which methods you use.
    /// </remarks>
    /// \ingroup publicApi
    public class PhotonStream
    {
        private readonly Queue<object> writeData;
        private object[] readData;
        private byte currentItem; //Used to track the next item to receive.

        /// <summary>If true, this client should add data to the stream to send it.</summary>
        public bool IsWriting { get; private set; }

        /// <summary>If true, this client should read data send by another client.</summary>
        public bool IsReading
        {
            get { return !this.IsWriting; }
        }

        /// <summary>Count of items in the stream.</summary>
        public int Count
        {
            get { return this.IsWriting ? this.writeData.Count : this.readData.Length; }
        }

        /// <summary>
        /// Creates a stream and initializes it. Used by PUN internally.
        /// </summary>
        public PhotonStream(bool write, object[] incomingData)
        {
            this.IsWriting = write;
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
            this.IsWriting = false;
        }

        internal void ResetWriteStream()
        {
            this.writeData.Clear();
        }

        /// <summary>Read next piece of data from the stream when IsReading is true.</summary>
        public object ReceiveNext()
        {
            if (this.IsWriting)
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
            if (this.IsWriting)
            {
                Debug.LogError("Error: you cannot read this stream that you are writing!");
                return null;
            }

            object obj = this.readData[this.currentItem];
            //this.currentItem++;
            return obj;
        }

        /// <summary>Add another piece of data to send it when IsWriting is true.</summary>
        public void SendNext(object obj)
        {
            if (!this.IsWriting)
            {
                Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
                return;
            }

            this.writeData.Enqueue(obj);
        }

        /// <summary>Turns the stream into a new object[].</summary>
        public object[] ToArray()
        {
            return this.IsWriting ? this.writeData.ToArray() : this.readData;
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref bool myBool)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(myBool);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    myBool = (bool) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref int myInt)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(myInt);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    myInt = (int) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref string value)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(value);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    value = (string) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref char value)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(value);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    value = (char) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref short value)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(value);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    value = (short) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref float obj)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(obj);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    obj = (float) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref Player obj)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(obj);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    obj = (Player) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref Vector3 obj)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(obj);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    obj = (Vector3) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref Vector2 obj)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(obj);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    obj = (Vector2) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }

        /// <summary>
        /// Will read or write the value, depending on the stream's IsWriting value.
        /// </summary>
        public void Serialize(ref Quaternion obj)
        {
            if (this.IsWriting)
            {
                this.writeData.Enqueue(obj);
            }
            else
            {
                if (this.readData.Length > this.currentItem)
                {
                    obj = (Quaternion) this.readData[this.currentItem];
                    this.currentItem++;
                }
            }
        }
    }


    public class SceneManagerHelper
    {
        public static string ActiveSceneName
        {
            get
            {
                Scene s = SceneManager.GetActiveScene();
                return s.name;
            }
        }

        public static int ActiveSceneBuildIndex
        {
            get { return SceneManager.GetActiveScene().buildIndex; }
        }


        #if UNITY_EDITOR
        /// <summary>In Editor, we can access the active scene's name.</summary>
        public static string EditorActiveSceneName
        {
            get { return SceneManager.GetActiveScene().name; }
        }
        #endif
    }


    /// <summary>
    /// The default implementation of a PrefabPool for PUN, which actually Instantiates and Destroys GameObjects but pools a resource.
    /// </summary>
    /// <remarks>
    /// This pool is not actually storing GameObjects for later reuse. Instead, it's destroying used GameObjects.
    /// However, prefabs will be loaded from a Resources folder and cached, which speeds up Instantiation a bit.
    ///
    /// The ResourceCache is public, so it can be filled without relying on the Resources folders.
    /// </remarks>
    public class DefaultPool : IPunPrefabPool
    {
        /// <summary>Contains a GameObject per prefabId, to speed up instantiation.</summary>
        public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
        
        /// <summary>Returns an inactive instance of a networked GameObject, to be used by PUN.</summary>
        /// <param name="prefabId">String identifier for the networked object.</param>
        /// <param name="position">Location of the new object.</param>
        /// <param name="rotation">Rotation of the new object.</param>
        /// <returns></returns>
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject res = null;
            bool cached = this.ResourceCache.TryGetValue(prefabId, out res);
            if (!cached)
            {
                res = (GameObject)Resources.Load(prefabId, typeof(GameObject));
                if (res == null)
                {
                    Debug.LogError("DefaultPool failed to load \"" + prefabId + "\" . Make sure it's in a \"Resources\" folder.");
                }
                else
                {
                    this.ResourceCache.Add(prefabId, res);
                }
            }

            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);

            GameObject instance =GameObject.Instantiate(res, position, rotation) as GameObject;

            if (wasActive) res.SetActive(true);
            return instance;
        }

        /// <summary>Simply destroys a GameObject.</summary>
        /// <param name="gameObject">The GameObject to get rid of.</param>
        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }


    /// <summary>Small number of extension methods that make it easier for PUN to work cross-Unity-versions.</summary>
    public static class PunExtensions
    {
        public static Dictionary<MethodInfo, ParameterInfo[]> ParametersOfMethods = new Dictionary<MethodInfo, ParameterInfo[]>();

        public static ParameterInfo[] GetCachedParemeters(this MethodInfo mo)
        {
            ParameterInfo[] result;
            bool cached = ParametersOfMethods.TryGetValue(mo, out result);

            if (!cached)
            {
                result = mo.GetParameters();
                ParametersOfMethods[mo] = result;
            }

            return result;
        }

        public static PhotonView[] GetPhotonViewsInChildren(this UnityEngine.GameObject go)
        {
            return go.GetComponentsInChildren<PhotonView>(true) as PhotonView[];
        }

        public static PhotonView GetPhotonView(this UnityEngine.GameObject go)
        {
            return go.GetComponent<PhotonView>() as PhotonView;
        }

        /// <summary>compares the squared magnitude of target - second to given float value</summary>
        public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
        {
            return (target - second).sqrMagnitude < sqrMagnitudePrecision; // TODO: inline vector methods to optimize?
        }

        /// <summary>compares the squared magnitude of target - second to given float value</summary>
        public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
        {
            return (target - second).sqrMagnitude < sqrMagnitudePrecision; // TODO: inline vector methods to optimize?
        }

        /// <summary>compares the angle between target and second to given float value</summary>
        public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
        {
            return Quaternion.Angle(target, second) < maxAngle;
        }

        /// <summary>compares two floats and returns true of their difference is less than floatDiff</summary>
        public static bool AlmostEquals(this float target, float second, float floatDiff)
        {
            return Mathf.Abs(target - second) < floatDiff;
        }
    }
}