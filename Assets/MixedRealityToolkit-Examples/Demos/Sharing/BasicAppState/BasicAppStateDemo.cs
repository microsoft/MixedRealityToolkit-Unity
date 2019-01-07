using Photon.Pun;
using Photon.Realtime;
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
    public class BasicAppStateDemo : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public GameObject startupButtons;
        [SerializeField]
        public GameObject demoContent;
        [SerializeField]
        private Text stateText;
        [SerializeField]
        private Text headerText;
        [SerializeField]
        private Text connectionStatusText;
        [SerializeField]
        private InputField newStateIDInput;
        [SerializeField]
        private InputField setStateIDInput;
        [SerializeField]
        private InputField setStateValueInput;
        [SerializeField]
        private Toggle stateTypeAToggle;
        [SerializeField]
        private Toggle stateTypeBToggle;
        [SerializeField]
        private Toggle stateTypeCToggle;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;
        private IAppStateReadWrite appState;
        
        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);
            // Find our app state
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);

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

            foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
                sharingAppObject.OnAppSynchronize();
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

        public void OnClickSubscriptionToggle()
        {

        }

        public void OnClickAddStateOfTypeA()
        {
            appState.AddStateOfType(typeof(StateTypeA));
        }

        public void OnClickAddStateOfTypeB()
        {
            appState.AddStateOfType(typeof(StateTypeB));
        }

        public void OnClickAddStateOfTypeC()
        {
            appState.AddStateOfType(typeof(StateTypeC));
        }

        public void OnClickSetState()
        {
            foreach (StateTypeA stateReadOnly in appState.GetStates<StateTypeA>())
            {
                StateTypeA state = stateReadOnly;
                state.Value = (byte)Random.Range(0, 255);
                appState.SetState<StateTypeA>(state);
            }

            foreach (StateTypeB stateReadOnly in appState.GetStates<StateTypeB>())
            {
                StateTypeB state = stateReadOnly;
                state.Value = (byte)Random.Range(0, 255);
                appState.SetState<StateTypeB>(state);
            }

            foreach (StateTypeC stateReadOnly in appState.GetStates<StateTypeC>())
            {
                StateTypeC state = stateReadOnly;
                state.Value = (byte)Random.Range(0, 255);
                appState.SetState<StateTypeC>(state);
            }
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
                foreach (BasicState basicState in appState.GetStates<BasicState>())
                {
                    text += "\n" + basicState.ToString() + "\n\n";
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
                    while (!connected)
                        yield return new WaitForSeconds(0.5f);

                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.PublishUserId = true;
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