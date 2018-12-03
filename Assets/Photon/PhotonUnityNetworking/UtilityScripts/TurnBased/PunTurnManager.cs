// ----------------------------------------------------------------------------
// <copyright file="PunTurnManager.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//  Manager for Turn Based games, using PUN
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using UnityEngine;

using Photon.Realtime;

using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Pun turnBased Game manager.
    /// Provides an Interface (IPunTurnManagerCallbacks) for the typical turn flow and logic, between players
    /// Provides Extensions for Player, Room and RoomInfo to feature dedicated api for TurnBased Needs
    /// </summary>
	public class PunTurnManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        /// <summary>
        /// Wraps accessing the "turn" custom properties of a room.
        /// </summary>
        /// <value>The turn index</value>
        public int Turn
        {
            get { return PhotonNetwork.CurrentRoom.GetTurn(); }
            private set
            {

                _isOverCallProcessed = false;

                PhotonNetwork.CurrentRoom.SetTurn(value, true);
            }
        }


        /// <summary>
        /// The duration of the turn in seconds.
        /// </summary>
        public float TurnDuration = 20f;

        /// <summary>
        /// Gets the elapsed time in the current turn in seconds
        /// </summary>
        /// <value>The elapsed time in the turn.</value>
        public float ElapsedTimeInTurn
        {
            get { return ((float) (PhotonNetwork.ServerTimestamp - PhotonNetwork.CurrentRoom.GetTurnStart())) / 1000.0f; }
        }


        /// <summary>
        /// Gets the remaining seconds for the current turn. Ranges from 0 to TurnDuration
        /// </summary>
        /// <value>The remaining seconds fo the current turn</value>
        public float RemainingSecondsInTurn
        {
            get { return Mathf.Max(0f, this.TurnDuration - this.ElapsedTimeInTurn); }
        }


        /// <summary>
        /// Gets a value indicating whether the turn is completed by all.
        /// </summary>
        /// <value><c>true</c> if this turn is completed by all; otherwise, <c>false</c>.</value>
        public bool IsCompletedByAll
        {
            get { return PhotonNetwork.CurrentRoom != null && Turn > 0 && this.finishedPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount; }
        }

        /// <summary>
        /// Gets a value indicating whether the current turn is finished by me.
        /// </summary>
        /// <value><c>true</c> if the current turn is finished by me; otherwise, <c>false</c>.</value>
        public bool IsFinishedByMe
        {
            get { return this.finishedPlayers.Contains(PhotonNetwork.LocalPlayer); }
        }

        /// <summary>
        /// Gets a value indicating whether the current turn is over. That is the ElapsedTimeinTurn is greater or equal to the TurnDuration
        /// </summary>
        /// <value><c>true</c> if the current turn is over; otherwise, <c>false</c>.</value>
        public bool IsOver
        {
            get { return this.RemainingSecondsInTurn <= 0f; }
        }

        /// <summary>
        /// The turn manager listener. Set this to your own script instance to catch Callbacks
        /// </summary>
        public IPunTurnManagerCallbacks TurnManagerListener;


        /// <summary>
        /// The finished players.
        /// </summary>
        private readonly HashSet<Player> finishedPlayers = new HashSet<Player>();

        /// <summary>
        /// The turn manager event offset event message byte. Used internaly for defining data in Room Custom Properties
        /// </summary>
        public const byte TurnManagerEventOffset = 0;

        /// <summary>
        /// The Move event message byte. Used internaly for saving data in Room Custom Properties
        /// </summary>
        public const byte EvMove = 1 + TurnManagerEventOffset;

        /// <summary>
        /// The Final Move event message byte. Used internaly for saving data in Room Custom Properties
        /// </summary>
        public const byte EvFinalMove = 2 + TurnManagerEventOffset;

        // keep track of message calls
        private bool _isOverCallProcessed = false;

        #region MonoBehaviour CallBack


        void Start(){}

        void Update()
        {
            if (Turn > 0 && this.IsOver && !_isOverCallProcessed)
            {
                _isOverCallProcessed = true;
                this.TurnManagerListener.OnTurnTimeEnds(this.Turn);
            }

        }

        #endregion


        /// <summary>
        /// Tells the TurnManager to begins a new turn.
        /// </summary>
        public void BeginTurn()
        {
            Turn = this.Turn + 1; // note: this will set a property in the room, which is available to the other players.
        }


        /// <summary>
        /// Call to send an action. Optionally finish the turn, too.
        /// The move object can be anything. Try to optimize though and only send the strict minimum set of information to define the turn move.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="finished"></param>
        public void SendMove(object move, bool finished)
        {
            if (IsFinishedByMe)
            {
                UnityEngine.Debug.LogWarning("Can't SendMove. Turn is finished by this player.");
                return;
            }

            // along with the actual move, we have to send which turn this move belongs to
            Hashtable moveHt = new Hashtable();
            moveHt.Add("turn", Turn);
            moveHt.Add("move", move);

            byte evCode = (finished) ? EvFinalMove : EvMove;
            PhotonNetwork.RaiseEvent(evCode, moveHt, new RaiseEventOptions() {CachingOption = EventCaching.AddToRoomCache}, SendOptions.SendReliable);
            if (finished)
            {
                PhotonNetwork.LocalPlayer.SetFinishedTurn(Turn);
            }

            // the server won't send the event back to the origin (by default). to get the event, call it locally
            // (note: the order of events might be mixed up as we do this locally)
			ProcessOnEvent(evCode, moveHt, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        /// <summary>
        /// Gets if the player finished the current turn.
        /// </summary>
        /// <returns><c>true</c>, if player finished the current turn, <c>false</c> otherwise.</returns>
        /// <param name="player">The Player to check for</param>
        public bool GetPlayerFinishedTurn(Player player)
        {
            if (player != null && this.finishedPlayers != null && this.finishedPlayers.Contains(player))
            {
                return true;
            }

            return false;
        }

        #region Callbacks

		// called internally
		void ProcessOnEvent(byte eventCode, object content, int senderId)
		{
			Player sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);
			switch (eventCode)
			{
			case EvMove:
				{
					Hashtable evTable = content as Hashtable;
					int turn = (int)evTable["turn"];
					object move = evTable["move"];
					this.TurnManagerListener.OnPlayerMove(sender, turn, move);

					break;
				}
			case EvFinalMove:
				{
					Hashtable evTable = content as Hashtable;
					int turn = (int)evTable["turn"];
					object move = evTable["move"];

					if (turn == this.Turn)
					{
						this.finishedPlayers.Add(sender);

						this.TurnManagerListener.OnPlayerFinished(sender, turn, move);

					}

					if (IsCompletedByAll)
					{
						this.TurnManagerListener.OnTurnCompleted(this.Turn);
					}
					break;
				}
			}
		}

        /// <summary>
        /// Called by PhotonNetwork.OnEventCall registration
        /// </summary>
		/// <param name="photonEvent">Photon event.</param>
		public void OnEvent(EventData photonEvent)
        {
			ProcessOnEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
        }

        /// <summary>
        /// Called by PhotonNetwork
        /// </summary>
        /// <param name="propertiesThatChanged">Properties that changed.</param>
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {

            //   Debug.Log("OnRoomPropertiesUpdate: "+propertiesThatChanged.ToStringFull());

            if (propertiesThatChanged.ContainsKey("Turn"))
            {
                _isOverCallProcessed = false;
                this.finishedPlayers.Clear();
                this.TurnManagerListener.OnTurnBegins(this.Turn);
            }
        }

        #endregion
    }


    public interface IPunTurnManagerCallbacks
    {
        /// <summary>
        /// Called the turn begins event.
        /// </summary>
        /// <param name="turn">Turn Index</param>
        void OnTurnBegins(int turn);

        /// <summary>
        /// Called when a turn is completed (finished by all players)
        /// </summary>
        /// <param name="turn">Turn Index</param>
        void OnTurnCompleted(int turn);

        /// <summary>
        /// Called when a player moved (but did not finish the turn)
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <param name="turn">Turn Index</param>
        /// <param name="move">Move Object data</param>
        void OnPlayerMove(Player player, int turn, object move);

        /// <summary>
        /// When a player finishes a turn (includes the action/move of that player)
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <param name="turn">Turn index</param>
        /// <param name="move">Move Object data</param>
        void OnPlayerFinished(Player player, int turn, object move);


        /// <summary>
        /// Called when a turn completes due to a time constraint (timeout for a turn)
        /// </summary>
        /// <param name="turn">Turn index</param>
        void OnTurnTimeEnds(int turn);
    }


    public static class TurnExtensions
    {
        /// <summary>
        /// currently ongoing turn number
        /// </summary>
        public static readonly string TurnPropKey = "Turn";

        /// <summary>
        /// start (server) time for currently ongoing turn (used to calculate end)
        /// </summary>
        public static readonly string TurnStartPropKey = "TStart";

        /// <summary>
        /// Finished Turn of Actor (followed by number)
        /// </summary>
        public static readonly string FinishedTurnPropKey = "FToA";

        /// <summary>
        /// Sets the turn.
        /// </summary>
        /// <param name="room">Room reference</param>
        /// <param name="turn">Turn index</param>
        /// <param name="setStartTime">If set to <c>true</c> set start time.</param>
        public static void SetTurn(this Room room, int turn, bool setStartTime = false)
        {
            if (room == null || room.CustomProperties == null)
            {
                return;
            }

            Hashtable turnProps = new Hashtable();
            turnProps[TurnPropKey] = turn;
            if (setStartTime)
            {
                turnProps[TurnStartPropKey] = PhotonNetwork.ServerTimestamp;
            }

            room.SetCustomProperties(turnProps);
        }

        /// <summary>
        /// Gets the current turn from a RoomInfo
        /// </summary>
        /// <returns>The turn index </returns>
        /// <param name="room">RoomInfo reference</param>
        public static int GetTurn(this RoomInfo room)
        {
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
            {
                return 0;
            }

            return (int) room.CustomProperties[TurnPropKey];
        }


        /// <summary>
        /// Returns the start time when the turn began. This can be used to calculate how long it's going on.
        /// </summary>
        /// <returns>The turn start.</returns>
        /// <param name="room">Room.</param>
        public static int GetTurnStart(this RoomInfo room)
        {
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnStartPropKey))
            {
                return 0;
            }

            return (int) room.CustomProperties[TurnStartPropKey];
        }

        /// <summary>
        /// gets the player's finished turn (from the ROOM properties)
        /// </summary>
        /// <returns>The finished turn index</returns>
        /// <param name="player">Player reference</param>
        public static int GetFinishedTurn(this Player player)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
            {
                return 0;
            }

            string propKey = FinishedTurnPropKey + player.ActorNumber;
            return (int) room.CustomProperties[propKey];
        }

        /// <summary>
        /// Sets the player's finished turn (in the ROOM properties)
        /// </summary>
        /// <param name="player">Player Reference</param>
        /// <param name="turn">Turn Index</param>
        public static void SetFinishedTurn(this Player player, int turn)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room == null || room.CustomProperties == null)
            {
                return;
            }

            string propKey = FinishedTurnPropKey + player.ActorNumber;
            Hashtable finishedTurnProp = new Hashtable();
            finishedTurnProp[propKey] = turn;

            room.SetCustomProperties(finishedTurnProp);
        }
    }
}