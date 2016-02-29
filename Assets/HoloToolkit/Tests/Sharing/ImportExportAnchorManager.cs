using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Sharing;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System;
using HoloToolkit.Sharing;

/// <summary>
/// Manages creating shared anchors and sharing the anchors with other clients.
/// </summary>
public class ImportExportAnchorManager : Singleton<ImportExportAnchorManager>
{
    /// <summary>
    /// Enum to track the progress through establishing a shared coordinate system.
    /// </summary>
    enum ImportExportState
    {
        Start,
        InitializingAnchorStore,
        AnchorStoreInitialized,
        RoomApiInitialized,
        DataRequested,
        DataReady,
        Importing,
        InitialAnchorRequired,
        CreatingInitialAnchor,
        ReadyToExportInitialAnchor,
        Ready,
        Failed
    }

    ImportExportState currentState = ImportExportState.Start;

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
    byte[] RawAnchorData = null;

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
    bool SharingServiceReady = false;

    /// <summary>
    /// Interface to the sharing service.
    /// </summary>
    XtoolsServerManager xtoolsManager;

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
    /// Keeps track of outstanding anchor download requests to the sharing service.
    /// </summary>
    AnchorDownloadRequest anchorDownloadRequest = null;

    void Start()
    {
        Debug.Log("Import Export Manager starting");

        xtoolsManager = XtoolsServerManager.Instance;

        // We need to get our local anchor store started up.
        currentState = ImportExportState.InitializingAnchorStore;
        WorldAnchorStore.GetAsync(AnchorStoreReady);

        // We will register for session joined to indicate wwhen the sharing service
        // is ready for us to make room related requests.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }

    /// <summary>
    /// Called when the local anchor store is ready for business.
    /// </summary>
    /// <param name="store"></param>
    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        currentState = ImportExportState.AnchorStoreInitialized;
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
        SharingServiceReady = true;
    }

    /// <summary>
    /// Initializes the room api.  
    /// </summary>
    void InitRoomApi()
    {
        roomManager = NetworkStage.Instance.Manager.GetRoomManager();

        // If we have a room, we'll join the first room we see.
        // Otherwise we'll make a room.
        if (roomManager.GetRoomCount() == 0)
        {
            Debug.Log("Creating room ");
            currentRoom = roomManager.CreateRoom(new XString("DefaultRoom"), 0);
        }
        else
        {
            Debug.Log("Joining room ");
            currentRoom = roomManager.GetRoom(0);
            roomManager.JoinRoom(currentRoom);
        }

        Debug.Log("In room :"+roomManager.GetCurrentRoom().GetName().GetString());
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
            // Extrac the name of the anchor.
            XString storedAnchorString = currentRoom.GetAnchorName(0);
            string storedAnchorName = storedAnchorString.GetString();

            // Attempt to attach to the anchor in our local anchor store.    
            if (AttachToCachedAnchor(storedAnchorName) == false)
            {
                Debug.Log("Starting room download");
                // If we cannot find the anchor by name, we will need the full data
                // blob.
                MakeAnchorDataRequest();
            }
        }
        else
        {
            // If there are no anchors in the room we should make the anchor.
            // We should only do this if we are in the room alone.
            // Otherwise another user should be making it.
            // **** I'm a little sketchy about this ****
            if (currentRoom.GetUserCount() == 1) 
            {
                currentState = ImportExportState.InitialAnchorRequired;
            }
        }
    }

    /// <summary>
    /// Kicks off getting the datablob required to import the shared anchor.
    /// </summary>
    void MakeAnchorDataRequest()
    {
        anchorDownloadRequest = roomManager.DownloadAnchor(currentRoom, currentRoom.GetAnchorName(0));
        currentState = ImportExportState.DataRequested;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            // If the local anchor store is initialized.
            case ImportExportState.AnchorStoreInitialized:
                if (SharingServiceReady)
                {
                    InitRoomApi();
                    currentState = ImportExportState.RoomApiInitialized;
                    StartAnchorProcess();
                }
                break;
            case ImportExportState.RoomApiInitialized:
                StartAnchorProcess();
                break;
            case ImportExportState.DataRequested:
                if (anchorDownloadRequest.IsDownloading() == false)
                {
                    int datasize = anchorDownloadRequest.GetDataSize();
                    Debug.Log(datasize + " bytes ");
                    RawAnchorData = new byte[datasize];
                    anchorDownloadRequest.GetData(RawAnchorData, datasize);
                    currentState = ImportExportState.DataReady;
                }
                break;
            case ImportExportState.DataReady:
                // DataReady is set when the unity editor has sent us the anchor's data blob.
                currentState = ImportExportState.Importing;
                WorldAnchorTransferBatch.ImportAsync(RawAnchorData, ImportComplete);
                break;
            case ImportExportState.InitialAnchorRequired:
                currentState = ImportExportState.CreatingInitialAnchor;
                // Since there is no anchor, there will also be no planes.
                CreateAnchorLocally();
                break;
            case ImportExportState.ReadyToExportInitialAnchor:
                // We've created an anchor locally and are satisfied that it is ready
                // to export.
                currentState = ImportExportState.Ready;
                Export();
                break;
        }

    }

    /// <summary>
    /// Starts establishing a new anchor.
    /// </summary>
    void CreateAnchorLocally()
    {
        // Default to 2m in front of user.
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;

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
            anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
        }
    }

    /// <summary>
    /// Callback to trigger when an anchor has been 'found'.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="located"></param>
    private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
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
    }

    /// <summary>
    /// Attmpts to attach to  an anchor by anchorName in the local store..
    /// </summary>
    /// <param name="AnchorName"></param>
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
                    wa.OnTrackingChanged += ImportExportAnchorManager_OnTrackingChanged; ;
                }
                return true;
            }
        }

        // didn't find the anchor
        return false;
    }

    /// <summary>
    /// Called when tracking changes for a 'cached' anchor.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="located"></param>
    private void ImportExportAnchorManager_OnTrackingChanged(WorldAnchor self, bool located)
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

        self.OnTrackingChanged -= ImportExportAnchorManager_OnTrackingChanged;
    }

    /// <summary>
    /// Called when a remote anchor has been deserialzied
    /// </summary>
    /// <param name="status"></param>
    /// <param name="wat"></param>
    void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch wat)
    {
        if (status == SerializationCompletionReason.Succeeded)
        {
            Debug.Log("Import complete");
            string first = wat.GetAllIds()[0];
            WorldAnchor myFriend = wat.LockObject(first, gameObject);
            anchorStore.Save(first, myFriend);
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

        WorldAnchor[] anchors = new WorldAnchor[1];
        anchors[0] = anchor;


        string guidString = Guid.NewGuid().ToString();
        exportingAnchorName = guidString;

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
        Debug.LogFormat("{0}", data.Length);
    }

    /// <summary>
    /// Called by the WorldAnchorTransferBatch when anchor exporting is complete.
    /// </summary>
    /// <param name="status"></param>
    public void ExportComplete(SerializationCompletionReason status)
    {
        Debug.Log("status " + status + " bytes: " + exportingAnchorBytes.Count);
        if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > minTrustworthySerializedAnchorDataSize)
        {
            Debug.Log("Sending " + exportingAnchorName);
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