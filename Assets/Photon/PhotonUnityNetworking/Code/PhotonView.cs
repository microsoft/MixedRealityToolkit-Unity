// ----------------------------------------------------------------------------
// <copyright file="PhotonView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Contains the PhotonView class.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;
    using System.Collections.Generic;
    using Photon.Realtime;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif



    /// <summary>
    /// A PhotonView identifies an object across the network (viewID) and configures how the controlling client updates remote instances.
    /// </summary>
    /// \ingroup publicApi
    [AddComponentMenu("Photon Networking/Photon View &v")]
    public class PhotonView : MonoBehaviour
    {
        #if UNITY_EDITOR
        [ContextMenu("Open PUN Wizard")]
        void OpenPunWizard()
        {
            EditorApplication.ExecuteMenuItem("Window/Photon Unity Networking");
        }
        #endif

        #if UNITY_EDITOR
        // Suppressing compiler warning "this variable is never used". Only used in the CustomEditor, only in Editor
        #pragma warning disable 0414
        [SerializeField]
        bool ObservedComponentsFoldoutOpen = true;
        #pragma warning restore 0414
        #endif


        [NonSerialized]
        private int ownerId; // TODO maybe changing this should trigger "Was Transfered"!?

        [FormerlySerializedAs("group")]
        public byte Group = 0;

        protected internal bool mixedModeIsReliable = false;


        /// <summary>
        /// Flag to check if ownership of this photonView was set during the lifecycle. Used for checking when joining late if event with mismatched owner and sender needs addressing.
        /// </summary>
        /// <value><c>true</c> if owner ship was transfered; otherwise, <c>false</c>.</value>
        [NonSerialized]
        public bool OwnershipWasTransfered;


        // NOTE: this is now an integer because unity won't serialize short (needed for instantiation). we SEND only a short though!
        // NOTE: prefabs have a prefixField of -1. this is replaced with any currentLevelPrefix that's used at runtime. instantiated GOs get their prefix set pre-instantiation (so those are not -1 anymore)
        public int Prefix
        {
            get
            {
                if (this.prefixField == -1 && PhotonNetwork.NetworkingClient != null)
                {
                    this.prefixField = PhotonNetwork.currentLevelPrefix;
                }

                return this.prefixField;
            }
            set { this.prefixField = value; }
        }

        // this field is serialized by unity. that means it is copied when instantiating a persistent obj into the scene
        [FormerlySerializedAs("prefixBackup")]
        public int prefixField = -1;

        /// <summary>
        /// This is the InstantiationData that was passed when calling PhotonNetwork.Instantiate* (if that was used to spawn this prefab)
        /// </summary>
        public object[] InstantiationData
        {
            get
            {
                if (!this.didAwake)
                {
                    // even though viewID and instantiationID are setup before the GO goes live, this data can't be set. as workaround: fetch it if needed
                    //this.instantiationDataField = PhotonNetwork.FetchInstantiationData(this.InstantiationId);
                    Debug.LogError("PhotonNetwork.FetchInstantiationData() was removed. Can only return this.instantiationDataField.");
                }
                return this.instantiationDataField;
            }
            set { this.instantiationDataField = value; }
        }

        internal object[] instantiationDataField;

        /// <summary>
        /// For internal use only, don't use
        /// </summary>
        protected internal object[] lastOnSerializeDataSent = null;

        /// <summary>
        /// For internal use only, don't use
        /// </summary>
        protected internal object[] lastOnSerializeDataReceived = null;

        [FormerlySerializedAs("synchronization")]
        public ViewSynchronization Synchronization;

        /// <summary>Defines if ownership of this PhotonView is fixed, can be requested or simply taken.</summary>
        /// <remarks>
        /// Note that you can't edit this value at runtime.
        /// The options are described in enum OwnershipOption.
        /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        /// </remarks>
        [FormerlySerializedAs("ownershipTransfer")]
        public OwnershipOption OwnershipTransfer = OwnershipOption.Fixed;

        public List<Component> ObservedComponents;


        [SerializeField]
        private int viewIdField = 0;

        /// <summary>
        /// The ID of the PhotonView. Identifies it in a networked game (per room).
        /// </summary>
        /// <remarks>See: [Network Instantiation](@ref instantiateManual)</remarks>
        public int ViewID
        {
            get { return this.viewIdField; }
            set
            {
                // if ID was 0 for an awakened PhotonView, the view should add itself into the NetworkingClient.photonViewList after setup
                bool viewMustRegister = this.didAwake && this.viewIdField == 0 && value != 0;
                //int oldValue = this.viewIdField;

                // TODO: decide if a viewID can be changed once it wasn't 0. most likely that is not a good idea
                // check if this view is in NetworkingClient.photonViewList and UPDATE said list (so we don't keep the old viewID with a reference to this object)
                // PhotonNetwork.NetworkingClient.RemovePhotonView(this, true);
                
                this.viewIdField = value;
                this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;

                if (viewMustRegister)
                {
                    PhotonNetwork.RegisterPhotonView(this);
                }
                //Debug.Log("Set ViewID: " + value + " ->  owner: " + this.ownerId + " was: "+ oldValue);
            }
        }

        [FormerlySerializedAs("instantiationId")]
        public int InstantiationId; // if the view was instantiated with a GO, this GO has a instantiationID (first view's viewID)

        /// <summary>True if the PhotonView was loaded with the scene (game object) or instantiated with InstantiateSceneObject.</summary>
        /// <remarks>
        /// Scene objects are not owned by a particular player but belong to the scene. Thus they don't get destroyed when their
        /// creator leaves the game and the current Master Client can control them (whoever that is).
        /// The ownerId is 0 (player IDs are 1 and up).
        /// </remarks>
        public bool IsSceneView
        {
            get { return this.CreatorActorNr == 0; }
        }

        /// <summary>
        /// The owner of a PhotonView is the player who created the GameObject with that view. Objects in the scene don't have an owner.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        ///
        /// Ownership can be transferred to another player with PhotonView.TransferOwnership or any player can request
        /// ownership by calling the PhotonView's RequestOwnership method.
        /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        /// </remarks>
        public Player Owner
        {
            get
            {
                // TODO cache Owner
                // using this.OwnerActorNr instead of this.ownerId so that it's the right value during awake.
                return PhotonNetwork.CurrentRoom == null ? null : PhotonNetwork.CurrentRoom.GetPlayer(this.OwnerActorNr);
            }
        }

        public int OwnerActorNr
        {
            get { return this.didAwake ? this.ownerId : this.ViewID / PhotonNetwork.MAX_VIEW_IDS; }
            protected internal set { this.ownerId = value; }
        }

        public Player Controller
        {
            get
            {
                // TODO cache Owner
                if (PhotonNetwork.CurrentRoom == null) return PhotonNetwork.LocalPlayer;
                if (!this.IsOwnerActive) return PhotonNetwork.MasterClient;
                return Owner;
            }
        }

        public int ControllerActorNr
        {
            get { return this.IsOwnerActive ? this.OwnerActorNr : (PhotonNetwork.MasterClient != null ? PhotonNetwork.MasterClient.ActorNumber:-1) ; }
        }

        public bool IsOwnerActive
        {
            get { return this.Owner != null && !this.Owner.IsInactive; }
        }

        public int CreatorActorNr
        {
            get { return this.viewIdField / PhotonNetwork.MAX_VIEW_IDS; }
        }

        /// <summary>
        /// True if the PhotonView is "mine" and can be controlled by this client.
        /// </summary>
        /// <remarks>
        /// PUN has an ownership concept that defines who can control and destroy each PhotonView.
        /// True in case the owner matches the local Player.
        /// True if this is a scene photonview on the Master client.
        /// </remarks>
        public bool IsMine
        {
            get
            {
                // using this.OwnerActorNr instead of this.ownerId so that it's the right value during awake.
                return (this.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber) || (PhotonNetwork.IsMasterClient && !this.IsOwnerActive);
             }
        }

        protected internal bool didAwake;

        [SerializeField]
        protected internal bool isRuntimeInstantiated;

        protected internal bool removedFromLocalViewList;

        internal MonoBehaviour[] RpcMonoBehaviours;


        /// <summary>Called by Unity on start of the application and does a setup the PhotonView.</summary>
        protected internal void Awake()
        {
            if (this.ViewID != 0)
            {
                this.ownerId = this.ViewID / PhotonNetwork.MAX_VIEW_IDS;
                
                // registration might be too late when some script (on this GO) searches this view BUT GetPhotonView() can search ALL in that case
                PhotonNetwork.RegisterPhotonView(this);
            }

            this.didAwake = true;
        }


        protected internal void OnDestroy()
        {
            if (!this.removedFromLocalViewList)
            {
                bool wasInList = PhotonNetwork.LocalCleanPhotonView(this);
                
                if (wasInList && this.InstantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                {
                    Debug.Log("PUN-instantiated '" + this.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
                }
            }
        }

        /// <summary>
        /// Depending on the PhotonView's OwnershipTransfer setting, any client can request to become owner of the PhotonView.
        /// </summary>
        /// <remarks>
        /// Requesting ownership can give you control over a PhotonView, if the OwnershipTransfer setting allows that.
        /// The current owner might have to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        ///
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void RequestOwnership()
        {
            PhotonNetwork.RequestOwnership(this.ViewID, this.ownerId);
        }

        /// <summary>
        /// Transfers the ownership of this PhotonView (and GameObject) to another player.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void TransferOwnership(Player newOwner)
        {
            this.TransferOwnership(newOwner.ActorNumber);
        }

        /// <summary>
        /// Transfers the ownership of this PhotonView (and GameObject) to another player.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void TransferOwnership(int newOwnerId)
        {
            PhotonNetwork.TransferOwnership(this.ViewID, newOwnerId);
            this.ownerId = newOwnerId;  // immediately switch ownership locally, to avoid more updates sent from this client.
        }


        public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
            {
                for (int i = 0; i < this.ObservedComponents.Count; ++i)
                {
                    SerializeComponent(this.ObservedComponents[i], stream, info);
                }
            }
        }

        public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
            {
                for (int i = 0; i < this.ObservedComponents.Count; ++i)
                {
                    DeserializeComponent(this.ObservedComponents[i], stream, info);
                }
            }
        }

        protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
        {
            IPunObservable observable = component as IPunObservable;
            if (observable != null)
            {
                observable.OnPhotonSerializeView(stream, info);
            }
            else
            {
                Debug.LogError("Observed scripts have to implement IPunObservable. " + component + " does not. It is Type: " + component.GetType(), component.gameObject);
            }
        }

        protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
        {
            IPunObservable observable = component as IPunObservable;
            if (observable != null)
            {
                observable.OnPhotonSerializeView(stream, info);
            }
            else
            {
                Debug.LogError("Observed scripts have to implement IPunObservable. "+ component + " does not. It is Type: " + component.GetType(), component.gameObject);
            }
        }


        /// <summary>
        /// Can be used to refesh the list of MonoBehaviours on this GameObject while PhotonNetwork.UseRpcMonoBehaviourCache is true.
        /// </summary>
        /// <remarks>
        /// Set PhotonNetwork.UseRpcMonoBehaviourCache to true to enable the caching.
        /// Uses this.GetComponents<MonoBehaviour>() to get a list of MonoBehaviours to call RPCs on (potentially).
        ///
        /// While PhotonNetwork.UseRpcMonoBehaviourCache is false, this method has no effect,
        /// because the list is refreshed when a RPC gets called.
        /// </remarks>
        public void RefreshRpcMonoBehaviourCache()
        {
            this.RpcMonoBehaviours = this.GetComponents<MonoBehaviour>();
        }


        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// RPC calls can target "All" or the "Others".
        /// Usually, the target "All" gets executed locally immediately after sending the RPC.
        /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        /// <param name="target">The group of targets and the way the RPC gets sent.</param>
        /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RPC(string methodName, RpcTarget target, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, target, false, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// RPC calls can target "All" or the "Others".
        /// Usually, the target "All" gets executed locally immediately after sending the RPC.
        /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        ///<param name="target">The group of targets and the way the RPC gets sent.</param>
        ///<param name="encrypt"> </param>
        ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RpcSecure(string methodName, RpcTarget target, bool encrypt, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// This method allows you to make an RPC calls on a specific player's client.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        /// <param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
        /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RPC(string methodName, Player targetPlayer, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// This method allows you to make an RPC calls on a specific player's client.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        ///<param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
        ///<param name="encrypt"> </param>
        ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RpcSecure(string methodName, Player targetPlayer, bool encrypt, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
        }

        public static PhotonView Get(Component component)
        {
            return component.GetComponent<PhotonView>();
        }

        public static PhotonView Get(GameObject gameObj)
        {
            return gameObj.GetComponent<PhotonView>();
        }

        public static PhotonView Find(int viewID)
        {
            return PhotonNetwork.GetPhotonView(viewID);
        }

        public override string ToString()
        {
            return string.Format("View {0}{3} on {1} {2}", this.ViewID, (this.gameObject != null) ? this.gameObject.name : "GO==null", (this.IsSceneView) ? "(scene)" : string.Empty, this.Prefix > 0 ? "lvl"+this.Prefix : "");
        }
    }
}