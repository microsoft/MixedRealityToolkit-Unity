using Photon.Pun;
using Photon.Realtime;
using Pixie.AppSystems.TimeSync;
using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class TimeSyncDemo : MonoBehaviourPunCallbacks, IPunPrefabPool
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

        [Header("Prefabs")]
        [SerializeField]
        private GameObject timeSyncObjectPrefab;

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        private INetworkTimeController networkTimeController;
        private Dictionary<int, GameObject> timeSyncObjects = new Dictionary<int, GameObject>();

        [SerializeField]
        private Vector3[] spawnPoints;
        private int lastSpawnPointIndex = 0;


        private IEnumerator Start()
        {
            startupButtons.SetActive(true);
            demoContent.SetActive(false);

            // Set the prefab pool to this class so we can instantiate our time sync objects when players join
            PhotonNetwork.PrefabPool = this;

            while (!connected)
                yield return null;
            
            // Now that we've loaded our scene, find our time controller.
            // Both server and client time sync components implement this interface.
            ComponentFinder.FindInScenes<INetworkTimeController>(out networkTimeController);

            // The app state is a sharing app object
            // These must be gathered up and initialized with the app role
            List<ISharingAppObject> sharingAppObjects = new List<ISharingAppObject>();
            ComponentFinder.FindAllInScenes<ISharingAppObject>(sharingAppObjects);

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

            if (networkTimeController != null && networkTimeController.Started)
            {
                text += "Started";
                text += "\nNetworkTime.Time: " + NetworkTime.Time.ToString("00.00");
                text += "\nNetworkTime.TargetTime: " + NetworkTime.TargetTime.ToString("00.00");
                text += "\nNetworkTime.DeltaTime: " + NetworkTime.DeltaTime.ToString("00.00");
                text += "\nNetworkTime.SyncDelta: " + NetworkTime.SyncDelta.ToString("00.00");

                text += "\n\nDEVICES:\n";

                foreach (IDeviceTime deviceTime in networkTimeController.DeviceTimeSources)
                {
                    text += "\nDevice ID: " + deviceTime.DeviceID;
                    text += "\nLatency: " + deviceTime.Latency.ToString("00.0000");
                    text += "\nSynchronized: " + deviceTime.Synchronized;
                    text += "\n";
                }
            }
            else
            {
                text += "(Time controller not started)";
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
                    TypedLobby typedLobby = new TypedLobby("PixieEDemos", LobbyType.Default);
                    if (!PhotonNetwork.CreateRoom(SceneManager.GetActiveScene().name, roomOptions, typedLobby))
                        Debug.LogError("Couldn't connect to room.");
                    break;
            }

            while (!joinedRoom)
                yield return null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!timeSyncObjects.ContainsKey(newPlayer.ActorNumber))
            {
                GameObject device = PhotonNetwork.Instantiate(timeSyncObjectPrefab.name, Vector3.zero, Quaternion.identity);
                IDeviceTime deviceTIme = (IDeviceTime)device.GetComponent(typeof(IDeviceTime));
                deviceTIme.AssignDeviceID((short)newPlayer.ActorNumber);
                timeSyncObjects.Add(newPlayer.ActorNumber, device);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            GameObject timeSyncObject;
            if (timeSyncObjects.TryGetValue(otherPlayer.ActorNumber, out timeSyncObject))
                Destroy(timeSyncObject);
        }

        #region IPunPrefabPool

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if (prefabId != timeSyncObjectPrefab.name)
                throw new System.Exception("You're trying to instantiate something we have no prefab for.");

            GameObject newObject = GameObject.Instantiate(timeSyncObjectPrefab, spawnPoints[lastSpawnPointIndex], rotation);

            lastSpawnPointIndex++;
            if (lastSpawnPointIndex >= spawnPoints.Length)
                lastSpawnPointIndex = 0;

            return newObject;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }

        #endregion
    }
}