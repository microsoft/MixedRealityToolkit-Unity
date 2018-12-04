using Photon.Pun;
using Photon.Realtime;
using Pixie.AppSystems;
using Pixie.AppSystems.StateObjects;
using Pixie.AppSystems.TimeSync;
using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using Pixie.StateControl.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class StateLoggingAndPlaybackDemo : MonoBehaviourPunCallbacks
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
        [SerializeField]
        private Button addHologramButton;
        [Header("Logging")]
        [SerializeField]
        private Button startLoggingButton;
        [SerializeField]
        private Button stopLogingButton;
        [SerializeField]
        private Button startPlaybackButton;
        [SerializeField]
        private Button stopPlaybackButton;
        [SerializeField]
        private Text logText;
        [SerializeField]
        private Text playbackText;
        [SerializeField]
        private string logFilePath;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        private float timeLastFlushed = 0;
        private float timePlaybackStarted;
        private IAppStateReadWrite appState;
        private IStateView stateView;
        private IAppStateLogger logger;
        private IAppStatePlayback playback;

        private Type[] typesToLog = new Type[] { typeof(LoggedTypeAState), typeof (LoggedTypeBState) };

        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);
            // Find our app state and state view
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<IStateView>(out stateView);

            ComponentFinder.FindInScenes<IAppStateLogger>(out logger);
            ComponentFinder.FindInScenes<IAppStatePlayback>(out playback);

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

        public void OnClickCreateStateTypeA()
        {
            appState.AddStateOfType(typeof(LoggedTypeAState));
        }

        public void OnClickCreateStateTypeB()
        {
            appState.AddStateOfType(typeof(LoggedTypeBState));
        }

        public void OnClickStartLogging()
        {
            logger.StartLogging(logFilePath);
        }

        public void OnClickStopLogging()
        {
            logger.StopLogging();
        }

        public void OnClickStartPlayback()
        {
            playback.StartPlayback(logFilePath);
            timePlaybackStarted = NetworkTime.Time;
        }

        public void OnClickStopPlayback()
        {
            playback.StopPlayback();
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

            switch (playback.State)
            {
                case PlaybackStateEnum.Playing:
                    stopPlaybackButton.interactable = true;
                    startPlaybackButton.interactable = false;
                    // Turn off all logging
                    startLoggingButton.interactable = false;
                    stopLogingButton.interactable = false;
                    // Update the playback time based on time elapsed since start
                    playback.SetTime(NetworkTime.Time - timePlaybackStarted);
                    break;

                case PlaybackStateEnum.Stopped:
                    // Enable based on logging state
                    switch (logger.State)
                    {
                        case LogStateEnum.Logging:
                            startLoggingButton.interactable = false;
                            stopLogingButton.interactable = true;

                            startPlaybackButton.interactable = false;
                            stopPlaybackButton.interactable = false;
                            break;

                        case LogStateEnum.Stopped:
                            startLoggingButton.interactable = true;
                            stopLogingButton.interactable = false;

                            startPlaybackButton.interactable = true;
                            stopPlaybackButton.interactable = false;
                            break;

                        case LogStateEnum.Writing:
                            startLoggingButton.interactable = false;
                            stopLogingButton.interactable = false;

                            startPlaybackButton.interactable = false;
                            stopPlaybackButton.interactable = false;
                            break;
                    }
                    break;
            }

            text = "Log state: " + logger.State;
            text += "\nNum queued snapshots: " + logger.NumQueuedSnapshots;
            text += "\nNum logged states: " + logger.NumLoggedStates;

            logText.text = text;

            text = "Playback state: " + playback.State;
            text += "\nCurrent time: " + playback.CurrentTime.ToString("00.00");
            text += "\nTotal time: " + playback.TotalDuration.ToString("00.00");

            playbackText.text = text;
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