using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.XTools;
using UnityEngine.VR.WSA.Sharing;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System;

/// <summary>
/// Manages creating shared anchors and sharing the anchors with other clients.
/// </summary>
public class ImportExportAnchorManager : Singleton<ImportExportAnchorManager>
{

    // Protocol
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

    const uint messageSize = 100000;

    void Start()
    {
        Debug.Log("Import Export Manager starting");

        XtoolsServerManager.Instance.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorName] = this.OnPostAnchorName;
        XtoolsServerManager.Instance.MessageHandlers[XtoolsServerManager.TestMessageID.RequestAnchorName] = this.OnRequestAnchorName;
        XtoolsServerManager.Instance.MessageHandlers[XtoolsServerManager.TestMessageID.PostAnchorData] = this.OnPostAnchorData;
        XtoolsServerManager.Instance.MessageHandlers[XtoolsServerManager.TestMessageID.RequestAnchorData] = this.OnRequestAnchorData;
#if !UNITY_EDITOR
        // When on device we need to get our local anchor store started up.
        currentState = ImportExportState.InitializingAnchorStore;
        WorldAnchorStore.GetAsync(AnchorStoreReady);
#else
        currentState = ImportExportState.Ready;
#endif
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        currentState = ImportExportState.AnchorStoreInitialized;
    }

    void Update()
    {
#if !UNITY_EDITOR
        switch (currentState)
        {
            case ImportExportState.AnchorStoreInitialized:
                // When the anchor store is initialized we can request the anchor name from
                // the Unity editor.
                if (XtoolsServerManager.Instance.SendRequestAnchorName())
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
    void OnRequestAnchorName(NetworkInMessage msg)
    {
        // on device this is a noop.
#if UNITY_EDITOR
        Debug.Log("Anchor name requested");
        if (RawAnchorData == null || RawAnchorData.Length < 300)
        {
            storedAnchorName = "";
        }

        XtoolsServerManager.Instance.SendPostAnchorName(storedAnchorName);
#endif
    }

    /// <summary>
    /// Function called when a client is providing the anchor name.
    /// </summary>
    /// <param name="msg"></param>
    void OnPostAnchorName(NetworkInMessage msg)
    {
        Debug.Log("Got anchor name");

        if (!string.IsNullOrEmpty(storedAnchorName))
        {
            Debug.Log("Not expecting a new anchor");
            return;
        }

        // Parse the message
        storedAnchorName = msg.ReadString().GetString();

#if !UNITY_EDITOR
        if (string.IsNullOrEmpty(storedAnchorName))
        {
            Debug.Log("Anchor name empty, making a new anchor");
            // need to make a new anchor
            currentState = ImportExportState.InitialAnchorRequired;
        }
        else
        {
            // Attempt to attach to the anchor in our local anchor store.    
            if (AttachToCachedAnchor(storedAnchorName) == false)
            {
                // If we cannot find the anchor by name, we will need the full data
                // blob.
                XtoolsServerManager.Instance.SendRequestAnchorData();
                currentState = ImportExportState.DataRequested;
            }
        }
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
        XtoolsServerManager.Instance.SendPostAnchorData(RawAnchorData);
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
        if (RawAnchorData == null) // only process an anchor once.
        {
            uint dataLen = (uint)msg.ReadInt32();
            if (dataLen > messageSize)
            {
                RawAnchorData = new byte[dataLen];
                msg.ReadArray(RawAnchorData, dataLen);
#if !UNITY_EDITOR
                currentState = ImportExportState.DataReady;
#endif
            }
            else
            {
                Debug.Log("Got bad anchor data");
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
        Debug.Log("Looking for anchor:" + AnchorName);
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            Debug.Log(ids[index]);
            if (ids[index] == AnchorName)
            {
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
        Debug.Log("Anchor located: " + located);
        if (located)
        {
            currentState = ImportExportState.Ready;
        }
        else
        {
            Debug.Log("Failed to find local anchor from cache.");
            XtoolsServerManager.Instance.SendRequestAnchorData();
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
        DataWriter dw = new DataWriter();
        dw.OnExportFailed = (() =>
        {
            Debug.Log("This anchor didn't work, trying again");
            currentState = ImportExportState.InitialAnchorRequired;
        });

        string guidString = Guid.NewGuid().ToString();
        dw.anchorName = guidString;
        Debug.Log("Anchor name: " + dw.anchorName);
        if (anchorStore.Save(dw.anchorName, anchor))
        {
            sharedAnchorInterface = new WorldAnchorTransferBatch();
            sharedAnchorInterface.AddWorldAnchor(guidString, anchor);
            WorldAnchorTransferBatch.ExportAsync(sharedAnchorInterface, dw.WriteBuffer, dw.ExportComplete);
        }
        else
        {
            Debug.Log("This anchor store save didn't work, trying again");
            currentState = ImportExportState.InitialAnchorRequired;
        }
        
    }

    /// <summary>
    /// Simple class to handle exporting an anchor.
    /// </summary>
    class DataWriter
    {
        public string anchorName { get; set; }
        List<byte> bytes = new List<byte>();

        public Action OnExportFailed { get; set; }
        /// <summary>
        /// Called by the WorldAnchorTransferBatch as anchor data is available.
        /// </summary>
        /// <param name="data"></param>
        public void WriteBuffer(byte[] data)
        {
            bytes.AddRange(data);
            Debug.LogFormat("{0}", data.Length);
        }

        /// <summary>
        /// Called by the WorldAnchorTransferBatch when anchor exporting is complete.
        /// </summary>
        /// <param name="status"></param>
        public void ExportComplete(SerializationCompletionReason status)
        {
            Debug.Log("status " + status + " bytes: " + bytes.Count);
            if (status == SerializationCompletionReason.Succeeded && bytes.Count > messageSize)
            {
                Debug.Log("Sending " + anchorName);
                XtoolsServerManager.Instance.SendPostAnchorName(anchorName);
                XtoolsServerManager.Instance.SendPostAnchorData(bytes.ToArray());
            }
            else
            {
               
                if(OnExportFailed != null)
                {
                    OnExportFailed();
                }
                Debug.Log("Export failed");
            }
        }
    }
}
