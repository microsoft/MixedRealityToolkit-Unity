using Photon.Pun;
using Photon.Realtime;
using Pixie.AnchorControl;
using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using Pixie.Initialization;
using Pixie.StateControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class UserSpaceAlignmentDemo : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public GameObject startupButtons;
        [SerializeField]
        public GameObject demoContent;
        [SerializeField]
        private Text headerText;
        [SerializeField]
        private Text connectionStatusText;
        [SerializeField]
        private Camera observerCamera;
        [Header("Definitions")]
        [SerializeField]
        UserDefinition[] userDefinitions;
        [Header("GUI elements")]
        [SerializeField]
        private UserDefinitionControl[] userDefinitionControls;
        [SerializeField]
        private DeviceDefinitionControl[] deviceDefinitionControls;
        [SerializeField]
        private LayoutGroup userLayoutGroup;
        [SerializeField]
        private LayoutGroup deviceLayoutGroup;
        [Header("Colors")]
        [SerializeField]
        private Color bgColorEmpty;
        [SerializeField]
        private Color bgColorFilled;
        [SerializeField]
        private Color bgColorIgnored;
        [SerializeField]
        private Color bgColorConnected;
        [SerializeField]
        private Color bgColorDisconnected;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        private IAppStateReadWrite appState;
        private IUserManager userManager;
        private IDeviceAssigner deviceAssigner;

        private IAnchorDefinitions anchorDefinitions;
        private IAnchorMatrixSource anchorMatrixSource;
        private IAnchorSynchronizerClient anchorSyncClient;
        private IAnchorSynchronizerServer anchorSyncServer;

        private short deviceWaitingToBeAssigned = -1;

        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);
            // Find our app state
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<IUserManager>(out userManager);
            ComponentFinder.FindInScenes<IDeviceAssigner>(out deviceAssigner);

            // Find all of our anchor-related stuff
            ComponentFinder.FindInScenes<IAnchorDefinitions>(out anchorDefinitions);
            ComponentFinder.FindInScenes<IAnchorMatrixSource>(out anchorMatrixSource);
            ComponentFinder.FindInScenes<IAnchorSynchronizerClient>(out anchorSyncClient);
            ComponentFinder.FindInScenes<IAnchorSynchronizerServer>(out anchorSyncServer);
            
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

            switch (appRole)
            {
                case AppRoleEnum.Server:
                    // If we're the server, generate our user slots based on our user definitions
                    userManager.GenerateUserStates(userDefinitions);
                    appState.Flush();
                    break;

                default:
                    // Otherwise, wait until our user slots are defined by the server
                    while (!userManager.UsersDefined)
                        yield return null;

                    break;

            }
            
            // Start our anchor sync service
            anchorSyncServer.CreateAnchorStates(anchorDefinitions, appState);
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

            UpdateUsersAndDevicesGUI();

            if (appState.Initialized)
            {
                switch (appRole)
                {
                    case AppRoleEnum.Client:
                        foreach (UserSlot user in appState.GetStates<UserSlot>())
                            anchorSyncClient.UpdateUserAnchorStates(user.UserID, anchorMatrixSource, appState);
                        break;

                    default:
                        if (anchorSyncServer.State == AnchorSyncStateEnum.Synchronizing)
                            anchorSyncServer.UpdateAlignmentStates(appState);
                        break;
                }

                appState.Flush();
            }
        }

        private void UpdateUsersAndDevicesGUI()
        {
            // Note: this UI is for demo purposes only. Not intended to be used in production.
            
            if (!userManager.UsersDefined)
            {
                foreach (UserDefinitionControl udc in userDefinitionControls)
                    udc.gameObject.SetActive(false);

                foreach (DeviceDefinitionControl ddc in deviceDefinitionControls)
                    ddc.gameObject.SetActive(false);

                return;
            }

            // Get all of our device states up front since we'll be referring to them frequently
            List<UserDeviceState> deviceStates = new List<UserDeviceState>(appState.GetStates<UserDeviceState>());
            Dictionary<short, List<Transform>> userDeviceConnectors = new Dictionary<short, List<Transform>>();

            // Users
            int numUserSlots = appState.GetNumStates<UserSlot>();

            if (numUserSlots >= userDefinitionControls.Length)
                throw new System.Exception("You've defined more users than this demo can display.");

            int udcIndex = 0;
            foreach (UserSlot userSlot in appState.GetStates<UserSlot>())
            {
                UserDefinitionControl udc = userDefinitionControls[udcIndex];

                if (udcIndex >= numUserSlots)
                {
                    udc.gameObject.SetActive(false);
                    continue;
                }

                if (userManager.LocalUserAssigned)
                {
                    IUserObject userObject = userManager.LocalUserObject;
                    if (userObject.UserID == userSlot.UserID)
                    {
                        udc.UserID.text = "UserID: " + userSlot.UserID + " (Local user)";
                    }
                    else
                    {
                        udc.UserID.text = "UserID: " + userSlot.UserID;
                    }
                }
                else
                {
                    udc.UserID.text = "UserID: " + userSlot.UserID;
                }

                udc.gameObject.SetActive(true);
                switch (userSlot.FillState)
                {
                    case UserSlot.FillStateEnum.Empty:
                        udc.BackgroundImage.color = bgColorEmpty;
                        break;

                    case UserSlot.FillStateEnum.Filled:
                        udc.BackgroundImage.color = bgColorFilled;
                        break;

                    case UserSlot.FillStateEnum.Ignore:
                        udc.BackgroundImage.color = bgColorIgnored;
                        break;
                }

                udc.FillStateText.text = userSlot.FillState.ToString();
                udc.RoleButtonText.text = userSlot.UserRole.ToString();
                udc.TeamButtonText.text = userSlot.UserTeam.ToString();
                                
                int numDeviceRoles = userSlot.DeviceRoles.Length;

                if (numDeviceRoles >= udc.AssignedDevices.Length)
                    throw new System.Exception("You've assigned more devices than this demo can display.");

                List<Transform> connectors = new List<Transform>();
                userDeviceConnectors.Add(userSlot.UserID, connectors);

                byte deviceRoleIndex = 0;
                foreach (AssignedDeviceControl adc in udc.AssignedDevices)
                {
                    if (deviceRoleIndex >= numDeviceRoles)
                    {
                        adc.gameObject.SetActive(false);
                        continue;
                    }

                    adc.gameObject.SetActive(true);
                    adc.TypeButtonText.text = userSlot.DeviceRoles[deviceRoleIndex].ToString();

                    // Store this so devices below can visually connect
                    connectors.Add(adc.Connector);

                    // Are we waiting to assign a device? Has this index been assigned?
                    short assignedDevice = -1;
                    foreach (UserDeviceState deviceState in deviceStates)
                    {
                        if (deviceState.UserID != userSlot.UserID)
                            continue;

                        if (deviceState.IsAssigned && deviceState.DeviceRoleIndex == deviceRoleIndex)
                        {
                            assignedDevice = deviceState.DeviceID;
                            break;
                        }
                    }

                    if (assignedDevice < 0)
                    {
                        adc.DeviceButtonText.text = "(None)";
                        if (deviceWaitingToBeAssigned < 0)
                        {
                            adc.DeviceButton.interactable = false;
                        }
                        else
                        {
                            adc.DeviceButton.interactable = true;
                            adc.DeviceButtonText.text = "Assign " + deviceWaitingToBeAssigned;

                            if (adc.ClickedAssignButton)
                            {
                                deviceAssigner.TryAssignDevice(deviceWaitingToBeAssigned, userSlot.UserID, userSlot.DeviceRoles[deviceRoleIndex]);
                                deviceWaitingToBeAssigned = -1;
                                adc.Reset();
                            }
                        }

                        adc.RevokeButton.interactable = false;
                    }
                    else
                    {
                        adc.DeviceButtonText.text = assignedDevice.ToString();
                        adc.DeviceButton.interactable = false;
                        adc.RevokeButton.interactable = true;

                        if (adc.ClickedRevokeButton)
                        {
                            deviceAssigner.RevokeAssignment(assignedDevice);
                            deviceWaitingToBeAssigned = -1;
                            adc.Reset();
                        }
                    }

                    deviceRoleIndex++;
                }

                // See if we've clicked any buttons
                udc.FillStateButton.interactable = true;
                if (udc.ClickedFillStateButton)
                {
                    udc.Reset();
                    
                    // Create a new user slot state
                    UserSlot newUserSlot = userSlot;

                    // Cycle through the fill state
                    switch (userSlot.FillState)
                    {
                        case UserSlot.FillStateEnum.Empty:
                            newUserSlot.FillState = UserSlot.FillStateEnum.Filled;
                            break;

                        case UserSlot.FillStateEnum.Filled:
                            newUserSlot.FillState = UserSlot.FillStateEnum.Ignore;
                            break;

                        case UserSlot.FillStateEnum.Ignore:
                            newUserSlot.FillState = UserSlot.FillStateEnum.Empty;
                            break;
                    }

                    appState.SetState<UserSlot>(newUserSlot);
                }

                udcIndex++;
            }


            // Devices
            int numDevices = deviceStates.Count;

            if (numDevices >= deviceDefinitionControls.Length)
                throw new System.Exception("You've defined more devices than this demo can display.");

            int deviceIndex = 0;
            foreach (DeviceDefinitionControl ddc in deviceDefinitionControls)
            {
                if (deviceIndex >= numDevices)
                {
                    ddc.gameObject.SetActive(false);
                    continue;
                }

                ddc.gameObject.SetActive(true);
                UserDeviceState deviceState = deviceStates[deviceIndex];
                ddc.DeviceID.text = "DeviceID: " + deviceState.DeviceID;
                ddc.DeviceTypeButtonText.text = deviceState.DeviceType.ToString();
                ddc.DeviceStatusButtonText.text = deviceState.ConnectionState.ToString();

                if (deviceState.IsAssigned)
                {
                    ddc.ConnectorLine.enabled = true;
                    // Get the user definition associated with the assigned user
                    Transform connector = userDeviceConnectors[deviceState.UserID][deviceState.DeviceRoleIndex];
                    ddc.ConnectorLine.SetPosition(0, connector.position);
                    ddc.ConnectorLine.SetPosition(1, ddc.ConnectorTransform.position);
                    // Set this to false until it's un-assigned
                    ddc.DeviceTypeButton.interactable = false;
                }
                else
                {
                    ddc.ConnectorLine.enabled = false;

                    // If we clicked its button, and we're not already assigning a device ID, assign this ID
                    if (deviceWaitingToBeAssigned < 0)
                    {
                        ddc.DeviceTypeButton.interactable = true;

                        if (ddc.ClickedAssignButton)
                        {
                            deviceWaitingToBeAssigned = deviceState.DeviceID;
                            ddc.Reset();
                        }
                    }
                    else
                    {
                        ddc.DeviceTypeButton.interactable = false;
                    }
                }

                switch (deviceState.ConnectionState)
                {
                    case DeviceConnectionStateEnum.Connected:
                        ddc.BackgroundImage.color = bgColorConnected;
                        break;

                    case DeviceConnectionStateEnum.NotConnected:
                        ddc.BackgroundImage.color = bgColorDisconnected;
                        break;
                }

                deviceIndex++;
            }

            // Hacky workaround for Unity's broken layout group nonsense
            userLayoutGroup.enabled = !userLayoutGroup.enabled;
            deviceLayoutGroup.enabled = !deviceLayoutGroup.enabled;
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
                    // THIS MUST BE SET TO TRUE FOR DEVICE ASSIGNER TO WORK.
                    // (Device assigner will throw an exception letting you know.)
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