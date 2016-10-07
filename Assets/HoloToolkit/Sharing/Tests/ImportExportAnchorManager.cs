// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;

/// <summary>
/// Manages creating anchors and sharing the anchors with other clients.
/// </summary>
public class ImportExportAnchorManager : Singleton<ImportExportAnchorManager>
{
    /// <summary>
    /// Enum to track the progress through establishing a shared coordinate system.
    /// </summary>
    enum ImportExportState
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

    ImportExportState currentState = ImportExportState.Start;

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

    /// <summary>
    /// WorldAnchorTransferBatch is the primary object in serializing/deserializing anchors.
    /// </summary>
    WorldAnchorTransferBatch sharedAnchorInterface;

    /// <summary>
    /// Keeps track of stored anchor data blob.
    /// </summary>
    byte[] rawAnchorData = null;

    /// <summary>
    /// Keeps track of locally stored anchors.
    /// </summary>
    WorldAnchorStore anchorStore = null;

    /// <summary>
    /// Keeps track of the name of the anchor we are exporting.
    /// </summary>
    string exportingAnchorName { get; set; }

    /// <summary>
    /// The datablob of the anchor.
    /// </summary>
    List<byte> exportingAnchorBytes = new List<byte>();

    /// <summary>
    /// Keeps track of if the sharing service is ready.
    /// We need the sharing service to be ready so we can
    /// upload and download data for sharing anchors.
    /// </summary>
    bool sharingServiceReady = false;

    /// <summary>
    /// The room manager API for the sharing service.
    /// </summary>
    RoomManager roomManager;

    /// <summary>
    /// Keeps track of the current room we are connected to.  Anchors
    /// are kept in rooms.
    /// </summary>
    Room currentRoom;

    /// <summary>
    /// Sometimes we'll see a really small anchor blob get generated.
    /// These tend to not work, so we have a minimum trustable size.
    /// </summary>
    const uint minTrustworthySerializedAnchorDataSize = 100000;

    /// <summary>
    /// Some room ID for indicating which room we are in.
    /// </summary>
    const long roomID = 8675309;

    /// <summary>
    /// Provides updates when anchor data is uploaded/downloaded.
    /// </summary>
    RoomManagerAdapter roomManagerCallbacks;

    void Start()
    {
        Debug.Log("Import Export Manager starting");

        currentState = ImportExportState.Ready;

        // We need to get our local anchor store started up.
        currentState = ImportExportState.AnchorStore_Initializing;
        WorldAnchorStore.GetAsync(AnchorStoreReady);

        //Wait for a notification that the sharing manager has been initialized (connected to sever)
        SharingStage.Instance.SharingManagerConnected += SharingManagerConnected;
    }

    void OnDestroy()
    {
        if (roomManagerCallbacks != null)
        {
            roomManagerCallbacks.AnchorsDownloadedEvent -= RoomManagerCallbacks_AnchorsDownloaded;
            roomManagerCallbacks.AnchorUploadedEvent -= RoomManagerCallbacks_AnchorUploaded;

            if (roomManager != null)
            {
                roomManager.RemoveListener(roomManagerCallbacks);
            }
        }
    }

    private void SharingManagerConnected(object sender, EventArgs e)
    {
        // Setup the room manager callbacks.
        roomManager = SharingStage.Instance.Manager.GetRoomManager();
        roomManagerCallbacks = new RoomManagerAdapter();

        roomManagerCallbacks.AnchorsDownloadedEvent += RoomManagerCallbacks_AnchorsDownloaded;
        roomManagerCallbacks.AnchorUploadedEvent += RoomManagerCallbacks_AnchorUploaded;
        roomManager.AddListener(roomManagerCallbacks);

        // We will register for session joined to indicate when the sharing service
        // is ready for us to make room related requests.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
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
            Debug.Log("Upload failed " + failureReason);
            currentState = ImportExportState.Failed;
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
            Debug.Log(datasize + " bytes ");
            rawAnchorData = new byte[datasize];

            request.GetData(rawAnchorData, datasize);
            currentState = ImportExportState.DataReady;
        }
        else
        {
            // If we failed, we can ask for the data again.
            Debug.Log("Anchor DL failed " + failureReason);
            MakeAnchorDataRequest();
        }
    }

    /// <summary>
    /// Called when the local anchor store is ready.
    /// </summary>
    /// <param name="store"></param>
    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        currentState = ImportExportState.AnchorStore_Initialized;
    }

    /// <summary>
    /// Called when a user (including the local user) joins a session.
    /// In this case we are using this event to signal that the sharing service is
    /// ready for us to make room related requests.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        // We don't need to get this event anymore.
        SharingSessionTracker.Instance.SessionJoined -= Instance_SessionJoined;

        // We still wait to wait a few seconds for everything to settle.
        Invoke("MarkSharingServiceReady", 5);
    }

    void MarkSharingServiceReady()
    {
        sharingServiceReady = true;
    }

    /// <summary>
    /// Initializes the room api.
    /// </summary>
    void InitRoomApi()
    {
        // If we have a room, we'll join the first room we see.
        // If we are the user with the lowest user ID, we will create the room.
        // Otherwise we will wait for the room to be created.
        if (roomManager.GetRoomCount() == 0)
        {
            if (LocalUserHasLowestUserId())
            {
                Debug.Log("Creating room ");
                // To keep anchors alive even if all users have left the session ...
                // Pass in true instead of false in CreateRoom.
                currentRoom = roomManager.CreateRoom(new XString("DefaultRoom"), roomID, false);
                currentState = ImportExportState.InitialAnchorRequired;
            }
        }
        else
        {
            Debug.Log("Joining room ");
            currentRoom = roomManager.GetRoom(0);
            roomManager.JoinRoom(currentRoom);
            currentState = ImportExportState.RoomApiInitialized;
        }

        if (currentRoom != null)
        {
            Debug.Log("In room :" + roomManager.GetCurrentRoom().GetName().GetString());
        }
    }

    bool LocalUserHasLowestUserId()
    {
        long localUserId = CustomMessages.Instance.localUserID;
        foreach (long userid in SharingSessionTracker.Instance.UserIds)
        {
            if (userid < localUserId)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Kicks off the process of creating the shared space.
    /// </summary>
    void StartAnchorProcess()
    {
        // First, are there any anchors in this room?
        int anchorCount = currentRoom.GetAnchorCount();

        Debug.Log(anchorCount + " anchors");

        // If there are anchors, we should attach to the first one.
        if (anchorCount > 0)
        {
            // Extract the name of the anchor.
            XString storedAnchorString = currentRoom.GetAnchorName(0);
            string storedAnchorName = storedAnchorString.GetString();

            // Attempt to attach to the anchor in our local anchor store.
            if (AttachToCachedAnchor(storedAnchorName) == false)
            {
                Debug.Log("Starting room download");
                // If we cannot find the anchor by name, we will need the full data blob.
                MakeAnchorDataRequest();
            }
        }
    }

    /// <summary>
    /// Kicks off getting the datablob required to import the shared anchor.
    /// </summary>
    void MakeAnchorDataRequest()
    {
        if (roomManager.DownloadAnchor(currentRoom, currentRoom.GetAnchorName(0)))
        {
            currentState = ImportExportState.DataRequested;
        }
        else
        {
            Debug.Log("Couldn't make the download request.");
            currentState = ImportExportState.Failed;
        }
    }

    void Update()
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
    void CreateAnchorLocally()
    {
        WorldAnchor anchor = GetComponent<WorldAnchor>();
        if (anchor == null)
        {
            anchor = gameObject.AddComponent<WorldAnchor>();
        }

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
            Debug.Log("Found anchor, ready to export");
            currentState = ImportExportState.ReadyToExportInitialAnchor;
        }
        else
        {
            Debug.Log("Failed to locate local anchor (super bad!)");
            currentState = ImportExportState.Failed;
        }

        self.OnTrackingChanged -= Anchor_OnTrackingChanged_InitialAnchor;
    }

    /// <summary>
    /// Attempts to attach to  an anchor by anchorName in the local store..
    /// </summary>
    /// <returns>True if it attached, false if it could not attach</returns>
    bool AttachToCachedAnchor(string AnchorName)
    {
        Debug.Log("Looking for " + AnchorName);
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == AnchorName)
            {
                Debug.Log("Using what we have");
                WorldAnchor wa = anchorStore.Load(ids[index], gameObject);
                if (wa.isLocated)
                {
                    currentState = ImportExportState.Ready;
                }
                else
                {
                    wa.OnTrackingChanged += ImportExportAnchorManager_OnTrackingChanged_Attaching;
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
        Debug.Log("anchor " + located);
        if (located)
        {
            currentState = ImportExportState.Ready;
        }
        else
        {
            Debug.Log("Failed to find local anchor from cache.");
            MakeAnchorDataRequest();
        }

        self.OnTrackingChanged -= ImportExportAnchorManager_OnTrackingChanged_Attaching;
    }

    /// <summary>
    /// Called when a remote anchor has been deserialized
    /// </summary>
    /// <param name="status"></param>
    /// <param name="wat"></param>
    void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch wat)
    {
        if (status == SerializationCompletionReason.Succeeded && wat.GetAllIds().Length > 0)
        {
            Debug.Log("Import complete");

            string first = wat.GetAllIds()[0];
            Debug.Log("Anchor name: " + first);

            WorldAnchor anchor = wat.LockObject(first, gameObject);
            anchorStore.Save(first, anchor);
            currentState = ImportExportState.Ready;
        }
        else
        {
            Debug.Log("Import fail");
            currentState = ImportExportState.DataReady;
        }
    }

    /// <summary>
    /// Exports the currently created anchor.
    /// </summary>
    void Export()
    {
        WorldAnchor anchor = GetComponent<WorldAnchor>();

        if (anchor == null)
        {
            Debug.Log("We should have made an anchor by now...");
            return;
        }

        string guidString = Guid.NewGuid().ToString();
        exportingAnchorName = guidString;

        // Save the anchor to our local anchor store.
        if (anchorStore.Save(exportingAnchorName, anchor))
        {
            sharedAnchorInterface = new WorldAnchorTransferBatch();
            sharedAnchorInterface.AddWorldAnchor(guidString, anchor);
            WorldAnchorTransferBatch.ExportAsync(sharedAnchorInterface, WriteBuffer, ExportComplete);
        }
        else
        {
            Debug.Log("This anchor didn't work, trying again");
            currentState = ImportExportState.InitialAnchorRequired;
        }
    }

    /// <summary>
    /// Called by the WorldAnchorTransferBatch as anchor data is available.
    /// </summary>
    /// <param name="data"></param>
    public void WriteBuffer(byte[] data)
    {
        exportingAnchorBytes.AddRange(data);
    }

    /// <summary>
    /// Called by the WorldAnchorTransferBatch when anchor exporting is complete.
    /// </summary>
    /// <param name="status"></param>
    public void ExportComplete(SerializationCompletionReason status)
    {
        if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > minTrustworthySerializedAnchorDataSize)
        {
            Debug.Log("Uploading anchor: " + exportingAnchorName);
            roomManager.UploadAnchor(
                currentRoom,
                new XString(exportingAnchorName),
                exportingAnchorBytes.ToArray(),
                exportingAnchorBytes.Count);
        }
        else
        {
            Debug.Log("This anchor didn't work, trying again");
            currentState = ImportExportState.InitialAnchorRequired;
        }
    }
}