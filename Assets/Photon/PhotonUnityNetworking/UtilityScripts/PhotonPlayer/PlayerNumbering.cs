// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNumbering.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Assign numbers to Players in a room. Uses Room custom Properties
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Implements consistent numbering in a room/game with help of room properties. Access them by Player.GetPlayerNumber() extension.
    /// </summary>
    /// <remarks>
    /// indexing ranges from 0 to the maximum number of Players.
    /// indexing remains for the player while in room.
	/// If a Player is numbered 2 and player numbered 1 leaves, numbered 1 become vacant and will assigned to the future player joining (the first available vacant number is assigned when joining)
    /// </remarks>
    public class PlayerNumbering : MonoBehaviourPunCallbacks
    {
        //TODO: Add a "numbers available" bool, to allow easy access to this?!

        #region Public Properties

        /// <summary>
        /// The instance. EntryPoint to query about Room Indexing.
        /// </summary>
        public static PlayerNumbering instance;

        public static Player[] SortedPlayers;

        /// <summary>
        /// OnPlayerNumberingChanged delegate. Use
        /// </summary>
        public delegate void PlayerNumberingChanged();
        /// <summary>
        /// Called everytime the room Indexing was updated. Use this for discrete updates. Always better than brute force calls every frame.
        /// </summary>
        public static event PlayerNumberingChanged OnPlayerNumberingChanged;


        /// <summary>Defines the room custom property name to use for room player indexing tracking.</summary>
        public const string RoomPlayerIndexedProp = "pNr";

        /// <summary>
        /// dont destroy on load flag for this Component's GameObject to survive Level Loading.
        /// </summary>
        public bool dontDestroyOnLoad = false;


        #endregion


        #region MonoBehaviours methods

        public void Awake()
        {

            if (instance != null && instance != this && instance.gameObject != null)
            {
                GameObject.DestroyImmediate(instance.gameObject);
            }

            instance = this;
            if (dontDestroyOnLoad)
            { 
                DontDestroyOnLoad(this.gameObject);
            }

            this.RefreshData();
        }

        #endregion


        #region PunBehavior Overrides

        public override void OnJoinedRoom()
        {
            this.RefreshData();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Remove(PlayerNumbering.RoomPlayerIndexedProp);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            this.RefreshData();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            this.RefreshData();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps != null && changedProps.ContainsKey(PlayerNumbering.RoomPlayerIndexedProp))
            {
                this.RefreshData();
            }
        }

        #endregion


        // each player can select it's own playernumber in a room, if all "older" players already selected theirs


        /// <summary>
        /// Internal call Refresh the cached data and call the OnPlayerNumberingChanged delegate.
        /// </summary>
       public void RefreshData()
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() >= 0)
            {
                SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();
                if (OnPlayerNumberingChanged != null)
                {
                    OnPlayerNumberingChanged();
                }
                return;
            }


            HashSet<int> usedInts = new HashSet<int>();
            Player[] sorted = PhotonNetwork.PlayerList.OrderBy((p) => p.ActorNumber).ToArray();

            string allPlayers = "all players: ";
            foreach (Player player in sorted)
            {
                allPlayers += player.ActorNumber + "=pNr:"+player.GetPlayerNumber()+", ";

                int number = player.GetPlayerNumber();

                // if it's this user, select a number and break
                // else:
                    // check if that user has a number
                    // if not, break!
                    // else remember used numbers

                if (player.IsLocal)
                {
					Debug.Log ("PhotonNetwork.CurrentRoom.PlayerCount = " + PhotonNetwork.CurrentRoom.PlayerCount);

                    // select a number
                    for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                    {
                        if (!usedInts.Contains(i))
                        {
                            player.SetPlayerNumber(i);
                            break;
                        }
                    }
                    // then break
                    break;
                }
                else
                {
                    if (number < 0)
                    {
                        break;
                    }
                    else
                    {
                        usedInts.Add(number);
                    }
                }
            }

            //Debug.Log(allPlayers);
            //Debug.Log(PhotonNetwork.LocalPlayer.ToStringFull() + " has PhotonNetwork.player.GetPlayerNumber(): " + PhotonNetwork.LocalPlayer.GetPlayerNumber());

            SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();
            if (OnPlayerNumberingChanged != null)
            {
                OnPlayerNumberingChanged();
            }
        }
    }



    /// <summary>Extension used for PlayerRoomIndexing and Player class.</summary>
    public static class PlayerNumberingExtensions
    {
        /// <summary>Extension for Player class to wrap up access to the player's custom property.
		/// Make sure you use the delegate 'OnPlayerNumberingChanged' to knoiw when you can query the PlayerNumber. Numbering can changes over time or not be yet assigned during the initial phase ( when player creates a room for example)
		/// </summary>
        /// <returns>persistent index in room. -1 for no indexing</returns>
        public static int GetPlayerNumber(this Player player)
        {
			if (player == null) {
				return -1;
			}

            if (PhotonNetwork.OfflineMode)
            {
                return 0;
            }
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                return -1;
            }

            object value;
			if (player.CustomProperties.TryGetValue (PlayerNumbering.RoomPlayerIndexedProp, out value)) {
				return (byte)value;
			}
            return -1;
        }

		/// <summary>
		/// Sets the player number.
		/// It's not recommanded to manually interfere with the playerNumbering, but possible.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="playerNumber">Player number.</param>
        public static void SetPlayerNumber(this Player player, int playerNumber)
        {
			if (player == null) {
				return;
			}

            if (PhotonNetwork.OfflineMode)
            {
                return;
            }

            if (playerNumber < 0)
            {
                Debug.LogWarning("Setting invalid playerNumber: " + playerNumber + " for: " + player.ToStringFull());
            }

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogWarning("SetPlayerNumber was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
                return;
            }

            int current = player.GetPlayerNumber();
            if (current != playerNumber)
            {
				Debug.Log("PlayerNumbering: Set number "+playerNumber);
                player.SetCustomProperties(new Hashtable() { { PlayerNumbering.RoomPlayerIndexedProp, (byte)playerNumber } });
            }
        }
    }
}