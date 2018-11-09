// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerRoomIndexingInspector.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Custom inspector for PlayerRoomIndexing
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace ExitGames.UtilityScripts
{
	[CustomEditor(typeof(PlayerRoomIndexing))]
	public class PlayerRoomIndexingInspector : Editor {

		PlayerRoomIndexing _target;
	 	int localPlayerIndex;

		void OnEnable () {
			_target = (PlayerRoomIndexing)target;
			_target.OnRoomIndexingChanged += RefreshData;
		}

		void OnDisable () {
			_target = (PlayerRoomIndexing)target;
			_target.OnRoomIndexingChanged -= RefreshData;
		}

		public override void OnInspectorGUI()
		{
			_target = (PlayerRoomIndexing)target;

			_target.OnRoomIndexingChanged += RefreshData;

			if (PhotonNetwork.inRoom)
			{
				EditorGUILayout.LabelField("Player Index", "PhotonPlayer ID");
				if (_target.PlayerIds != null)
				{
					int index = 0;
					foreach(int ID in _target.PlayerIds)
					{
						GUI.enabled = ID!=0;
						EditorGUILayout.LabelField("Player " +index + 
						                           (PhotonNetwork.player.ID==ID?" - You -":"") +
						                           (PhotonNetwork.masterClient.ID==ID?" Master":"")
						                           , ID==0?"n/a":PhotonPlayer.Find(ID).ToStringFull());
						GUI.enabled = true;
						index++;
					}
				}
			}else{
				GUILayout.Label("Room Indexing only works when localPlayer is inside a room");
			}
		}

		/// <summary>
		/// force repaint fo the inspector, else we would not see the new data in the inspector.
		/// This is better then doing it in OnInspectorGUI too many times per frame for no need
		/// </summary>
		void RefreshData()
		{
			Repaint();
		}

	}
}