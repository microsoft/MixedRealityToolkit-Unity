// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Manages creating anchors and sharing the anchors with other clients.
    /// </summary>
    public class ImportExportAnchorManager : Singleton<ImportExportAnchorManager>
    {
        /// <summary>
        /// Enum to track the progress through establishing a shared coordinate system.
        /// </summary>
        private enum ImportExportState
        {
            // Overall states
            Start,
            Ready,
            Failed,
            // AnchorStore values
            AnchorStore_Initializing,
            AnchorStore_Initialized,
            RoomApiInitialized,
            // Anchor creation values
            InitialAnchorRequired,
            CreatingInitialAnchor,
            ReadyToExportInitialAnchor,
            UploadingInitialAnchor,
            // Anchor values
            DataRequested,
            DataReady,
            Importing
        }

        private ImportExportState currentState = ImportExportState.Start;

        public string StateName
        {
            get
            {
                return currentState.ToString();
            }
        }

        public bool AnchorEstablished
        {
            get
            {
                return currentState == ImportExportState.Ready;
            }
        }

        public long RoomID
        {
            get
            {
                return roomID;
            }

            set
            {
                if (currentRoom == null)
                {
                    roomID = value;
                }
            }
        }

        private static bool ShouldLocalUserCreateRoom
        {
            get
            {
                if (SharingStage.Instance == null)
                {
                    return false;
                }

                if (SharingStage.Instance.SessionUsersTracker != null)
                {
                    long localUserId;
                    using (User localUser = SharingStage.Instance.Manager.GetLocalUser())
                    {
                        localUserId = localUser.GetID();
                    }

                    for (int i = 0; i < SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count; i++)
                    {
                        User user = SharingStage.Instance.SessionUsersTracker.CurrentUsers[i];
                        if (user.GetID() < localUserId)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Called once the anchor has fully uploaded
        /// </summary>
        public event Action<bool> AnchorUploaded;

        /// <summary>
        /// Called when the anchor has been loaded
        /// </summary>
        public event Action AnchorLoaded;

        /// <summary>
        /// Indicates if the room should kept around even after all users leave
        /// </summary>
        public bool KeepRoomAlive;

        /// <summary>
        /// Room name to join
        /// </summary>
        public string RoomName = "DefaultRoom";

        /// <summary>
        /// WorldAnchorTransferBatch is the primary object in serializing/deserializing anchors.
        /// </summary>
        private WorldAnchorTransferBatch sharedAnchorInterface;

        /// <summary>
        /// Keeps track of stored anchor data blob.
        /// </summary>
        private byte[] rawAnchorData;

        /// <summary>
        /// Keeps track of locally stored anchors.
        /// </summary>
        private WorldAnchorStore anchorStore;

        /// <summary>
        /// Keeps track of the name of the anchor we are exporting.
        /// </summary>
        private string exportingAnchorName;

        /// <summary>
        /// The datablob of the anchor.
        /// </summary>
        private List<byte> exportingAnchorBytes = new List<byte>();

        /// <summary>
        /// Keeps track of if the sharing service is ready.
        /// We need the sharing service to be ready so we can
        /// upload and download data for sharing anchors.
        /// </summary>
        private bool sharingServiceReady;

        /// <summary>
        /// The room manager API for the sharing service.
        /// </summary>
        private RoomManager roomManager;

        /// <summary>
        /// Keeps track of the current room we are connected to.  Anchors
        /// are kept in rooms.
        /// </summary>
        private Room currentRoom;

        /// <summary>
        /// Sometimes we'll see a really small anchor blob get generated.
        /// These tend to not work, so we have a minimum trustable size.
        /// </summary>
        private const uint MinTrustworthySerializedAnchorDataSize = 100000;

        /// <summary>
        /// Some room ID for indicating which room we are in.
        /// </summary>
        private long roomID = 8675309;

        /// <summary>
        /// Provides updates when anchor data is uploaded/downloaded.
        /// </summary>
        private RoomManagerAdapter roomManagerCallbacks;

        protected override void Awake()
        {
            base.Awake();

            // We need to get our local anchor store started up.
            currentState = ImportExportState.AnchorStore_Initializing;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        private void Start()
        {
            // SharingStage should be valid at this point.
            SharingStage.Instance.SharingManagerConnected += Connected;
        }

        private void Connected(object sender, EventArgs e)
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("Import Export Manager starting");
            }

            SharingStage.Instance.SharingManagerConnected -= Connected;

            // Setup the room manager callbacks.
            roomManager = SharingStage.Instance.Manager.GetRoomManager();
            roomManagerCallbacks = new RoomManagerAdapter();

            roomManagerCallbacks.AnchorsDownloadedEvent += RoomManagerCallbacks_AnchorsDownloaded;
            roomManagerCallbacks.AnchorUploadedEvent += RoomManagerCallbacks_AnchorUploaded;
            roomManager.AddListener(roomManagerCallbacks);

            // We will register for session joined and left to indicate when the sharing service
            // is ready for us to make room related requests.
            SharingStage.Instance.SessionsTracker.CurrentUserJoined += CurrentUserJoinedSession;
            SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                if (SharingStage.Instance.SessionsTracker != null)
                {
                    SharingStage.Instance.SessionsTracker.CurrentUserJoined -= CurrentUserJoinedSession;
                    SharingStage.Instance.SessionsTracker.CurrentUserLeft -= CurrentUserLeftSession;
                }
            }

            if (roomManagerCallbacks != null)
            {
                roomManagerCallbacks.AnchorsDownloadedEvent -= RoomManagerCallbacks_AnchorsDownloaded;
                roomManagerCallbacks.AnchorUploadedEvent -= RoomManagerCallbacks_AnchorUploaded;

                if (roomManager != null)
                {
                    roomManager.RemoveListener(roomManagerCallbacks);
                }

                roomManagerCallbacks.Dispose();
                roomManagerCallbacks = null;
            }

            if (roomManager != null)
            {
                roomManager.Dispose();
                roomManager = null;
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Called when anchor upload operations complete.
        /// </summary>
        private void RoomManagerCallbacks_AnchorUploaded(bool successful, XString failureReason)
        {
            if (successful)
            {
                currentState = ImportExportState.Ready;
            }
            else
            {
                Debug.LogError("Upload failed " + failureReason);
                currentState = ImportExportState.Failed;
            }

            if (AnchorUploaded != null)
            {
                AnchorUploaded(successful);
            }
        }

        /// <summary>
        /// Called when anchor download operations complete.
        /// </summary>
        private void RoomManagerCallbacks_AnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
        {
            // If we downloaded anchor data successfully we should import the data.
            if (successful)
            {
                int datasize = request.GetDataSize();

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log(datasize.ToString() + " bytes ");
                }

                rawAnchorData = new byte[datasize];

                request.GetData(rawAnchorData, datasize);
                currentState = ImportExportState.DataReady;
            }
            else
            {
                // If we failed, we can ask for the data again.
                Debug.LogWarning("Anchor DL failed " + failureReason);
                MakeAnchorDataRequest();
            }
        }

        /// <summary>
        /// Called when the local anchor store is ready.
        /// </summary>
        /// <param name="store"></param>
        private void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;
            currentState = ImportExportState.AnchorStore_Initialized;
        }

        /// <summary>
        /// Called when the local user joins a session.
        /// In this case, we are using this event is used to signal that the sharing service is ready to make room-related requests.
        /// </summary>
        /// <param name="session">Session joined.</param>
        private void CurrentUserJoinedSession(Session session)
        {
            if (!sharingServiceReady)
            {
                // Wait to wait a few seconds for everything to settle.
                Invoke("MarkSharingServiceReady", 5);
            }
        }

        /// <summary>
        /// Called when the local user leaves a session.
        /// This event is used to signal that the sharing service must stop making room-related requests.
        /// </summary>
        /// <param name="session">Session left.</param>
        private void CurrentUserLeftSession(Session session)
        {
            sharingServiceReady = false;

            // Reset the state so that we join a new room when we eventually rejoin a session
            if (anchorStore != null)
            {
                currentState = ImportExportState.AnchorStore_Initialized;
            }
            else
            {
                currentState = ImportExportState.AnchorStore_Initializing;
            }
        }

        private void MarkSharingServiceReady()
        {
            sharingServiceReady = true;

#if UNITY_EDITOR || UNITY_STANDALONE
            InitRoomApi();
#endif
        }

        /// <summary>
        /// Initializes the room api.
        /// </summary>
        private void InitRoomApi()
        {
            // First check if there is a current room
            currentRoom = roomManager.GetCurrentRoom();
            if (currentRoom == null)
            {
                // If we have a room, we'll join the first room we see.
                // If we are the user with the lowest user ID, we will create the room.
                // Otherwise we will wait for the room to be created.
                if (roomManager.GetRoomCount() == 0)
                {
                    if (ShouldLocalUserCreateRoom)
                    {
                        if (SharingStage.Instance.ShowDetailedLogs)
                        {
                            Debug.Log("Creating room " + RoomName);
                        }

                        // To keep anchors alive even if all users have left the session ...
                        // Pass in true instead of false in CreateRoom.
                        currentRoom = roomManager.CreateRoom(new XString(RoomName), roomID, KeepRoomAlive);
                    }
                }
                else
                {
                    // Look through the existing rooms and join the one that matches the room name provided.
                    int roomCount = roomManager.GetRoomCount();
                    for (int i = 0; i < roomCount; i++)
                    {
                        Room room = roomManager.GetRoom(i);
                        if (room.GetName().GetString().Equals(RoomName, StringComparison.OrdinalIgnoreCase))
                        {
                            currentRoom = room;
                            roomManager.JoinRoom(currentRoom);

                            if (SharingStage.Instance.ShowDetailedLogs)
                            {
                                Debug.Log("Joining room " + room.GetName().GetString());
                            }

                            break;
                        }
                    }

                    if (currentRoom == null)
                    {
                        // Couldn't locate a matching room, just join the first one.
                        currentRoom = roomManager.GetRoom(0);
                        roomManager.JoinRoom(currentRoom);
                        RoomName = currentRoom.GetName().GetString();
                    }

                    currentState = ImportExportState.RoomApiInitialized;
                }
            }

            if (currentRoom != null)
            {
                if (currentRoom.GetAnchorCount() == 0)
                {
                    // If the room has no anchors, request the initial anchor
                    currentState = ImportExportState.InitialAnchorRequired;
                }
                else
                {
                    // Room already has anchors
                    currentState = ImportExportState.RoomApiInitialized;
                }


                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("In room : {0} with ID {1} and state {2}",
                        roomManager.GetCurrentRoom().GetName().GetString(),
                        roomManager.GetCurrentRoom().GetID().ToString(),
                        currentState);
                }
            }
            else
            {
                Debug.LogError("Unable to determine room");
            }
        }

        /// <summary>
        /// Kicks off the process of creating the shared space.
        /// </summary>
        private void StartAnchorProcess()
        {
            // First, are there any anchors in this room?
            int anchorCount = currentRoom.GetAnchorCount();

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log(anchorCount.ToString() + " anchors");
            }

            // If there are anchors, we should attach to the first one.
            if (anchorCount > 0)
            {
                // Extract the name of the anchor.
                XString storedAnchorString = currentRoom.GetAnchorName(0);
                string storedAnchorName = storedAnchorString.GetString();

                // Attempt to attach to the anchor in our local anchor store.
                if (AttachToCachedAnchor(storedAnchorName) == false)
                {
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.Log("Starting room download");
                    }

                    // If we cannot find the anchor by name, we will need the full data blob.
                    MakeAnchorDataRequest();
                }
            }
        }

        /// <summary>
        /// Kicks off getting the datablob required to import the shared anchor.
        /// </summary>
        private void MakeAnchorDataRequest()
        {
            if (roomManager.DownloadAnchor(currentRoom, currentRoom.GetAnchorName(0)))
            {
                currentState = ImportExportState.DataRequested;
            }
            else
            {
                Debug.LogError("Couldn't make the download request.");
                currentState = ImportExportState.Failed;
            }
        }

        private void Update()
        {
            switch (currentState)
            {
                // If the local anchor store is initialized.
                case ImportExportState.AnchorStore_Initialized:
                    if (sharingServiceReady)
                    {
                        InitRoomApi();
                    }
                    break;
                case ImportExportState.RoomApiInitialized:
                    StartAnchorProcess();
                    break;
                case ImportExportState.DataReady:
                    // DataReady is set when the anchor download completes.
                    currentState = ImportExportState.Importing;
                    WorldAnchorTransferBatch.ImportAsync(rawAnchorData, ImportComplete);
                    break;
                case ImportExportState.InitialAnchorRequired:
                    currentState = ImportExportState.CreatingInitialAnchor;
                    CreateAnchorLocally();
                    break;
                case ImportExportState.ReadyToExportInitialAnchor:
                    // We've created an anchor locally and it is ready to export.
                    currentState = ImportExportState.UploadingInitialAnchor;
                    Export();
                    break;
            }
        }

        /// <summary>
        /// Starts establishing a new anchor.
        /// </summary>
        private void CreateAnchorLocally()
        {
            var anchor = GetComponent<WorldAnchor>() ?? gameObject.AddComponent<WorldAnchor>();

            if (anchor.isLocated)
            {
                currentState = ImportExportState.ReadyToExportInitialAnchor;
            }
            else
            {
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged_InitialAnchor;
            }
        }

        /// <summary>
        /// Callback to trigger when an anchor has been 'found'.
        /// </summary>
        private void Anchor_OnTrackingChanged_InitialAnchor(WorldAnchor self, bool located)
        {
            if (located)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("Found anchor, ready to export");
                }

                currentState = ImportExportState.ReadyToExportInitialAnchor;
            }
            else
            {
                Debug.LogError("Failed to locate local anchor!");
                currentState = ImportExportState.Failed;
            }

            self.OnTrackingChanged -= Anchor_OnTrackingChanged_InitialAnchor;
        }

        /// <summary>
        /// Attempts to attach to  an anchor by anchorName in the local store..
        /// </summary>
        /// <returns>True if it attached, false if it could not attach</returns>
        private bool AttachToCachedAnchor(string anchorName)
        {

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("Looking for " + anchorName);
            }

            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                if (ids[index] == anchorName)
                {
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.LogFormat("Attempting to load {0}...", anchorName);
                    }

                    WorldAnchor anchor = anchorStore.Load(ids[index], gameObject);
                    if (anchor.isLocated)
                    {
                        AnchorLoadComplete();
                    }
                    else
                    {
                        anchor.OnTrackingChanged += ImportExportAnchorManager_OnTrackingChanged_Attaching;
                        currentState = ImportExportState.Ready;
                    }
                    return true;
                }
            }

            // Didn't find the anchor.
            return false;
        }

        /// <summary>
        /// Called when tracking changes for a 'cached' anchor.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="located"></param>
        private void ImportExportAnchorManager_OnTrackingChanged_Attaching(WorldAnchor self, bool located)
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("anchor " + located.ToString());
            }

            if (located)
            {
                AnchorLoadComplete();
            }
            else
            {
                Debug.LogWarning("Failed to find local anchor from cache.");
                MakeAnchorDataRequest();
            }

            self.OnTrackingChanged -= ImportExportAnchorManager_OnTrackingChanged_Attaching;
        }

        /// <summary>
        /// Called when a remote anchor has been deserialized
        /// </summary>
        /// <param name="status"></param>
        /// <param name="wat"></param>
        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch wat)
        {
            if (status == SerializationCompletionReason.Succeeded)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("Import complete");
                }

                if (wat.GetAllIds().Length > 0)
                {
                    string first = wat.GetAllIds()[0];

                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.Log("Anchor name: " + first);
                    }

                    WorldAnchor anchor = wat.LockObject(first, gameObject);
                    anchorStore.Save(first, anchor);
                }

                AnchorLoadComplete();
            }
            else
            {
                Debug.LogError("Import failed");
                currentState = ImportExportState.DataReady;
            }
        }

        private void AnchorLoadComplete()
        {
            if (AnchorLoaded != null)
            {
                AnchorLoaded();
            }

            currentState = ImportExportState.Ready;
        }

        /// <summary>
        /// Exports the currently created anchor.
        /// </summary>
        private void Export()
        {
            var anchor = GetComponent<WorldAnchor>();

            string guidString = Guid.NewGuid().ToString();
            exportingAnchorName = guidString;

            // Save the anchor to our local anchor store.
            if (anchor != null && anchorStore.Save(exportingAnchorName, anchor))
            {
                sharedAnchorInterface = new WorldAnchorTransferBatch();
                sharedAnchorInterface.AddWorldAnchor(guidString, anchor);
                WorldAnchorTransferBatch.ExportAsync(sharedAnchorInterface, WriteBuffer, ExportComplete);
            }
            else
            {
                Debug.LogWarning("Failed to export anchor, trying again...");
                currentState = ImportExportState.InitialAnchorRequired;
            }
        }

        /// <summary>
        /// Called by the WorldAnchorTransferBatch as anchor data is available.
        /// </summary>
        /// <param name="data"></param>
        private void WriteBuffer(byte[] data)
        {
            exportingAnchorBytes.AddRange(data);
        }

        /// <summary>
        /// Called by the WorldAnchorTransferBatch when anchor exporting is complete.
        /// </summary>
        /// <param name="status"></param>
        private void ExportComplete(SerializationCompletionReason status)
        {
            if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > MinTrustworthySerializedAnchorDataSize)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("Uploading anchor: " + exportingAnchorName);
                }

                roomManager.UploadAnchor(
                    currentRoom,
                    new XString(exportingAnchorName),
                    exportingAnchorBytes.ToArray(),
                    exportingAnchorBytes.Count);
            }
            else
            {
                Debug.LogWarning("Failed to export anchor, trying again...");
                currentState = ImportExportState.InitialAnchorRequired;
            }
        }
    }
}