// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FriendListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{

    /// <summary>
    /// Friend list UI view.
    /// </summary>
    public class FriendListView : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Friend detail class
        /// This info comes from your social network and is meant to be matched against the friendInfo from Photon
        /// </summary>
        [Serializable]
        public class FriendDetail
        {
            public FriendDetail(string NickName, string UserId)
            {
                this.NickName = NickName;
                this.UserId = UserId;
            }

            public string NickName;
            public string UserId;
        }

        public FriendListCell CellPrototype;

		public Text ContentFeedback;

        public Text UpdateStatusText;

        [System.Serializable]
        public class OnJoinRoomEvent : UnityEvent<string> { }

        public OnJoinRoomEvent OnJoinRoom;

        Dictionary<string, FriendListCell> FriendCellList = new Dictionary<string, FriendListCell>();

        string[] FriendsLUT = new string[0];


        void Awake()
        {
            CellPrototype.gameObject.SetActive(false);
		
        }

        public override void OnEnable()
        {
            base.OnEnable();

            UpdateStatusText.text = string.Empty;
			ContentFeedback.text = string.Empty;;
        }


        public void SetFriendDetails(FriendDetail[] friendList)
        {
            ResetList();

            List<string> _list = new List<string>();
            foreach (FriendDetail _friend in friendList)
			{
                if (_friend.UserId != PhotonNetwork.LocalPlayer.UserId)
                {
                    FriendCellList[_friend.UserId] = Instantiate(CellPrototype);
                    FriendCellList[_friend.UserId].transform.SetParent(CellPrototype.transform.parent, false);
                    FriendCellList[_friend.UserId].gameObject.SetActive(true);
                    FriendCellList[_friend.UserId].RefreshInfo(_friend);

                    _list.Add(_friend.UserId);
                }
            }

            this.FriendsLUT = _list.ToArray<string>();

            FindFriends();
        }

        public void FindFriends()
        {

            PhotonNetwork.FindFriends(FriendsLUT);

			ContentFeedback.text = "Finding Friends...";
        }

        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            StartCoroutine("UpdateUIPing");

			if (friendList.Count == 0)
			{
				ContentFeedback.text = "No Friends Found";
			}else
			{
				ContentFeedback.text = string.Empty;
			}


            foreach (FriendInfo _info in friendList)
            {
                if (FriendCellList.ContainsKey(_info.UserId))
                {
                    FriendCellList[_info.UserId].RefreshInfo(_info);
                }
            }
        }

        public void OnRoomListUpdateCallBack(List<RoomInfo> roomList)
        {
			PhotonNetwork.FindFriends(FriendsLUT);
        }

        public void JoinFriendRoom(string RoomName)
        {
            //Debug.Log("FriendListView:JoinFriendRoom " + RoomName);
            OnJoinRoom.Invoke(RoomName);
        }

        IEnumerator UpdateUIPing()
        {
            UpdateStatusText.text = "Updated";

            yield return new WaitForSeconds(1f);

            UpdateStatusText.text = string.Empty;
        }


        public void ResetList()
        {
            foreach (KeyValuePair<string, FriendListCell> entry in FriendCellList)
            {
                if (entry.Value != null)
                {
                    Destroy(entry.Value.gameObject);
                }
            }

            FriendCellList = new Dictionary<string, FriendListCell>();
        }
    }
}