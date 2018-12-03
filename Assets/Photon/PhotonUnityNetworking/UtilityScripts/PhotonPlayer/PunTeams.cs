// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunTeams.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
// Implements teams in a room/game with help of player properties. Access them by Player.GetTeam extension.
// </summary>
// <remarks>
// Teams are defined by enum Team. Change this to get more / different teams.
// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
// </remarks>                                                                                           
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Implements teams in a room/game with help of player properties. Access them by Player.GetTeam extension.
    /// </summary>
    /// <remarks>
    /// Teams are defined by enum Team. Change this to get more / different teams.
    /// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
    /// </remarks>
    public class PunTeams : MonoBehaviourPunCallbacks
    {
        /// <summary>Enum defining the teams available. First team should be neutral (it's the default value any field of this enum gets).</summary>
        public enum Team : byte { none, red, blue };

        /// <summary>The main list of teams with their player-lists. Automatically kept up to date.</summary>
        /// <remarks>Note that this is static. Can be accessed by PunTeam.PlayersPerTeam. You should not modify this.</remarks>
        public static Dictionary<Team, List<Player>> PlayersPerTeam;

        /// <summary>Defines the player custom property name to use for team affinity of "this" player.</summary>
        public const string TeamPlayerProp = "team";


        #region Events by Unity and Photon

        public void Start()
        {
            PlayersPerTeam = new Dictionary<Team, List<Player>>();
            Array enumVals = Enum.GetValues(typeof(Team));
            foreach (var enumVal in enumVals)
            {
                PlayersPerTeam[(Team)enumVal] = new List<Player>();
            }
        }

        public override void OnDisable()
        {
            PlayersPerTeam = new Dictionary<Team, List<Player>>();
        }

        /// <summary>Needed to update the team lists when joining a room.</summary>
        /// <remarks>Called by PUN. See enum MonoBehaviourPunCallbacks for an explanation.</remarks>
        public override void OnJoinedRoom()
        {

            this.UpdateTeams();
        }

        public override void OnLeftRoom()
        {
            Start();
        }

        /// <summary>Refreshes the team lists. It could be a non-team related property change, too.</summary>
        /// <remarks>Called by PUN. See enum MonoBehaviourPunCallbacks for an explanation.</remarks>
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            this.UpdateTeams();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            this.UpdateTeams();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            this.UpdateTeams();
        }

        #endregion


        public void UpdateTeams()
        {
            Array enumVals = Enum.GetValues(typeof(Team));
            foreach (var enumVal in enumVals)
            {
                PlayersPerTeam[(Team)enumVal].Clear();
            }

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                Team playerTeam = player.GetTeam();
                PlayersPerTeam[playerTeam].Add(player);
            }
        }
    }

    /// <summary>Extension used for PunTeams and Player class. Wraps access to the player's custom property.</summary>
    public static class TeamExtensions
    {
        /// <summary>Extension for Player class to wrap up access to the player's custom property.</summary>
        /// <returns>PunTeam.Team.none if no team was found (yet).</returns>
        public static PunTeams.Team GetTeam(this Player player)
        {
            object teamId;
            if (player.CustomProperties.TryGetValue(PunTeams.TeamPlayerProp, out teamId))
            {
                return (PunTeams.Team)teamId;
            }

            return PunTeams.Team.none;
        }

        /// <summary>Switch that player's team to the one you assign.</summary>
        /// <remarks>Internally checks if this player is in that team already or not. Only team switches are actually sent.</remarks>
        /// <param name="player"></param>
        /// <param name="team"></param>
        public static void SetTeam(this Player player, PunTeams.Team team)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
                return;
            }

            PunTeams.Team currentTeam = player.GetTeam();
            if (currentTeam != team)
            {
                player.SetCustomProperties(new Hashtable() { { PunTeams.TeamPlayerProp, (byte)team } });
            }
        }
    }
}