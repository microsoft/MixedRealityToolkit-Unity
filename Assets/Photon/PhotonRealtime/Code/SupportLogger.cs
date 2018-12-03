// ----------------------------------------------------------------------------
// <copyright file="SupportLogger.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Implements callbacks of the Realtime API to logs selected information
//   for support cases.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    #if SUPPORTED_UNITY
    using UnityEngine;
    #endif

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif

    /// <summary>
    /// Helper class to debug log basic information about Photon client and vital traffic statistics.
    /// </summary>
    /// <remarks>
    /// Set SupportLogger.Client for this to work.
    /// </remarks>
    #if SUPPORTED_UNITY
	[AddComponentMenu("")] // hide from Unity Menus and searches
	public class SupportLogger : MonoBehaviour, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks
    #else
	public class SupportLogger : IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks , ILobbyCallbacks
    #endif
    {
        /// <summary>
        /// Toggle to enable or disable traffic statistics logging.
        /// </summary>
        public bool LogTrafficStats;
        private bool loggedStillOfflineMessage;

        private LoadBalancingClient client;

        /// <summary>
        /// Photon client to log information and statistics from.
        /// </summary>
        public LoadBalancingClient Client
        {
            get { return this.client; }
            set
            {
                if (this.client != value)
                {
                    if (this.client != null)
                    {
                        this.client.RemoveCallbackTarget(this);
                    }
                    this.client = value;
                    this.client.AddCallbackTarget(this);
                }
            }
        }


        #if SUPPORTED_UNITY
        protected void Start()
        {
            if (this.LogTrafficStats)
            {
                this.InvokeRepeating("LogStats", 10, 10);
            }
        }

        protected void OnApplicationPause(bool pause)
        {
            Debug.Log("SupportLogger OnApplicationPause: " + pause + " connected: " + this.client.IsConnected);
        }

        protected void OnApplicationQuit()
        {
            this.CancelInvoke();
        }
        #endif

        /// <summary>
        /// Debug logs vital traffic statistics about the attached Photon Client.
        /// </summary>
        public void LogStats()
        {
            if (this.client.State == ClientState.PeerCreated)
            {
                if (!this.loggedStillOfflineMessage)
                {
                    this.loggedStillOfflineMessage = true;
                    Debug.Log("SupportLogger Photon Client is not connected yet: this logger won't be able to capture the state.");
                }
                return;
            }

            if (this.LogTrafficStats)
            {
                Debug.Log("SupportLogger " + this.client.LoadBalancingPeer.VitalStatsToString(false));
            }
        }

        /// <summary>
        /// Debug logs basic information (AppId, AppVersion, PeerID, Server address, Region) about the attached Photon Client.
        /// </summary>
        private void LogBasics()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SupportLogger Info: ");
            sb.AppendFormat("AppID: {0}*** AppVersion: {1} PeerID: {2} ", this.client.AppId.Substring(0, 8), this.client.AppVersion, this.client.LoadBalancingPeer.PeerID);
            sb.AppendFormat("Server: {0} IP: {1} Region: {2}", this.client.NameServerHost, this.client.CurrentServerAddress, this.client.CloudRegion);

            Debug.Log(sb.ToString());
        }


        public void OnConnected()
        {
            Debug.Log("SupportLogger OnConnected().");
            this.LogBasics();

            if (this.LogTrafficStats)
            {
                this.client.EnableLobbyStatistics = true;
            }
        }

        public void OnConnectedToMaster()
        {
			Debug.Log("SupportLogger OnConnectedToMaster().");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
			Debug.Log("SupportLogger OnFriendListUpdate(friendList).");
        }

        public void OnJoinedLobby()
        {
            Debug.Log("SupportLogger OnJoinedLobby(" + this.client.CurrentLobby + ").");
        }

        public void OnLeftLobby()
        {
			Debug.Log("SupportLogger OnLeftLobby().");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
			Debug.Log("SupportLogger OnCreateRoomFailed("+returnCode+","+message+").");
        }

        public void OnJoinedRoom()
        {
            Debug.Log("SupportLogger OnJoinedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
			Debug.Log("SupportLogger OnJoinRandomFailed("+returnCode+","+message+").");
        }

        public void OnCreatedRoom()
        {
            Debug.Log("SupportLogger OnCreatedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnLeftRoom()
        {
            Debug.Log("SupportLogger OnLeftRoom().");
        }

		public void OnDisconnected(DisconnectCause cause)
        {
			Debug.Log("SupportLogger OnDisconnected(" + cause + ").");
			this.LogBasics();
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
			Debug.Log("SupportLogger OnRegionListReceived(regionHandler).");
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
			Debug.Log("SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
			Debug.Log("SupportLogger OnPlayerEnteredRoom("+newPlayer+").");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
			Debug.Log("SupportLogger OnPlayerLeftRoom("+otherPlayer+").");
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
			Debug.Log("SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
			Debug.Log("SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
			Debug.Log("SupportLogger OnMasterClientSwitched("+newMasterClient+").");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
			Debug.Log("SupportLogger OnCustomAuthenticationResponse("+data.ToStringFull()+").");
        }

		public void OnCustomAuthenticationFailed (string debugMessage)
		{
			Debug.Log("SupportLogger OnCustomAuthenticationFailed("+debugMessage+").");
		}

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
			Debug.Log("SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
        }


        #if !SUPPORTED_UNITY
        private static class Debug
        {
            public static void Log(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogWarning(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogError(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }
        #endif
    }
}