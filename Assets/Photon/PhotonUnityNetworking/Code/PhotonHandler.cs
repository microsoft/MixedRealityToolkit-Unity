// ----------------------------------------------------------------------------
// <copyright file="PhotonHandler.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// PhotonHandler is a runtime MonoBehaviour to include PUN into the main loop.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using UnityEngine;

    #if UNITY_5_5_OR_NEWER
    using UnityEngine.Profiling;
    #endif


    /// <summary>
    /// Internal MonoBehaviour that allows Photon to run an Update loop.
    /// </summary>
    internal class PhotonHandler : ConnectionHandler, IInRoomCallbacks, IMatchmakingCallbacks
    {
        internal static PhotonHandler Instance;

        protected internal static bool AppQuits;

        protected internal int UpdateInterval; // time [ms] between consecutive SendOutgoingCommands calls

        protected internal int UpdateIntervalOnSerialize; // time [ms] between consecutive RunViewUpdate calls (sending syncs, etc)

        private int nextSendTickCount;

        private int nextSendTickCountOnSerialize;

        private SupportLogger supportLoggerComponent;


        protected override void Awake()
        {
            if (Instance != null && Instance != this && Instance.gameObject != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
            Instance = this;


            this.Client = PhotonNetwork.NetworkingClient;
            base.Awake();


            if (PhotonNetwork.PhotonServerSettings.EnableSupportLogger)
            {
                this.supportLoggerComponent = this.gameObject.AddComponent<SupportLogger>();
                this.supportLoggerComponent.Client = PhotonNetwork.NetworkingClient;
                this.supportLoggerComponent.LogTrafficStats = true;
            }

            this.UpdateInterval = 1000 / PhotonNetwork.SendRate;
            this.UpdateIntervalOnSerialize = 1000 / PhotonNetwork.SerializationRate;

            this.StartFallbackSendAckThread();
        }

        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }


        #if UNITY_5_4_OR_NEWER

        protected void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                PhotonNetwork.NewSceneLoaded();
                PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
            };
        }

        #else

        /// <summary>Called by Unity after a new level was loaded.</summary>
        protected void OnLevelWasLoaded(int level)
        {
            PhotonNetwork.NewSceneLoaded();
            PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
        }

        #endif


        /// <summary>Called by Unity when the application is closed. Disconnects.</summary>
        protected override void OnApplicationQuit()
        {
            AppQuits = true;
            base.OnApplicationQuit();
        }


        protected void FixedUpdate()
        {
            if (PhotonNetwork.NetworkingClient == null)
            {
                Debug.LogError("NetworkPeer broke!");
                return;
            }

            //if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated || PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.OfflineMode)
            //{
            //    return;
            //}


            bool doDispatch = true;
            while (PhotonNetwork.IsMessageQueueRunning && doDispatch)
            {
                // DispatchIncomingCommands() returns true of it found any command to dispatch (event, result or state change)
                Profiler.BeginSample("DispatchIncomingCommands");
                doDispatch = PhotonNetwork.NetworkingClient.LoadBalancingPeer.DispatchIncomingCommands();
                Profiler.EndSample();
            }
        }

        public static int MaxDatagrams = 10;
        public static bool SendAsap;
        
        /// <summary>This corrects the "next time to serialize the state" value by some ms. As LateUpdate typically gets called every 15ms it's better to be early(er) than late to achieve a SerializeRate.</summary>
        private const int SerializeRateFrameCorrection = 8;

        protected void LateUpdate()
        {
            int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000); // avoiding Environment.TickCount, which could be negative on long-running platforms
            if (PhotonNetwork.IsMessageQueueRunning && currentMsSinceStart > this.nextSendTickCountOnSerialize)
            {
                PhotonNetwork.RunViewUpdate();
                this.nextSendTickCountOnSerialize = currentMsSinceStart + this.UpdateIntervalOnSerialize - SerializeRateFrameCorrection;
                this.nextSendTickCount = 0; // immediately send when synchronization code was running
            }

            currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);
            if (SendAsap || currentMsSinceStart > this.nextSendTickCount)
            {
                SendAsap = false;
                bool doSend = true;
                int sendCounter = 0;
                while (PhotonNetwork.IsMessageQueueRunning && doSend && sendCounter < MaxDatagrams)
                {
                    // Send all outgoing commands
                    Profiler.BeginSample("SendOutgoingCommands");
                    doSend = PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
                    sendCounter++;
                    Profiler.EndSample();
                }

                this.nextSendTickCount = currentMsSinceStart + this.UpdateInterval;
            }
        }

        public void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevelIfSynced(); //TODO: do we really need this since we do this inside OnRoomPropertiesUpdate()
        }

        public void OnCreatedRoom()
        {
            PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            PhotonNetwork.LoadLevelIfSynced();
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){}

        public void OnMasterClientSwitched(Player newMasterClient){}

        public void OnFriendListUpdate(System.Collections.Generic.List<FriendInfo> friendList){}

        public void OnCreateRoomFailed(short returnCode, string message){}

        public void OnJoinRoomFailed(short returnCode, string message){}

        public void OnJoinRandomFailed(short returnCode, string message){}

        public void OnLeftRoom(){}

        public void OnPlayerEnteredRoom(Player newPlayer){}

        public void OnPlayerLeftRoom(Player otherPlayer){}
    }
}