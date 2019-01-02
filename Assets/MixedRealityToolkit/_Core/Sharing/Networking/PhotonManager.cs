using UnityEngine;
using Photon.Pun;
using Pixie.Core;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

namespace Pixie.Networking
{
    public class PhotonManager : MonoBehaviourPunCallbacks, IServerConnection, IClientConnection, ISharingAppObject
    {
        public AppRoleEnum AppRole { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }
        public string FeedbackText { get { return feedbackText; } }

        public bool CanJoinExperience
        {
            get
            {
                switch (status)
                {
                    case ConnectionStatusEnum.Connected:
                        return true;

                    default:
                        return false;
                }
            }
        }

        [SerializeField]
        private string gameVersion = "1.0";
        [SerializeField]
        private string appID;
        [SerializeField]
        private float connectTimeout = 5f;
        [SerializeField]
        private string lobbyName = "PixieLobby";

        private string experienceName;
        private string feedbackText;

        [SerializeField]
        private ConnectionStatusEnum status = ConnectionStatusEnum.NotConnected;

        private List<RoomInfo> availableRooms = new List<RoomInfo>();

        // Server

        ConnectionStatusEnum IServerConnection.Status { get { return status; } }

        void IServerConnection.StartServer()
        {
            switch (status)
            {
                default:
                    Debug.Log("Can't start server in state: " + status);
                    return;

                case ConnectionStatusEnum.NotConnected:
                    break;
            }

            status = ConnectionStatusEnum.ConnectingToService;
            StartCoroutine(ConnectToService());
        }

        void IServerConnection.ForceDisconnect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            status = ConnectionStatusEnum.NotConnected;
        }

        void IServerConnection.CreateExperience(string experienceName)
        {
            switch (status)
            {
                case ConnectionStatusEnum.Connected:
                    break;

                default:
                    Debug.Log("Can't create experience in state " + status);
                    return;
            }

            this.experienceName = experienceName;
            StartCoroutine(TryCreateExperience());
        }

        // Client

        public IEnumerable<string> AvailableExperiences
        {
            get
            {
                foreach (RoomInfo roomInfo in availableRooms)
                    yield return roomInfo.Name;
            }
        }

        ConnectionStatusEnum IClientConnection.Status { get { return status; } }

        void IClientConnection.StartClient()
        {
            switch (status)
            {
                default:
                    Debug.Log("Can't start client in state: " + status);
                    return;

                case ConnectionStatusEnum.NotConnected:
                    break;
            }

            status = ConnectionStatusEnum.ConnectingToService;
            StartCoroutine(ConnectToService());
        }

        void IClientConnection.JoinExperience(string experienceName)
        {
            switch (status)
            {
                case ConnectionStatusEnum.Connected:
                    break;

                default:
                    Debug.Log("Can't create experience in state " + status);
                    return;
            }

            this.experienceName = experienceName;
            StartCoroutine(TryJoinExperience());
        }

        void IClientConnection.ForceDisconnect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            status = ConnectionStatusEnum.NotConnected;
        }

        // Sharing app object

        public void OnAppInitialize() { }

        public void OnAppConnect() { }

        public void OnAppShutDown() { }

        public override void OnCreatedRoom()
        {
            status = ConnectionStatusEnum.ExperienceJoined;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            feedbackText = "Couldn't create experience on server: " + message + ": " + returnCode;
            status = ConnectionStatusEnum.Connected;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            status = ConnectionStatusEnum.NotConnected;
            feedbackText = "Disconnected: " + cause;
        }

        public override void OnConnectedToMaster()
        {
            feedbackText = "Connected to service.";
            status = ConnectionStatusEnum.Connected;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            feedbackText = "Failed to join experience: " + returnCode + ", " + message;
            status = ConnectionStatusEnum.JoiningExperience;
        }

        public override void OnJoinedRoom()
        {
            feedbackText = "Joined experience.";
            status = ConnectionStatusEnum.ExperienceJoined;
        }

        public void OnAppSynchronize()
        {

        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room list was updated");
            foreach (RoomInfo roomInfo in roomList)
            {
                // TODO remove empty / destroyed rooms etc
                availableRooms.Add(roomInfo);
            }
        }

        private IEnumerator ConnectToService()
        {
            status = ConnectionStatusEnum.ConnectingToService;

            // Keep trying to connect unless we force disconnect
            while (status == ConnectionStatusEnum.ConnectingToService)
            {
                feedbackText = "Attempting to connect..." + Time.realtimeSinceStartup;

                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    feedbackText = "Couldn't connect to service, will retry in a moment..." + Time.realtimeSinceStartup;
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    // Wait for a callback from photon
                    status = ConnectionStatusEnum.ConnectingToService;
                    float startTime = Time.realtimeSinceStartup;

                    while (status == ConnectionStatusEnum.ConnectingToService)
                    {
                        feedbackText = "Trying to connect to service... " + Time.realtimeSinceStartup;
                        yield return new WaitForSeconds(1f);

                        // If we time out, start the process over
                        if (Time.realtimeSinceStartup > startTime + connectTimeout)
                        {
                            feedbackText = "Failed to connect to service, will retry..." + Time.realtimeSinceStartup;
                            yield return new WaitForSeconds(0.5f);
                            break;
                        }
                    }

                    switch (status)
                    {
                        case ConnectionStatusEnum.NotConnected:
                            // We've stopped trying to connect
                            yield break;

                        case ConnectionStatusEnum.Connected:
                            // We've connected - time to exit
                            feedbackText = "Connected to service." + Time.realtimeSinceStartup;
                            yield break;

                        case ConnectionStatusEnum.ConnectingToService:
                            // Keep trying
                            break;

                        default:
                            break;
                    }
                }

                yield return null;
            }
        }
        
        private IEnumerator TryCreateExperience()
        {
            // Once we're connected, attempt to create a room
            RoomOptions roomOptions = new RoomOptions();
            // MAKE SURE USER ID IS PUBLISHED
            // Our device source uses this for its lookup tables
            roomOptions.PublishUserId = true;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;

            TypedLobby typedLobby = new TypedLobby(lobbyName, LobbyType.Default);

            while (status != ConnectionStatusEnum.ExperienceJoined)
            {
                feedbackText = "Attempting to create experience...";

                // Try to create the room
                if (!PhotonNetwork.CreateRoom(experienceName, roomOptions, typedLobby))
                {
                    feedbackText = "Couldn't create experience, will retry in a moment...";
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    // Wait for result
                    status = ConnectionStatusEnum.JoiningExperience;
                    float startTime = Time.realtimeSinceStartup;
                    while (status == ConnectionStatusEnum.JoiningExperience)
                    {
                        feedbackText = "Trying to create experience...";
                        yield return new WaitForSeconds(1f);

                        // If we time out, start the process over
                        if (Time.realtimeSinceStartup > startTime + connectTimeout)
                        {
                            feedbackText = "Failed to create experience, will retry...";
                            break;
                        }
                    }
                }

                switch (status)
                {
                    case ConnectionStatusEnum.NotConnected:
                        // We've stopped trying to connect
                        yield break;

                    case ConnectionStatusEnum.ExperienceJoined:
                        // We've joined the experience - time to exit
                        feedbackText = "Joined experience.";
                        yield break;

                    case ConnectionStatusEnum.JoiningExperience:
                        // Keep trying
                        break;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator TryJoinExperience()
        {
            status = ConnectionStatusEnum.JoiningExperience;

            while (status != ConnectionStatusEnum.ExperienceJoined)
            {
                feedbackText = "Attempting to join experience...";

                // Try to create the room
                if (!PhotonNetwork.JoinRoom(experienceName))
                {
                    feedbackText = "Immediate failure, Couldn't join experience, will retry in a moment...";
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    // Wait for result
                    status = ConnectionStatusEnum.JoiningExperience;
                    float startTime = Time.realtimeSinceStartup;
                    while (status == ConnectionStatusEnum.JoiningExperience)
                    {
                        feedbackText = "Trying to join experience...";
                        yield return new WaitForSeconds(1f);

                        // If we time out, start the process over
                        if (Time.realtimeSinceStartup > startTime + connectTimeout)
                        {
                            feedbackText = "Failed to join experience, will retry...";
                            break;
                        }
                    }
                }

                switch (status)
                {
                    case ConnectionStatusEnum.NotConnected:
                        // We've stopped trying to connect
                        yield break;

                    case ConnectionStatusEnum.ExperienceJoined:
                        // We've joined the experience - time to exit
                        feedbackText = "Joined experience.";
                        yield break;

                    case ConnectionStatusEnum.JoiningExperience:
                        // Keep trying
                        break;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void OnDestroy()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
    }
}