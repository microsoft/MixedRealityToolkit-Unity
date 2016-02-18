using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Sharing;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System;
using HoloToolkit.XTools;

/// <summary>
/// Manages creating shared anchors and sharing the anchors with other clients.
/// </summary>
public class ImportExportAnchorManager : Singleton<ImportExportAnchorManager>
{

    // protocol
    // 1 - request location name
    // 2 - if name is "" create anchor and send
    // 3 - if name is a location we know, load it
    // 4 - if name is not a location we know request full data

    /// <summary>
    /// Enum to track the progress through establishing a shared coordinate system.
    /// </summary>
    enum ImportExportState
    {
        Start,
        InitializingAnchorStore,
        AnchorStoreInitialized,
        NameRequested,
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
    /// Keeps track of stored anchor name.
    /// </summary>
    string storedAnchorName = "";

    /// <summary>
    /// Keeps track of stored anchor data blob.
    /// </summary>
    byte[] RawAnchorData = null;

    /// <summary>
    /// Keeps track of locally stored anchors.
    /// </summary>
    WorldAnchorStore anchorStore = null;

    /// <summary>
    /// Keeps track of the user ID associated with the editor (anchor repository).
    /// </summary>
    long EditorId { get; set; }

    /// <summary>
    /// The device which first asks for the anchor name will be sent an
    /// empty string.  This is a signal to create the first anchor.  
    /// Subsequent name requests will be defered until we get the anchor
    /// data blob.  Ultimately this probably needs to be a list of Xtools 
    /// user ids to send the anchor infromation to, but since all we can do is
    /// broadcast, this should be enough for now.
    /// </summary>
    public bool DeferAnchorNameRequests { get; set; }

    /// <summary>
    /// Keeps track of the name of the anchor we are exporting.
    /// </summary>
    string exportingAnchorName { get; set; }

    /// <summary>
    /// The datablob of the anchor.
    /// </summary>
    List<byte> exportingAnchorBytes = new List<byte>();

    XtoolsServerManager xtoolsManager;

    const uint minTrustworthySerializedAnchorDataSize = 100000;

    void Start()
    {
        xtoolsManager = XtoolsServerManager.Instance;
        Debug.Log("Import Export Manager starting");
        xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorName] = this.OnPostAnchorName;
        xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorData] = this.OnPostAnchorData;

#if !UNITY_EDITOR
        // When on device we need to get our local anchor store started up.
        currentState = ImportExportState.InitializingAnchorStore;
        WorldAnchorStore.GetAsync(AnchorStoreReady);
#else
        xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.RequestAnchorName] = this.OnRequestAnchorName;
        xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.RequestAnchorData] = this.OnRequestAnchorData;
        currentState = ImportExportState.Ready;
        DeferAnchorNameRequests = false;
#endif
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        currentState = ImportExportState.AnchorStoreInitialized;
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        switch (currentState)
        {
            case ImportExportState.AnchorStoreInitialized:
                // When the anchor store is initialized we can request the anchor name from
                // the Unity editor.
                if (xtoolsManager.SendRequestAnchorName())
                {
                    currentState = ImportExportState.NameRequested;
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
#else
        currentState = ImportExportState.Ready;
#endif
    }

    /// <summary>
    /// Function called when a remote client is asking for the anchor name.
    /// </summary>
    /// <param name="msg"></param>
    void OnRequestAnchorName(HoloToolkit.XTools.NetworkInMessage msg)
    {
        // on device this is a noop.
#if UNITY_EDITOR
        Debug.Log("Anchor name requested");
        if (DeferAnchorNameRequests)
        {
            Debug.Log("Will send anchor name once we get anchor data");
            return;
        }

        if (RawAnchorData == null || RawAnchorData.Length < 300)
        {
            storedAnchorName = "";
        }

        // Parse the message
        long userID = msg.ReadInt64();
        xtoolsManager.SendPostAnchorName(storedAnchorName);
        DeferAnchorNameRequests = string.IsNullOrEmpty(storedAnchorName);
#endif
    }

    /// <summary>
    /// Function called when a client is providing the anchor name.
    /// </summary>
    /// <param name="msg"></param>
    void OnPostAnchorName(HoloToolkit.XTools.NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();
        byte isEditor = msg.ReadByte();
        string anchorName = msg.ReadString().GetString();

        Debug.Log(string.Format(
            "Got anchor {0} from User {1} who {2} editor",
            anchorName,
            userID,
            isEditor == 0 ? "is not" : "is"));

        if (!string.IsNullOrEmpty(storedAnchorName))
        {
            Debug.Log("Not expecting a new anchor");
            return;
        }
        // We only want to get the anchor name from the unity editor instance.
#if !UNITY_EDITOR
        if (isEditor == 0)
        {
            Debug.Log("Ignoring non-editor name");
            return;
        }
#endif
        storedAnchorName = anchorName;

#if !UNITY_EDITOR
        if (string.IsNullOrEmpty(storedAnchorName))
        {
            Debug.Log("Anchor name empty, making a new anchor");
            // need to make a new anchor
            currentState = ImportExportState.InitialAnchorRequired;
            EditorId = userID;
        }
        else
        {
            // Attempt to attach to the anchor in our local anchor store.    
            if (AttachToCachedAnchor(storedAnchorName) == false)
            {
                // If we cannot find the anchor by name, we will need the full data
                // blob.
                xtoolsManager.SendRequestAnchorData();
                currentState = ImportExportState.DataRequested;
            }
        }

        // We will no longer care about anchor name updates.
        xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorName] = null;
#endif
    }

    /// <summary>
    /// Called when a client is requesting the full anchor blob.
    /// Ideally this would only send the blob to the client that 'cares'.
    /// </summary>
    /// <param name="msg"></param>
    void OnRequestAnchorData(NetworkInMessage msg)
    {
        // on device this is a noop.
#if UNITY_EDITOR
        Debug.Log("Anchor data requested");
        // Parse the message
        long userID = msg.ReadInt64();
        if (!DeferAnchorNameRequests)
        {
            xtoolsManager.SendPostAnchorData(userID,RawAnchorData);
        }
#endif
    }

    /// <summary>
    /// Called when a remote client has sent the data blob required to 
    /// deserialize an anchor.
    /// </summary>
    /// <param name="msg"></param>
    void OnPostAnchorData(NetworkInMessage msg)
    {
        Debug.Log("Got anchor Data");
        // Parse the message
        long userID = msg.ReadInt64();
        byte isEditor = msg.ReadByte();
        // We only want to get the anchor data from the unity editor instance.
        // (Unless this is the editor).
#if !UNITY_EDITOR
        if (isEditor == 0)
        {
            Debug.Log("Ignoring non-editor data");
            return;
        }
#endif
        if (RawAnchorData == null) // only process an anchor once.
        {
            uint dataLen = (uint)msg.ReadInt32();
            if (dataLen > minTrustworthySerializedAnchorDataSize)
            {
                RawAnchorData = new byte[dataLen];
                msg.ReadArray(RawAnchorData, dataLen);
#if !UNITY_EDITOR
                currentState = ImportExportState.DataReady;
#else
                if (DeferAnchorNameRequests)
                {
                    DeferAnchorNameRequests = false;
                    xtoolsManager.SendPostAnchorName(storedAnchorName);
                }
#endif
                // We will no longer care about new anchor data.
                xtoolsManager.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorData] = null;
            }
            else
            {
                Debug.Log("Got bad data");
                currentState = ImportExportState.Failed;
            }
        }
        else
        {
            Debug.Log("Ignoring already processed anchor");
        }

    }

    /// <summary>
    /// Starts establishing a new anchor.
    /// </summary>
    void CreateAnchorLocally()
    {
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
        else {
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
            Debug.Log(ids[index]);
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
            xtoolsManager.SendRequestAnchorData();
            currentState = ImportExportState.DataRequested;
        }
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
            storedAnchorName = first;
            anchorStore.Save(storedAnchorName, myFriend);
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
            XtoolsServerManager.Instance.SendPostAnchorName(exportingAnchorName);
            XtoolsServerManager.Instance.SendPostAnchorData(EditorId, exportingAnchorBytes.ToArray());
        }
        else
        {

            Debug.Log("This anchor didn't work, trying again");
            currentState = ImportExportState.InitialAnchorRequired;
        }
    }

}