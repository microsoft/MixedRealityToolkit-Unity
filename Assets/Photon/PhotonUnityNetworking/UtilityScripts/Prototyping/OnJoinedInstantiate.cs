// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnJoinedInstantiate.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  This component will instantiate a network GameObject when a room is joined
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// This component will instantiate a network GameObject when a room is joined
    /// </summary>
	public class OnJoinedInstantiate : MonoBehaviour , IConnectionCallbacks , IMatchmakingCallbacks , ILobbyCallbacks
    {
        public Transform SpawnPosition;
        public float PositionOffset = 2.0f;
        public GameObject[] PrefabsToInstantiate; // set in inspector

        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnJoinedRoom()
        {
            if (this.PrefabsToInstantiate != null)
            {
                foreach (GameObject o in this.PrefabsToInstantiate)
                {
                    Debug.Log("Instantiating: " + o.name);

                    Vector3 spawnPos = Vector3.up;
                    if (this.SpawnPosition != null)
                    {
                        spawnPos = this.SpawnPosition.position;
                    }

                    Vector3 random = Random.insideUnitSphere;
                    random.y = 0;
                    random = random.normalized;
                    Vector3 itempos = spawnPos + this.PositionOffset * random;

                    PhotonNetwork.Instantiate(o.name, itempos, Quaternion.identity, 0);
                }
            }
        }

        public void OnConnected()
        {
        }

		public void OnCustomAuthenticationResponse (Dictionary<string, object> data)
		{
		}

		public void OnCustomAuthenticationFailed (string debugMessage)
		{
		}

        public void OnConnectedToMaster()
        {
        }

		public void OnDisconnected(DisconnectCause cause)
        {
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnJoinedLobby()
        {
        }

        public void OnLeftLobby()
        {
        }

		public void OnLobbyStatisticsUpdate (List<TypedLobbyInfo> lobbyStatistics)
		{
		}

        public void OnCreatedRoom()
        {
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
        }
    }
}