// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Room list UI View.
    /// </summary>
    public class RoomListView : MonoBehaviourPunCallbacks
    {
        [System.Serializable]
        public class OnJoinRoomEvent : UnityEvent<string> { }

        public OnJoinRoomEvent OnJoinRoom;

        public RoomListCell CellPrototype;

        public Text UpdateStatusText;

		public Text ContentFeedback;

		public InputField LobbyNameInputField;
		public InputField SqlQueryInputField;

        bool _firstUpdate = true;

        Dictionary<string, RoomListCell> roomCellList = new Dictionary<string, RoomListCell>();


        public override void OnEnable()
        {
            base.OnEnable();

            ResetList();
            CellPrototype.gameObject.SetActive(false);
            UpdateStatusText.text = string.Empty;
			ContentFeedback.text = string.Empty;
        }



        public void OnRoomCellJoinButtonClick(string roomName)
        {
            OnJoinRoom.Invoke(roomName);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
			UpdateStatusText.text = "Updated";

			if (roomList.Count == 0 && !PhotonNetwork.InLobby) {
				ContentFeedback.text = "No Room found in lobby "+LobbyNameInputField.text+" Matching: "+SqlQueryInputField.text;
			}
		
            foreach (RoomInfo entry in roomList)
            {
                if (roomCellList.ContainsKey(entry.Name))
                {
                    if (entry.RemovedFromList)
                    {
                        // we delete the cell
                        roomCellList[entry.Name].RemoveFromList();
                        roomCellList.Remove(entry.Name);
                    }
                    else
                    {
                        // we update the cell
                        roomCellList[entry.Name].RefreshInfo(entry);
                    }

                }
                else
                {
                    if (!entry.RemovedFromList)
                    {
                        // we create the cell
                        roomCellList[entry.Name] = Instantiate(CellPrototype);
                        roomCellList[entry.Name].gameObject.SetActive(true);
                        roomCellList[entry.Name].transform.SetParent(CellPrototype.transform.parent, false);
                        roomCellList[entry.Name].AddToList(entry, !_firstUpdate);
                    }
                }
            }

            StartCoroutine("clearStatus");

            _firstUpdate = false;
        }

        IEnumerator clearStatus()
        {
            yield return new WaitForSeconds(1f);

            UpdateStatusText.text = string.Empty;
        }

        public void OnJoinedLobbyCallBack()
        {
            _firstUpdate = true;
			ContentFeedback.text = string.Empty;
        }

        public void GetRoomList()
        {
			ResetList ();

		
			TypedLobby sqlLobby = new TypedLobby(LobbyNameInputField.text, LobbyType.SqlLobby);

			Debug.Log ("Cockpit: GetCustomRoomList() matchmaking against '"+LobbyNameInputField.text+"' SqlLobby using query :  "+SqlQueryInputField.text);

			PhotonNetwork.GetCustomRoomList(sqlLobby, SqlQueryInputField.text ); //"C0 = 'Hello'"

			ContentFeedback.text = "looking for Rooms in Lobby '"+LobbyNameInputField.text+"' Matching: '"+SqlQueryInputField.text;
        }


        public void ResetList()
        {
            _firstUpdate = true;

            foreach (KeyValuePair<string, RoomListCell> entry in roomCellList)
            {

                if (entry.Value != null)
                {
                    Destroy(entry.Value.gameObject);
                }

            }
            roomCellList = new Dictionary<string, RoomListCell>();
        }
    }
}