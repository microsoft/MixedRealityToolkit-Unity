using Photon.Pun;
using Photon.Realtime;
using Pixie.AppSystems;
using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.StateObjects;
using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class SessionsDemo : MonoBehaviourPunCallbacks
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
        private Text sessionStateText;

        [SerializeField]
        private Button initButton;
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private Button pauseButton;
        [SerializeField]
        private Button nextStageButton;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        private float timeLastFlushed = 0;
        private ISceneLoader sceneLoader;
        private IAppStateReadWrite appState;
        private ISessionManager sessionManager;

        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);
            startButton.interactable = false;
            pauseButton.interactable = false;
            nextStageButton.interactable = false;

            // Find our components
            ComponentFinder.FindInScenes<ISceneLoader>(out sceneLoader);
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<ISessionManager>(out sessionManager);

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

        private IEnumerator InitializeAndUpdateSession()
        {
            // Session manager goes through several stages before running a session

            // First, we must choose an ExperienceMode.
            // Experiences are scriptable objects that define session stages.
            // Often you will want to choose one of several.
            // In this demo we have only one, so we can select it and move on.
            if (sessionManager.State == SessionStateEnum.ChoosingExperience)
                sessionManager.SetExperienceMode(0);

            // If the session has a layout scene defined, it will wait until we've loeaded and prepped the scene.
            if (sessionManager.State == SessionStateEnum.LoadingLayoutScene)
            {
                yield return sceneLoader.LoadSharedScene(sessionManager.ExperienceMode.LayoutSceneName);
                // If the layout scene has state objects defined in it, 
                // we would be expected to find and load those objects into the app state / state view.
                // Since we're not doing that in this demo, we can proceed.
                sessionManager.SetLayoutSceneLoaded();
            }

            startButton.interactable = true;

            // Now we wait until user tells the session to start (in this case with a button press)
            while (sessionManager.State == SessionStateEnum.ReadyToStart)
                yield return null;

            startButton.interactable = false;
            pauseButton.interactable = true;
            nextStageButton.interactable = true;

            // While the session is in progress, we are responsible for its tasks by calling TryUpdateSession.
            // Failing to call this effectively pauses the session.
            while (sessionManager.State == SessionStateEnum.InProgress)
            {
                // If the session does progress, flush the app state
                if (sessionManager.TryUpdateSession())
                    appState.Flush();

                yield return null;
            }

            pauseButton.interactable = false;
            nextStageButton.interactable = false;

            yield break;
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

        public void OnClickInitializeSession()
        {
            switch (sessionManager.State)
            {
                case SessionStateEnum.ChoosingExperience:
                    initButton.interactable = false;
                    StartCoroutine(InitializeAndUpdateSession());
                    break;

                default:
                    throw new System.Exception("Can't start session in state " + sessionManager.State);
            }
        }

        public void OnClickStartSession()
        {
            sessionManager.StartSession();
        }

        public void OnClickForceNextStage()
        {
            sessionManager.ForceCompleteStage();
        }

        public void OnClickPauseSession()
        {
            sessionManager.SetPaused(!sessionManager.Paused);
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
                appState.Flush();

                foreach (Type stateType in appState.ItemStateTypes)
                {
                    text += "\n" + stateType.Name + "s:\n----";
                    foreach (object state in appState.GetStates(stateType))
                    {
                        text += "\n" + state.ToString() + "\n\n";
                    }
                }
            }

            stateText.text = text;

            text = string.Empty;

            switch (appRole)
            {
                case AppRoleEnum.Server:
                    if (sessionManager != null)
                    {
                        text = sessionManager.State.ToString() + "\n";
                        text += "Paused: " + sessionManager.Paused + "\n";
                        if (sessionManager.State == SessionStateEnum.InProgress)
                        {
                            text += "Stage: " + sessionManager.CurrentStage.name + "\n";
                            text += "Progression Type: " + sessionManager.CurrentStage.ProgressionType + "\n";
                            text += "Time Elapsed: " + sessionManager.CurrentStage.TimeElapsed.ToString("00.0");
                            text += "Stage State: " + sessionManager.CurrentStage.State;
                        }
                    }
                    break;

                case AppRoleEnum.Client:
                    text = "(Session manager is only run on server)";
                    break;
            }

            sessionStateText.text = text;
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