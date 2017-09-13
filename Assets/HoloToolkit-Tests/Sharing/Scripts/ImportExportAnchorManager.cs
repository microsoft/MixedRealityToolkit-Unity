// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using HoloToolkit.Unity;

#if UNITY_WSA && !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;
#endif

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
            Failed,
            Ready,
            RoomApiInitializing,
            RoomApiInitialized,
            AnchorEstablished,
            // AnchorStore states
            AnchorStore_Initializing,
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
                return currentState == ImportExportState.AnchorEstablished;
            }
        }

        /// <summary>
        /// The Room Id of the current room.
        /// </summary>
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
                if (SharingStage.Instance == null || SharingStage.Instance.SessionUsersTracker == null)
                {
                    return false;
                }

                long localUserId;
                using (User localUser = SharingStage.Instance.Manager.GetLocalUser())
                {
                    localUserId = localUser.GetID();
                }

                for (int i = 0; i < SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count; i++)
                {
                    if (SharingStage.Instance.SessionUsersTracker.CurrentUsers[i].GetID() < localUserId)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Called once the anchor has fully uploaded
        /// </summary>
        public event Action<bool> AnchorUploaded;

#if UNITY_WSA && !UNITY_EDITOR

        /// <summary>
        /// Called when the anchor has been loaded
        /// </summary>
        public event Action AnchorLoaded;

        /// <summary>
        /// WorldAnchorTransferBatch is the primary object in serializing/deserializing anchors.
        /// <remarks>Only available on device.</remarks>
        /// </summary>
        private WorldAnchorTransferBatch sharedAnchorInterface;

        /// <summary>
        /// Keeps track of locally stored anchors.
        /// <remarks>Only available on device.</remarks>
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
        /// Sometimes we'll see a really small anchor blob get generated.
        /// These tend to not work, so we have a minimum trustworthy size.
        /// </summary>
        private const uint MinTrustworthySerializedAnchorDataSize = 100000;

#endif

        /// <summary>
        /// Keeps track of stored anchor data blob.
        /// </summary>
        private byte[] rawAnchorData;

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
        /// Some room ID for indicating which room we are in.
        /// </summary>
        private long roomID = 8675309;

        /// <summary>
        /// Indicates if the room should kept around even after all users leave
        /// </summary>
        public bool KeepRoomAlive;

        /// <summary>
        /// Room name to join
        /// </summary>
        public string RoomName = "DefaultRoom";

        /// <summary>
        /// Debug text for displaying information.
        /// </summary>
        public TextMesh AnchorDebugText;

        /// <summary>
        /// Provides updates when anchor data is uploaded/downloaded.
        /// </summary>
        private RoomManagerAdapter roomManagerListener;

        #region Untiy APIs

        protected override void Awake()
        {
            base.Awake();

#if UNITY_WSA && !UNITY_EDITOR
            // We need to get our local anchor store started up.
            currentState = ImportExportState.AnchorStore_Initializing;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
#else
            currentState = ImportExportState.Ready;
#endif
        }

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }
        }

        private void Update()
        {
            switch (currentState)
            {
                // If the local anchor store is initialized.
                case ImportExportState.Ready:
                    if (sharingServiceReady)
                    {
                        StartCoroutine(InitRoomApi());
                    }
                    break;
                case ImportExportState.RoomApiInitialized:
                    StartAnchorProcess();
                    break;
#if UNITY_WSA && !UNITY_EDITOR
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
#endif
            }
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

            if (roomManagerListener != null)
            {
                roomManagerListener.AnchorsChangedEvent -= RoomManagerCallbacks_AnchorsChanged;
                roomManagerListener.AnchorsDownloadedEvent -= RoomManagerListener_AnchorsDownloaded;
                roomManagerListener.AnchorUploadedEvent -= RoomManagerListener_AnchorUploaded;

                if (roomManager != null)
                {
                    roomManager.RemoveListener(roomManagerListener);
                }

                roomManagerListener.Dispose();
                roomManagerListener = null;
            }

            if (roomManager != null)
            {
                roomManager.Dispose();
                roomManager = null;
            }

            base.OnDestroy();
        }

        #endregion

        #region Event Callbacks

        /// <summary>
        /// Called when the sharing stage connects to a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Events Arguments.</param>
        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("Anchor Manager: Starting...");
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += "\nConnected to Server";
            }

            // Setup the room manager callbacks.
            roomManager = SharingStage.Instance.Manager.GetRoomManager();
            roomManagerListener = new RoomManagerAdapter();

            roomManagerListener.AnchorsChangedEvent += RoomManagerCallbacks_AnchorsChanged;
            roomManagerListener.AnchorsDownloadedEvent += RoomManagerListener_AnchorsDownloaded;
            roomManagerListener.AnchorUploadedEvent += RoomManagerListener_AnchorUploaded;

            roomManager.AddListener(roomManagerListener);

            // We will register for session joined and left to indicate when the sharing service
            // is ready for us to make room related requests.
            SharingStage.Instance.SessionsTracker.CurrentUserJoined += CurrentUserJoinedSession;
            SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
        }

        /// <summary>
        /// Called when anchor upload operations complete.
        /// </summary>
        private void RoomManagerListener_AnchorUploaded(bool successful, XString failureReason)
        {
            if (successful)
            {

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("Anchor Manager: Successfully uploaded anchor");
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += "\nSuccessfully uploaded anchor";
                }

                currentState = ImportExportState.AnchorEstablished;
            }
            else
            {
                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\n Upload failed " + failureReason);
                }

                Debug.LogError("Anchor Manager: Upload failed " + failureReason);
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
        private void RoomManagerListener_AnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
        {
            // If we downloaded anchor data successfully we should import the data.
            if (successful)
            {
                int dataSize = request.GetDataSize();

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("Anchor Manager: Anchor size: {0} bytes.", dataSize.ToString());
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nAnchor size: {0} bytes.", dataSize.ToString());
                }

                rawAnchorData = new byte[dataSize];

                request.GetData(rawAnchorData, dataSize);
                currentState = ImportExportState.DataReady;
            }
            else
            {
                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nAnchor DL failed " + failureReason);
                }

                // If we failed, we can ask for the data again.
                Debug.LogWarning("Anchor Manager: Anchor DL failed " + failureReason);
#if UNITY_WSA && !UNITY_EDITOR
                MakeAnchorDataRequest();
#endif
            }
        }

        /// <summary>
        /// Called when the anchors have changed in the room.
        /// </summary>
        /// <param name="room">The room where the anchors have changed.</param>
        private void RoomManagerCallbacks_AnchorsChanged(Room room)
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("Anchor Manager: Anchors in room {0} changed", room.GetName());
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += string.Format("\nAnchors in room {0} changed", room.GetName());
            }

            // if we're currently in the room where the anchors changed
            if (currentRoom == room)
            {
                ResetState();
            }
        }

        /// <summary>
        /// Called when the user joins a session.
        /// In this case, we are using this event is used to signal that the sharing service is ready to make room-related requests.
        /// </summary>
        /// <param name="session">Session joined.</param>
        private void CurrentUserJoinedSession(Session session)
        {
            if (SharingStage.Instance.Manager.GetLocalUser().IsValid())
            {
                sharingServiceReady = true;
            }
            else
            {
                Debug.LogWarning("Unable to get local user on session joined");
            }
        }

        /// <summary>
        /// Called when the user leaves a session.
        /// This event is used to signal that the sharing service must stop making room-related requests.
        /// </summary>
        /// <param name="session">Session left.</param>
        private void CurrentUserLeftSession(Session session)
        {
            sharingServiceReady = false;

            // Reset the state so that we join a new room when we eventually rejoin a session
            ResetState();
        }

        #endregion

        /// <summary>
        /// Resets the Anchor Manager state.
        /// </summary>
        private void ResetState()
        {
#if UNITY_WSA && !UNITY_EDITOR
            if (anchorStore != null)
            {
                currentState = ImportExportState.Ready;
            }
            else
            {
                currentState = ImportExportState.AnchorStore_Initializing;
            }
#else
            currentState = ImportExportState.Ready;
#endif
        }

        /// <summary>
        /// Initializes the room api.
        /// </summary>
        private IEnumerator InitRoomApi()
        {
            currentState = ImportExportState.RoomApiInitializing;
            // First check if there is a current room
            currentRoom = roomManager.GetCurrentRoom();

            while (currentRoom == null)
            {
                // If we have a room, we'll join the first room we see.
                // If we are the user with the lowest user ID, we will create the room.
                // Otherwise we will wait for the room to be created.
                yield return new WaitForEndOfFrame();
                if (roomManager.GetRoomCount() == 0)
                {
                    if (ShouldLocalUserCreateRoom)
                    {
                        if (SharingStage.Instance.ShowDetailedLogs)
                        {
                            Debug.Log("Anchor Manager: Creating room " + RoomName);
                        }

                        if (AnchorDebugText != null)
                        {
                            AnchorDebugText.text += string.Format("\nCreating room " + RoomName);
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
                                Debug.Log("Anchor Manager: Joining room " + room.GetName().GetString());
                            }

                            if (AnchorDebugText != null)
                            {
                                AnchorDebugText.text += string.Format("\nJoining room " + room.GetName().GetString());
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

                yield return new WaitForEndOfFrame();
            }

            if (currentRoom.GetAnchorCount() == 0)
            {
#if UNITY_WSA && !UNITY_EDITOR
                // If the room has no anchors, request the initial anchor
                currentState = ImportExportState.InitialAnchorRequired;
#else
                currentState = ImportExportState.RoomApiInitialized;
#endif
            }
            else
            {
                // Room already has anchors
                currentState = ImportExportState.RoomApiInitialized;
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("Anchor Manager: In room {0} with ID {1}",
                    roomManager.GetCurrentRoom().GetName().GetString(),
                    roomManager.GetCurrentRoom().GetID().ToString());
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += string.Format("\nIn room {0} with ID {1}",
                    roomManager.GetCurrentRoom().GetName().GetString(),
                    roomManager.GetCurrentRoom().GetID().ToString());
            }

            yield return null;
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
                Debug.LogFormat("Anchor Manager: {0} anchors found.", anchorCount.ToString());
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += string.Format("\n{0} anchors found.", anchorCount.ToString());
            }

#if UNITY_WSA && !UNITY_EDITOR

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
                        Debug.Log("Anchor Manager: Starting room anchor download of " + storedAnchorString);
                    }

                    if (AnchorDebugText != null)
                    {
                        AnchorDebugText.text += string.Format("\nStarting room anchor download of " + storedAnchorString);
                    }

                    // If we cannot find the anchor by name, we will need the full data blob.
                    MakeAnchorDataRequest();
                }
            }

#else

            currentState = ImportExportState.AnchorEstablished;

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += anchorCount > 0 ? "\n" + currentRoom.GetAnchorName(0).ToString() : "\nNo Anchors Found";
            }

#endif
        }

        #region WSA Specific Methods

#if UNITY_WSA && !UNITY_EDITOR

        /// <summary>
        /// Kicks off getting the datablob required to import the shared anchor.
        /// When finished downloading, the RoomManager will raise RoomManagerListener_AnchorsDownloaded.
        /// </summary>
        private void MakeAnchorDataRequest()
        {
            if (roomManager.DownloadAnchor(currentRoom, currentRoom.GetAnchorName(0)))
            {
                currentState = ImportExportState.DataRequested;
            }
            else
            {
                Debug.LogError("Anchor Manager: Couldn't make the download request.");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nCouldn't make the download request.");
                }

                currentState = ImportExportState.Failed;
            }
        }

        /// <summary>
        /// Called when the local anchor store is ready.
        /// </summary>
        /// <param name="store"></param>
        private void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;

            if (!KeepRoomAlive)
            {
                anchorStore.Clear();
            }

            currentState = ImportExportState.Ready;
        }

        /// <summary>
        /// Starts establishing a new anchor.
        /// </summary>
        private void CreateAnchorLocally()
        {
            WorldAnchor anchor = this.EnsureComponent<WorldAnchor>();
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
                    Debug.Log("Anchor Manager: Found anchor, ready to export");
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nFound anchor, ready to export");
                }

                currentState = ImportExportState.ReadyToExportInitialAnchor;
            }
            else
            {
                Debug.LogError("Anchor Manager: Failed to locate local anchor!");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nFailed to locate local anchor!");
                }

                currentState = ImportExportState.Failed;
            }

            self.OnTrackingChanged -= Anchor_OnTrackingChanged_InitialAnchor;
        }

        /// <summary>
        /// Attempts to attach to an anchor by anchorName in the local store.
        /// </summary>
        /// <returns>True if it attached, false if it could not attach</returns>
        private bool AttachToCachedAnchor(string anchorName)
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("Anchor Manager: Looking for cahced anchor {0}...", anchorName);
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += string.Format("\nLooking for cahced anchor {0}...", anchorName);
            }

            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                if (ids[index] == anchorName)
                {
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.LogFormat("Anchor Manager: Attempting to load cached anchor {0}...", anchorName);
                    }

                    if (AnchorDebugText != null)
                    {
                        AnchorDebugText.text += string.Format("\nAttempting to load cached anchor {0}...", anchorName);
                    }

                    WorldAnchor anchor = anchorStore.Load(ids[index], gameObject);

                    if (anchor.isLocated)
                    {
                        AnchorLoadComplete();
                    }
                    else
                    {
                        if (AnchorDebugText != null)
                        {
                            AnchorDebugText.text += "\n"+anchorName;
                        }

                        anchor.OnTrackingChanged += ImportExportAnchorManager_OnTrackingChanged_Attaching;
                        currentState = ImportExportState.AnchorEstablished;
                    }
                    return true;
                }
            }

            // Didn't find the anchor, so we'll download from room.
            return false;
        }

        /// <summary>
        /// Called when tracking changes for a 'cached' anchor.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="located"></param>
        private void ImportExportAnchorManager_OnTrackingChanged_Attaching(WorldAnchor self, bool located)
        {
            if (located)
            {
                AnchorLoadComplete();
            }
            else
            {
                Debug.LogWarning("Anchor Manager: Failed to find local anchor from cache.");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nFailed to find local anchor from cache.");
                }

                MakeAnchorDataRequest();
            }

            self.OnTrackingChanged -= ImportExportAnchorManager_OnTrackingChanged_Attaching;
        }

        /// <summary>
        /// Called when a remote anchor has been deserialized.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="anchorBatch"></param>
        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch anchorBatch)
        {
            if (status == SerializationCompletionReason.Succeeded)
            {
                if (anchorBatch.GetAllIds().Length > 0)
                {
                    string first = anchorBatch.GetAllIds()[0];

                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.Log("Anchor Manager: Successfully imported anchor " + first);
                    }

                    if (AnchorDebugText != null)
                    {
                        AnchorDebugText.text += string.Format("\nSuccessfully imported anchor " + first);
                    }

                    WorldAnchor anchor = anchorBatch.LockObject(first, gameObject);
                    anchorStore.Save(first, anchor);
                }

                AnchorLoadComplete();
            }
            else
            {
                Debug.LogError("Anchor Manager: Import failed");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nImport failed");
                }

                currentState = ImportExportState.DataReady;
            }
        }

        private void AnchorLoadComplete()
        {
            if (AnchorLoaded != null)
            {
                AnchorLoaded();
            }

            currentState = ImportExportState.AnchorEstablished;
        }

        /// <summary>
        /// Exports the currently created anchor.
        /// </summary>
        private void Export()
        {
            WorldAnchor anchor = this.GetComponent<WorldAnchor>();
            string guidString = Guid.NewGuid().ToString();
            exportingAnchorName = guidString;

            // Save the anchor to our local anchor store.
            if (anchor != null && anchorStore.Save(exportingAnchorName, anchor))
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("Anchor Manager: Exporting anchor " + exportingAnchorName);
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nExporting anchor {0}", exportingAnchorName);
                }

                sharedAnchorInterface = new WorldAnchorTransferBatch();
                sharedAnchorInterface.AddWorldAnchor(guidString, anchor);
                WorldAnchorTransferBatch.ExportAsync(sharedAnchorInterface, WriteBuffer, ExportComplete);
            }
            else
            {
                Debug.LogWarning("Anchor Manager: Failed to export anchor, trying again...");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nFailed to export anchor, trying again...");
                }

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
                    Debug.Log("Anchor Manager: Uploading anchor: " + exportingAnchorName);
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nUploading anchor: " + exportingAnchorName);
                }

                roomManager.UploadAnchor(
                    currentRoom,
                    new XString(exportingAnchorName),
                    exportingAnchorBytes.ToArray(),
                    exportingAnchorBytes.Count);
            }
            else
            {
                Debug.LogWarning("Anchor Manager: Failed to upload anchor, trying again...");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nFailed to upload anchor, trying again...");
                }

                currentState = ImportExportState.InitialAnchorRequired;
            }
        }

#endif // UNITY_WSA
        #endregion // WSA Specific Methods
    }
}