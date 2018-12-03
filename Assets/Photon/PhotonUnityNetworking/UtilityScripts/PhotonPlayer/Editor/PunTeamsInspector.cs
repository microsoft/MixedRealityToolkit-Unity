// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunTeamsInspector.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Custom inspector for PunTeams
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	[CustomEditor(typeof(PunTeams))]
	public class PunTeamsInspector : Editor {


		Dictionary<PunTeams.Team, bool> _Foldouts ;

		public override void OnInspectorGUI()
		{
			if (_Foldouts==null)
			{
				_Foldouts = new Dictionary<PunTeams.Team, bool>();
			}

			if (PunTeams.PlayersPerTeam!=null)
			{
				foreach (KeyValuePair<PunTeams.Team,List<Player>> _pair in PunTeams.PlayersPerTeam)
				{	
					if (!_Foldouts.ContainsKey(_pair.Key))
					{
						_Foldouts[_pair.Key] = true;
					}

					_Foldouts[_pair.Key] =   EditorGUILayout.Foldout(_Foldouts[_pair.Key],"Team "+_pair.Key +" ("+_pair.Value.Count+")");

					if (_Foldouts[_pair.Key])
					{
						EditorGUI.indentLevel++;
						foreach(Player _player in _pair.Value)
						{
							EditorGUILayout.LabelField("",_player.ToString() + (PhotonNetwork.LocalPlayer==_player?" - You -":""));
						}
						EditorGUI.indentLevel--;
					}
				
				}
			}
		}
	}
}