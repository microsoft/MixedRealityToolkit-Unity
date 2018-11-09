// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerRoomIndexing.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Assign numbers to Players in a room. Uses Room custom Properties
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Photon;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;



namespace ExitGames.UtilityScripts
{
	/// <summary>
	/// Implements consistent indexing in a room/game with help of room properties. Access them by PhotonPlayer.GetRoomIndex() extension.
	/// </summary>
	/// <remarks>
	/// indexing ranges from 0 to the maximum number of Players.
	/// indexing remains for the player while in room.
	/// If a Player is indexed 2 and player indexes 1 leaves, index 1 become vacant and will assigned to the future player joining (the first available vacant index is assigned when joining)
	/// </remarks>
	public class PlayerRoomIndexing : PunBehaviour
	{

		#region Public Properties

		/// <summary>
		/// The instance. EntryPoint to query about Room Indexing.
		/// </summary>
		public static PlayerRoomIndexing instance;

		/// <summary>
		/// OnRoomIndexingChanged delegate. Use
		/// </summary>
		public delegate void RoomIndexingChanged();
		/// <summary>
		/// Called everytime the room Indexing was updated. Use this for discrete updates. Always better than brute force calls every frame.
		/// </summary>
		public RoomIndexingChanged OnRoomIndexingChanged;

		/// <summary>Defines the room custom property name to use for room player indexing tracking.</summary>
		public const string RoomPlayerIndexedProp = "PlayerIndexes";

		/// <summary>
		/// Cached list of Player indexes. You can use <PhotonPlayer>.GetRoomIndex()
		/// </summary>
		/// <value>The player identifiers.</value>
		public int[] PlayerIds
		{
			get {
				return _playerIds;
			}
		}

		#endregion



		#region Private Properties

		int[] _playerIds;
		object _indexes;
		Dictionary<int,int> _indexesLUT;
		List<bool> _indexesPool;
		PhotonPlayer _p;

		#endregion

		#region MonoBehaviours methods

		public void Awake()
		{
			if (instance!=null)
			{
				Debug.LogError("Existing instance of PlayerRoomIndexing found. Only One instance is required at the most. Please correct and have only one at any time.");
			}
			instance = this;

			// check if we are already in room, likely if component was added at runtime or came late into scene
			if (PhotonNetwork.room!=null)
			{
				SanitizeIndexing(true);
			}
		}

		#endregion


		#region PunBehavior Overrides

		public override void OnJoinedRoom()
		{
			if (PhotonNetwork.isMasterClient)
			{
				AssignIndex(PhotonNetwork.player);
			}else{
				RefreshData();
			}
		}

		public override void OnLeftRoom()
		{
			RefreshData();
		}

		public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
		{
			if (PhotonNetwork.isMasterClient)
			{
				AssignIndex(newPlayer);
			}

		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
			if (PhotonNetwork.isMasterClient)
			{
				UnAssignIndex(otherPlayer);
			}
		}

		public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
		{
			if (propertiesThatChanged.ContainsKey(PlayerRoomIndexing.RoomPlayerIndexedProp))
			{
				RefreshData();
			}
		}


		public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
		{
			if (PhotonNetwork.isMasterClient)
			{
				SanitizeIndexing();
			}
		}

		#endregion

		/// <summary>Get the room index of a particular PhotonPlayer. You can also use <PhotonPlayer>.GetRoomIndex() </summary>
		/// <returns>persistent index in room. -1 for none</returns>
		public int GetRoomIndex( PhotonPlayer player)
		{
			if (_indexesLUT!=null && _indexesLUT.ContainsKey(player.ID))
			{
				return _indexesLUT[player.ID];
			}
			return -1;
		}


		/// <summary>
		/// Sanitizes the indexing incase a player join while masterclient was changed and missed it.
		/// </summary>
		void SanitizeIndexing(bool forceIndexing = false)
		{
			if (!forceIndexing && !PhotonNetwork.isMasterClient)
			{
				return;
			}

			if (PhotonNetwork.room==null)
			{
				return;
			}

			// attempt to access index props.
			Dictionary<int,int> _indexesLUT_local = new Dictionary<int, int>();
			if(PhotonNetwork.room.CustomProperties.TryGetValue(PlayerRoomIndexing.RoomPlayerIndexedProp, out _indexes))
			{
				_indexesLUT_local = _indexes as Dictionary<int,int>;
			}

			// check if we need to assign
			if (_indexesLUT_local.Count != PhotonNetwork.room.PlayerCount)
			{
			 	foreach(PhotonPlayer _p in	PhotonNetwork.playerList)
				{
					if (!_indexesLUT_local.ContainsKey(_p.ID))
					{
						//	Debug.Log("Sanitizing Index for "+_p);
						AssignIndex(_p);
					}
				}

			}

		}

		/// <summary>
		/// Internal call Refresh the cached data and call the OnRoomIndexingChanged delegate.
		/// </summary>
		void RefreshData()
		{
			if (PhotonNetwork.room!=null)
			{
				_playerIds = new int[PhotonNetwork.room.MaxPlayers];
				if (PhotonNetwork.room.CustomProperties.TryGetValue(PlayerRoomIndexing.RoomPlayerIndexedProp, out _indexes))
				{
					_indexesLUT = _indexes as Dictionary<int,int>;

					foreach(KeyValuePair<int,int> _entry in _indexesLUT)
					{
						//Debug.Log("Entry; "+_entry.Key+":"+_entry.Value);
						_p = PhotonPlayer.Find(_entry.Key);
						_playerIds[_entry.Value] = _p.ID;
					}
				}
			}else{
				_playerIds = new int[0];
			}


			if (OnRoomIndexingChanged!=null) OnRoomIndexingChanged();
		}


		void AssignIndex(PhotonPlayer player)
		{
			if (PhotonNetwork.room.CustomProperties.TryGetValue(PlayerRoomIndexing.RoomPlayerIndexedProp, out _indexes))
			{
				_indexesLUT = _indexes as Dictionary<int,int>;

			}else{
				_indexesLUT = new Dictionary<int, int>();
			}

			List<bool> _indexesPool = new List<bool>( new bool[PhotonNetwork.room.MaxPlayers] );
			foreach(KeyValuePair<int,int> _entry in _indexesLUT)
			{
				_indexesPool[_entry.Value] = true;
			}

			_indexesLUT[player.ID] = Mathf.Max (0,_indexesPool.IndexOf(false));

			PhotonNetwork.room.SetCustomProperties(new Hashtable() {{PlayerRoomIndexing.RoomPlayerIndexedProp, _indexesLUT}});

			RefreshData();
		}


		void UnAssignIndex(PhotonPlayer player)
		{
			if (PhotonNetwork.room.CustomProperties.TryGetValue(PlayerRoomIndexing.RoomPlayerIndexedProp, out _indexes))
			{
				_indexesLUT = _indexes as Dictionary<int,int>;

				_indexesLUT.Remove(player.ID);
				PhotonNetwork.room.SetCustomProperties(new Hashtable() {{PlayerRoomIndexing.RoomPlayerIndexedProp, _indexesLUT}});
			}else{

			}

			RefreshData();
		}

	}

	/// <summary>Extension used for PlayerRoomIndexing and PhotonPlayer class.</summary>
	public static class PlayerRoomIndexingExtensions
	{
		/// <summary>Extension for PhotonPlayer class to wrap up access to the player's custom property.</summary>
		/// <returns>persistent index in room. -1 for no indexing</returns>
		public static int GetRoomIndex(this PhotonPlayer player)
		{
			if (PlayerRoomIndexing.instance == null)
			{
				Debug.LogError("Missing PlayerRoomIndexing Component in Scene");
				return -1;
			}
			return PlayerRoomIndexing.instance.GetRoomIndex(player);
		}

	}
}