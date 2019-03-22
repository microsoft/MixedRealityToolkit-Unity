// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNumberingInspector.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Custom inspector for PlayerNumbering
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	[CustomEditor(typeof(PlayerNumbering))]
	public class PlayerNumberingInspector : Editor {

	 	int localPlayerIndex;

		void OnEnable () {
		    PlayerNumbering.OnPlayerNumberingChanged += RefreshData;
		}

		void OnDisable () {
		    PlayerNumbering.OnPlayerNumberingChanged -= RefreshData;
		}

		public override void OnInspectorGUI()
		{
            DrawDefaultInspector();

		    PlayerNumbering.OnPlayerNumberingChanged += RefreshData;

			if (PhotonNetwork.InRoom)
			{
				EditorGUILayout.LabelField("Player Index", "Player ID");
				if (PlayerNumbering.SortedPlayers != null)
				{
					foreach(Player punPlayer in PlayerNumbering.SortedPlayers)
					{
						GUI.enabled = punPlayer.ActorNumber > 0;
						EditorGUILayout.LabelField("Player " +punPlayer.GetPlayerNumber() + (punPlayer.IsLocal?" - You -":""), punPlayer.ActorNumber == 0?"n/a":punPlayer.ToStringFull());
						GUI.enabled = true;
					}
				}
			}else{
				GUILayout.Label("PlayerNumbering only works when localPlayer is inside a room");
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