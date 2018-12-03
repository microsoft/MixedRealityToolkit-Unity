// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunCockpit.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Pun.Demo.Shared;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// UI based work in progress to test out api and act as foundation when dealing with room, friends and player list
    /// </summary>
    public class PunCockpit : MonoBehaviourPunCallbacks
    {
        public static PunCockpit Instance;
        public static bool Embedded;
        public static string EmbeddedGameTitle = "";

		public bool debug = false;

        public string UserId { get; set; }

        public Text Title;
        public Text StateText; // set in inspector
        public Text UserIdText; // set in inspector

        [Header("Demo Integration")]

        public CanvasGroup MinimalCanvasGroup;
        public CanvasGroup MaximalCanvasGroup;
        public GameObject MinimizeButton;
        public GameObject MinimalUIEmbeddHelp;

        [Header("Connection UI")]
        public GameObject ConnectingLabel;
        public GameObject ConnectionPanel;
        public GameObject AdvancedConnectionPanel;
        public Dropdown ConnectAsDropDown;

        [Header("Common UI")]
        public GameObject InfosPanel;
        public GameObject MinimalUiInfosPanel;

        [Header("Lobby UI")]
        public GameObject LobbyPanel;
        public Selectable JoinLobbyButton;
        public RoomListView RoomListManager;
        public FriendListView FriendListManager;
        public GameObject RoomListMatchMakingForm;

        [Header("Game UI")]
        public GameObject GamePanel;
        public PlayerListView PlayerListManager;
        public PlayerDetailsController PlayerDetailsManager;

        public InputField RoomCustomPropertyInputfield;

		[Header("Photon Settings")]
        /// <summary>
        /// The game version override. This is one way to let the user define the gameversion, and set it properly right after we call connect to override the server settings
        /// Check ConnectAndJoinRandom.cs for another example of gameversion overriding
        /// </summary>
		public string GameVersionOverride = String.Empty;

        /// <summary>
        /// The reset flag for best cloud ServerSettings.
        /// This is one way to let the user define if bestcloud cache should be reseted when connecting.
        /// </summary>
		public bool ResetBestRegionCodeInPreferences = false;

        [Header("Room Options")]
        public int MaxPlayers = 4;
        public int PlayerTtl = 0;
        public int EmptyRoomTtl = 0;
        public string Plugins = "";
        public bool PublishUserId = true;
        public bool IsVisible = true;
        public bool IsOpen = true;
        //public bool CheckUserOnJoin = false;
        public bool CleanupCacheOnLeave = true;
        public bool DeleteNullProperties = false;

        [Header("Room Options UI")]
        public IntInputField PlayerTtlField;
        public IntInputField EmptyRoomTtlField;
        public IntInputField MaxPlayersField;
        public StringInputField PluginsField;
        public BoolInputField PublishUserIdField;
        public BoolInputField IsVisibleField;
        public BoolInputField IsOpenField;
        public BoolInputField CleanupCacheOnLeaveField;
        //	public BoolInputField CheckUserOnJoinField;
        public BoolInputField DeleteNullPropertiesField;

        [Header("Friends Options")]
        public FriendListView.FriendDetail[] FriendsList =
            new FriendListView.FriendDetail[]{
            new FriendListView.FriendDetail("Joe","Joe"),
            new FriendListView.FriendDetail("Jane","Jane"),
            new FriendListView.FriendDetail("Bob","Bob")
            };

		[Header("Modal window")]
		public CanvasGroup ModalWindow;

		public RegionListView RegionListView;
		public Text RegionListLoadingFeedback;

        public void Start()
        {

            Instance = this;

			// doc setup

			DocLinks.Language = DocLinks.Languages.English;
			DocLinks.Product = DocLinks.Products.Pun;
			DocLinks.Version = DocLinks.Versions.V2;

			//

			ModalWindow.gameObject.SetActive (false);

            MaximalCanvasGroup.gameObject.SetActive(true);

            this.UserIdText.text = "";
            this.StateText.text = "";
            this.StateText.gameObject.SetActive(true);
            this.UserIdText.gameObject.SetActive(true);
            this.Title.gameObject.SetActive(true);

            this.ConnectingLabel.SetActive(false);
            this.LobbyPanel.SetActive(false);
            this.GamePanel.SetActive(false);

            if (string.IsNullOrEmpty(UserId))
            {
                UserId = "user" + Environment.TickCount % 99; //made-up username
            }

            PlayerTtlField.SetValue(this.PlayerTtl);
            EmptyRoomTtlField.SetValue(this.EmptyRoomTtl);
            MaxPlayersField.SetValue(this.MaxPlayers);
            PluginsField.SetValue(this.Plugins);
            PublishUserIdField.SetValue(this.PublishUserId);
            IsVisibleField.SetValue(this.IsVisible);
            IsOpenField.SetValue(this.IsOpen);
            CleanupCacheOnLeaveField.SetValue(this.CleanupCacheOnLeave);
            //CheckUserOnJoinField.SetValue (this.CheckUserOnJoin);
            DeleteNullPropertiesField.SetValue(this.DeleteNullProperties);



            // prefill dropdown selection of users
            ConnectAsDropDown.ClearOptions();
            ConnectAsDropDown.AddOptions(FriendsList.Select(x => x.NickName).ToList());


			// check the current network status

			if (PhotonNetwork.IsConnected)
			{
				if (PhotonNetwork.Server == ServerConnection.GameServer)
				{
					this.OnJoinedRoom ();

				}
				else if (PhotonNetwork.Server == ServerConnection.MasterServer || PhotonNetwork.Server == ServerConnection.NameServer)
				{
			
					if (PhotonNetwork.InLobby)
					{
						this.OnJoinedLobby ();
					}
					else
					{
						this.OnConnectedToMaster ();
					}

				}
			}else
			{
	            this.SwitchToSimpleConnection();

	            if (!Embedded)
	            {
	                MinimizeButton.SetActive(false);
	                SwitchtoMaximalPanel();
	            }
	            else
	            {
	                this.Title.text = EmbeddedGameTitle;
	                SwitchtoMinimalPanel();
	            }
			}
        }

        public void SwitchtoMinimalPanel()
        {
            MinimalCanvasGroup.gameObject.SetActive(true);
            MaximalCanvasGroup.alpha = 0f;
            MaximalCanvasGroup.blocksRaycasts = false;
            MaximalCanvasGroup.interactable = false;
        }

        public void SwitchtoMaximalPanel()
        {
            MinimalUIEmbeddHelp.SetActive(false);
            MinimalCanvasGroup.gameObject.SetActive(false);

            MaximalCanvasGroup.alpha = 1f;
            MaximalCanvasGroup.blocksRaycasts = true;
            MaximalCanvasGroup.interactable = true;
        }

        public void SwitchToAdvancedConnection()
        {
            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(true);
        }

        public void SwitchToSimpleConnection()
        {
            this.ConnectionPanel.gameObject.SetActive(true);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);
        }

        public void ToggleInfosInMinimalPanel()
        {
            MinimalUiInfosPanel.SetActive(!MinimalUiInfosPanel.activeSelf);
        }

        public void RequestInfosPanel(GameObject Parent)
        {
            if (Parent != null)
            {
                InfosPanel.transform.SetParent(Parent.transform, false);
            }
        }

        public void OnUserIdSubmited(string userId)
        {
            this.UserId = userId;
            this.Connect();
        }

        public void SetPlayerTtlRoomOption(int value)
        {
            this.PlayerTtl = value;
			if (debug)	Debug.Log("PunCockpit:PlayerTtl = " + this.PlayerTtl);
        }

        public void SetEmptyRoomTtlRoomOption(int value)
        {
            this.EmptyRoomTtl = value;
			if (debug)	Debug.Log("PunCockpit:EmptyRoomTtl = " + this.EmptyRoomTtl);
        }

        public void SetMaxPlayersRoomOption(int value)
        {
            this.MaxPlayers = value;
			if (debug)	Debug.Log("PunCockpit:MaxPlayers = " + this.MaxPlayers);
        }

        public void SetPluginsRoomOption(string value)
        {
            this.Plugins = value;
			if (debug)	Debug.Log("PunCockpit:Plugins = " + this.Plugins);
        }

        public void SetPublishUserId(bool value)
        {
            this.PublishUserId = value;
			if (debug)	Debug.Log("PunCockpit:PublishUserId = " + this.PublishUserId);
        }

        public void SetIsVisible(bool value)
        {
            this.IsVisible = value;
			if (debug)	Debug.Log("PunCockpit:IsVisible = " + this.IsVisible);
        }

        public void SetIsOpen(bool value)
        {
            this.IsOpen = value;
			if (debug)	Debug.Log("PunCockpit:IsOpen = " + this.IsOpen);
        }

        //	public void SetCheckUserOnJoin(bool value)
        //	{
        //		this.CheckUserOnJoin = value;
        //		Debug.Log ("CheckUserOnJoin = " + this.CheckUserOnJoin);
        //	}

		public void SetResetBestRegionCodeInPreferences(bool value)
		{
			this.ResetBestRegionCodeInPreferences = value;
			if (debug)	Debug.Log("PunCockpit:ResetBestRegionCodeInPreferences = " + this.ResetBestRegionCodeInPreferences);
		}

        public void SetCleanupCacheOnLeave(bool value)
        {
            this.CleanupCacheOnLeave = value;
			if (debug)	Debug.Log("PunCockpit:CleanupCacheOnLeave = " + this.CleanupCacheOnLeave);
        }

        public void SetDeleteNullProperties(bool value)
        {
            this.DeleteNullProperties = value;
			if (debug)	Debug.Log("PunCockpit:DeleteNullProperties = " + this.DeleteNullProperties);
        }

		LoadBalancingClient _lbc;
		bool _regionPingProcessActive;
		List<Region> RegionsList;

		/// <summary>
		/// in progress, not fully working
		/// </summary>
		public void PingRegions()
		{
			ModalWindow.gameObject.SetActive (true);

			RegionListLoadingFeedback.text = "Connecting to NameServer...";
			_regionPingProcessActive = true;
			if (debug)	Debug.Log("PunCockpit:PingRegions:ConnectToNameServer");

			_lbc = new LoadBalancingClient (PhotonNetwork.NetworkingClient.ExpectedProtocol);
			_lbc.AddCallbackTarget(this);


			_lbc.StateChanged += OnStateChanged;

			_lbc.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			_lbc.ConnectToNameServer ();

		}

		void Update()
		{
			if (_lbc!=null) _lbc.Service();

			if (RegionsList !=null)
			{
				if (this.ModalWindow.gameObject.activeInHierarchy) {

					if (PunCockpit.Instance.debug)	Debug.Log("PunCockpit:OnRegionsPinged");

					this.RegionListView.OnRegionListUpdate (RegionsList);
				}

				_lbc = null;

				RegionListLoadingFeedback.text = string.Empty;

				RegionsList = null;
			}
		}


		void OnStateChanged(ClientState previousState, ClientState state)
		{
			if (state == ClientState.ConnectedToNameServer) {
				_lbc.StateChanged -= this.OnStateChanged;

				if (debug)	Debug.Log("PunCockpit:OnStateChanged: ClientState.ConnectedToNameServer. Waiting for OnRegionListReceived callback.");

				RegionListLoadingFeedback.text = "Waiting for application Region List...";
			}
		}

        public override void OnRegionListReceived(RegionHandler regionHandler)
		{
			if (PunCockpit.Instance.debug)
				Debug.Log ("PunCockpit:OnRegionListReceived: " + regionHandler);

			if (_regionPingProcessActive)
			{
				RegionListLoadingFeedback.text = "Pinging Regions...";
				_regionPingProcessActive = false;
				regionHandler.PingMinimumOfRegions (OnRegionsPinged, null);
			}
        }
        

		private void OnRegionsPinged(RegionHandler regionHandler)
		{
				RegionsList = regionHandler.EnabledRegions.OrderBy(x=>x.Ping).ToList();
				// will check this on Update() to get back to the main thread.

		}

		public void CloseRegionListView()
		{

			RegionsList = null;

			if (_lbc != null) {
				_lbc.Disconnect ();
				_lbc = null;
			}

			_regionPingProcessActive = false;

			this.RegionListView.ResetList ();
			this.ModalWindow.gameObject.SetActive (false);
		}

		public void LoadLevel(string level)
		{
			if (debug) Debug.Log("PunCockpit:LoadLevel(" +level+")");
			PhotonNetwork.LoadLevel(level);
		}

        public void SetRoomCustomProperty(string value)
        {
			if (debug) Debug.Log("PunCockpit:SetRoomCustomProperty() c0 = " + value);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "C0", value } });
        }

        public void JoinRoom(string roomName)
        {
            this.RoomListManager.ResetList();
            this.LobbyPanel.gameObject.SetActive(false);
            this.ConnectingLabel.SetActive(true);
            PhotonNetwork.JoinRoom(roomName);
        }

        public void CreateRoom()
        {
            this.CreateRoom("");
        }

        public void CreateRoom(string roomName, string lobbyName = "myLobby", LobbyType lobbyType = LobbyType.SqlLobby, string[] expectedUsers = null)
        {
			if (debug) Debug.Log("PunCockpit:CreateRoom roomName:" + roomName + " lobbyName:" + lobbyName + " lobbyType:" + lobbyType + " expectedUsers:" + (expectedUsers == null ? "null" : expectedUsers.ToStringFull()));

            this.RoomListManager.ResetList();
            this.LobbyPanel.gameObject.SetActive(false);
            this.ConnectingLabel.SetActive(true);

            RoomOptions _roomOptions = this.GetRoomOptions();
			if (debug) Debug.Log("PunCockpit:Room options  <" + _roomOptions + ">");

            TypedLobby sqlLobby = new TypedLobby(lobbyName, lobbyType);
            bool _result = PhotonNetwork.CreateRoom(roomName, _roomOptions, sqlLobby, expectedUsers);

			if (debug) Debug.Log("PunCockpit:CreateRoom() -> " + _result);

        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void LeaveRoom()
        {
            PlayerListManager.ResetList();
            this.GamePanel.gameObject.SetActive(false);
            this.ConnectingLabel.SetActive(true);

            PhotonNetwork.LeaveRoom();

        }

        public void Connect()
        {
            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);

            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = this.UserId;

            this.ConnectingLabel.SetActive(true);

            PhotonNetwork.ConnectUsingSettings();
			//if (GameVersionOverride != string.Empty) {
		//		PhotonNetwork.GameVersion = "28"; // GameVersionOverride;
		//	}
        }

        public void ReConnect()
        {
            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);

            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = this.UserId;

            this.ConnectingLabel.SetActive(true);

            PhotonNetwork.Reconnect();
        }

        public void ReconnectAndRejoin()
        {
            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);

            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = this.UserId;

            this.ConnectingLabel.SetActive(true);

            PhotonNetwork.ReconnectAndRejoin();
        }


        public void ConnectToBestCloudServer()
        {

            PhotonNetwork.NetworkingClient.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);

            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = this.UserId;

            this.ConnectingLabel.SetActive(true);

			if (this.ResetBestRegionCodeInPreferences) {
				ServerSettings.ResetBestRegionCodeInPreferences ();
			}

            PhotonNetwork.ConnectToBestCloudServer();
			if (GameVersionOverride != string.Empty) {
				PhotonNetwork.GameVersion = GameVersionOverride;
			}
        }

        public void ConnectToRegion(string region)
        {

			if (debug)  Debug.Log("PunCockpit:ConnectToRegion(" + region + ")");

            PhotonNetwork.NetworkingClient.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

            this.ConnectionPanel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);

            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = this.UserId;

            this.ConnectingLabel.SetActive(true);

            bool _result = PhotonNetwork.ConnectToRegion(region);

			if (GameVersionOverride != string.Empty) {
				PhotonNetwork.GameVersion = GameVersionOverride;
			}

			if (debug)  Debug.Log("PunCockpit:ConnectToRegion(" + region + ") ->" + _result);
        }



        public void ConnectOffline()
        {
			if (debug)  Debug.Log("PunCockpit:ConnectOffline()");
            PhotonNetwork.OfflineMode = true;
        }

        public void JoinLobby()
        {
			if (debug)  Debug.Log("PunCockpit:JoinLobby()");
			bool _result =  PhotonNetwork.JoinLobby();

			if (!_result) {
				Debug.LogError ("PunCockpit: Could not joinLobby");
			}

        }

        public void Disconnect()
        {
			if (debug)  Debug.Log("PunCockpit:Disconnect()");
            PhotonNetwork.Disconnect();
        }


        public void OpenDashboard()
        {
            Application.OpenURL("https://dashboard.photonengine.com");
        }


        #region CONNECT UI
        public void OnDropdownConnectAs(int dropdownIndex)
        {
			if (debug)  Debug.Log("PunCockpit:OnDropdownConnectAs(" + dropdownIndex + ")");

            this.UserId = this.FriendsList[dropdownIndex].UserId;
            PlayerPrefs.SetString(UserIdUiForm.UserIdPlayerPref, this.UserId);

            StartCoroutine(OnDropdownConnectAs_CB());
        }

        IEnumerator OnDropdownConnectAs_CB()
        {
            // wait for the dropdown to animate.
            yield return new WaitForSeconds(0.2f);

            this.Connect();
        }

        #endregion
        #region IN LOBBY UI

        public void OnLobbyToolsViewTabChanged(string tabId)
        {
            //	Debug.Log("PunCockpit:OnLobbyToolsViewTabChanged("+tabId+")");
        }


        #endregion

        #region IN ROOM UI 

        public void OnSelectPlayer()
        {

        }

        #endregion

        #region PUN CallBacks

        public override void OnConnected()
        {
			if (debug) Debug.Log("PunCockpit:OnConnected()");

            this.ConnectingLabel.SetActive(false);

            this.UserIdText.text = "UserId:" + this.UserId + " Nickname:" + PhotonNetwork.NickName;
        }

		public override void OnDisconnected(DisconnectCause cause)
        {
			if (debug) Debug.Log("PunCockpit:OnDisconnected("+cause+")");

            this.ConnectingLabel.SetActive(false);
            this.UserIdText.text = string.Empty;
            this.StateText.text = string.Empty;

            this.GamePanel.gameObject.SetActive(false);
            this.LobbyPanel.gameObject.SetActive(false);
            this.ConnectionPanel.gameObject.SetActive(true);

        }

        public override void OnConnectedToMaster()
        {
			if (debug)  Debug.Log("PunCockpit:OnConnectedToMaster()");


            this.StateText.text = "Connected to Master" + (PhotonNetwork.OfflineMode ? " <Color=Red><b>Offline</b></color>" : "");

            this.SetUpLobbyGenericUI();
        }

        public override void OnJoinedLobby()
        {
            this.StateText.text = "Connected to Lobby";

			if (debug)  Debug.Log("PunCockpit:OnJoinedLobby()");
            this.SetUpLobbyGenericUI();
        }

        void SetUpLobbyGenericUI()
        {
            this.ConnectingLabel.gameObject.SetActive(false);
            this.AdvancedConnectionPanel.gameObject.SetActive(false);
            this.LobbyPanel.gameObject.SetActive(true);
            this.RoomListManager.OnJoinedLobbyCallBack();
            this.FriendListManager.SetFriendDetails(this.FriendsList);

            JoinLobbyButton.interactable = !PhotonNetwork.InLobby && !PhotonNetwork.OfflineMode;


            RoomListManager.gameObject.SetActive(!PhotonNetwork.OfflineMode);
            FriendListManager.gameObject.SetActive(!PhotonNetwork.OfflineMode);

            RoomListMatchMakingForm.SetActive(!PhotonNetwork.InLobby);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
			if (debug) Debug.Log("PunCockpit:OnRoomPropertiesUpdate() " + propertiesThatChanged.ToStringFull());

            if (propertiesThatChanged.ContainsKey("C0"))
            {
                RoomCustomPropertyInputfield.text = propertiesThatChanged["C0"].ToString();
            }
        }

        public override void OnLeftLobby()
        {
			if (debug) Debug.Log("PunCockpit:OnLeftLobby()");

            this.RoomListManager.ResetList();
            this.LobbyPanel.gameObject.SetActive(false);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
			if (debug)  Debug.Log("PunCockpit:OnCreateRoomFailed(" + returnCode + "," + message + ")");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
			if (debug)  Debug.Log("PunCockpit:OnJoinRandomFailed(" + returnCode + "," + message + ")");
        }

        public override void OnJoinedRoom()
        {

            this.StateText.text = "Connected to GameServer " + (PhotonNetwork.OfflineMode ? " <Color=Red><b>Offline</b></color>" : "");


			if (debug)  Debug.Log("PunCockpit:OnJoinedRoom()");

            this.ConnectingLabel.gameObject.SetActive(false);

            this.PlayerListManager.ResetList();

            this.GamePanel.gameObject.SetActive(true);

            this.PlayerDetailsManager.SetPlayerTarget(PhotonNetwork.LocalPlayer);

        }

        public override void OnLeftRoom()
        {
			if (debug)  Debug.Log("PunCockpit:OnLeftRoom()");
            this.GamePanel.gameObject.SetActive(false);

			if (PhotonNetwork.OfflineMode)
			{
				this.ConnectingLabel.gameObject.SetActive(false);
				this.ConnectionPanel.gameObject.SetActive (true);
			}
        }

        #endregion


        RoomOptions GetRoomOptions()
        {
            RoomOptions _roomOptions = new RoomOptions();

            _roomOptions.MaxPlayers = (byte)this.MaxPlayers;

            _roomOptions.IsOpen = this.IsOpen;

            _roomOptions.IsVisible = this.IsVisible;

            _roomOptions.EmptyRoomTtl = this.EmptyRoomTtl;

            _roomOptions.PlayerTtl = this.PlayerTtl;

            _roomOptions.PublishUserId = this.PublishUserId;

            _roomOptions.CleanupCacheOnLeave = this.CleanupCacheOnLeave;
            _roomOptions.DeleteNullProperties = this.DeleteNullProperties;

            _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", "Hello" } };
            _roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" };


            return _roomOptions;
        }
    }
}