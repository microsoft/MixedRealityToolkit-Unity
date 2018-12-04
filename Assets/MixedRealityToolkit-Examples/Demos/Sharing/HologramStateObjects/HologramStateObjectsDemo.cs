using Photon.Pun;
using Photon.Realtime;
using Pixie.AppSystems;
using Pixie.AppSystems.StateObjects;
using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class HologramStateObjectsDemo : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public GameObject startupButtons;
        [SerializeField]
        public GameObject demoContent;
        [SerializeField]
        private InputField flushInterval;
        [SerializeField]
        private Text stateText;
        [SerializeField]
        private Text headerText;
        [SerializeField]
        private Text connectionStatusText;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        private float timeLastFlushed = 0;
        private IAppStateReadWrite appState;
        private IStateView stateView;

        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);
            // Find our app state and state view
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<IStateView>(out stateView);

            // The app state is a sharing app object
            // These must be gathered up and initialized with the app role
            List<ISharingAppObject> sharingAppObjects = new List<ISharingAppObject>();
            ComponentFinder.FindAllInScenes<ISharingAppObject>(sharingAppObjects);

            while (!connected)
                yield return null;

            foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
            {
                sharingAppObject.AppRole = appRole;
                sharingAppObject.OnAppInitialize();
            }

            // Wait until we've joined a room
            while (!joinedRoom)
                yield return null;

            // Once we've joined a room, tell our sharing app objects that we've connected
            foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
                sharingAppObject.OnAppConnect();
        }

        public void OnClickServer()
        {
            appRole = AppRoleEnum.Server;
            startupButtons.SetActive(false);
            demoContent.SetActive(true);
            StartCoroutine(ConnectToService());
        }

        public void OnClickClient()
        {
            appRole = AppRoleEnum.Client;
            startupButtons.SetActive(false);
            demoContent.SetActive(true);
            StartCoroutine(ConnectToService());
        }

        public void OnClickCreateState()
        {
            appState.AddStateOfType(typeof(HologramState));
        }

        public void OnClickFlush()
        {
            appState.Flush();
        }

        void Update()
        {
            headerText.text = "Running as: " + appRole.ToString();

            if (connectedToMaster)
            {
                if (PhotonNetwork.InRoom)
                {
                    connectionStatusText.text = "Connected and in room.";
                }
                else
                {
                    connectionStatusText.text = "Connected, but not in room.";
                }
            }
            else
            {
                connectionStatusText.text = "Not connected to master. State changes will not be broadcast to other users, but you can still experiment with AppState functions.";
            }

            string text = string.Empty;

            if (appState.Initialized)
            {
                // Tell the state view to update
                stateView.OnSessionUpdate(default(SessionState));
                // Check if we need to flush the app state
                if (Time.time > timeLastFlushed + float.Parse(flushInterval.text))
                {
                    timeLastFlushed = Time.time;
                    appState.Flush();
                }

                foreach (HologramState hologramState in appState.GetStates<HologramState>())
                {
                    text += "\n" + hologramState.ToString() + "\n\n";
                }
            }

            stateText.text = text;
        }

        public override void OnConnected()
        {
            connected = true;
        }

        public override void OnConnectedToMaster()
        {
            connectedToMaster = true;
        }

        public override void OnJoinedRoom()
        {
            joinedRoom = true;
        }

        private IEnumerator ConnectToService()
        {
            if (!PhotonNetwork.ConnectUsingSettings())
                Debug.LogError("Couldn't connect using photon settings.");

            switch (appRole)
            {
                case AppRoleEnum.Client:
                    while (!connectedToMaster)
                        yield return new WaitForSeconds(0.5f);

                    if (!PhotonNetwork.JoinRoom(SceneManager.GetActiveScene().name))
                        Debug.LogError("Couldn't connect to room.");

                    break;

                case AppRoleEnum.Server:
                    while (!connectedToMaster)
                        yield return new WaitForSeconds(0.5f);

                    RoomOptions roomOptions = new RoomOptions();
                    TypedLobby typedLobby = new TypedLobby("PixieEDemos", LobbyType.Default);
                    if (!PhotonNetwork.CreateRoom(SceneManager.GetActiveScene().name, roomOptions, typedLobby))
                        Debug.LogError("Couldn't connect to room.");
                    break;
            }

            while (!joinedRoom)
                yield return null;
        }
    }
}