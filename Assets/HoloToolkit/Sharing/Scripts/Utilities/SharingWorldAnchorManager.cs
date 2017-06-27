// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Wrapper around world anchor store to streamline some of the persistence api busy work.
    /// </summary>
    public class SharingWorldAnchorManager : WorldAnchorManager
    {
        /// <summary>
        /// Called once the anchor has fully uploaded.
        /// </summary>
        public event Action<bool> AnchorUploaded;

        /// <summary>
        /// Called when the anchor has been downloaded.
        /// </summary>
        public event Action<bool, GameObject> AnchorDownloaded;

        /// <summary>
        /// Sometimes we'll see a really small anchor blob get generated.
        /// These tend to not work, so we have a minimum trustworthy size.
        /// </summary>
        private const uint MinTrustworthySerializedAnchorDataSize = 100000;

        /// <summary>
        /// The room manager API for the sharing service.
        /// </summary>
        private RoomManager roomManager;

        /// <summary>
        /// Provides updates when anchor data is uploaded/downloaded.
        /// </summary>
        private RoomManagerAdapter roomManagerListener;

        /// <summary>
        /// WorldAnchorTransferBatch is the primary object in serializing/deserializing anchors.
        /// <remarks>Only available on device.</remarks>
        /// </summary>
        private WorldAnchorTransferBatch currentAnchorTransferBatch;

        private bool isExportingAnchors;
        private bool shouldExportAnchors;

        /// <summary>
        /// The data blob of the anchor.
        /// </summary>
        private List<byte> rawAnchorUploadData = new List<byte>(0);

        private bool isImportingAnchors;
        private bool shouldImportAnchors;

        /// <summary>
        /// The current download anchor data blob.
        /// </summary>
        private byte[] rawAnchorDownloadData;

        #region Unity Methods

        private void Start()
        {
            if (SharingStage.Instance != null)
            {
                ShowDetailedLogs = SharingStage.Instance.ShowDetailedLogs;

                // SharingStage should be valid at this point, but we may not be connected.
                if (SharingStage.Instance.IsConnected)
                {
                    Connected();
                }
                else
                {
                    Disconnected();
                }
            }
            else
            {
                Debug.LogError("SharingWorldAnchorManager requires the SharingStage.");
            }
        }

        protected override void Update()
        {
            if (AnchorStore == null) { return; }

            if (LocalAnchorOperations.Count > 0)
            {
                if (!isExportingAnchors && !isImportingAnchors)
                {
                    DoAnchorOperation(LocalAnchorOperations.Dequeue());
                }
            }
            else
            {
                if (shouldExportAnchors && !isExportingAnchors && !isImportingAnchors)
                {
                    isExportingAnchors = true;
                    shouldExportAnchors = false;
                    WorldAnchorTransferBatch.ExportAsync(currentAnchorTransferBatch, WriteBuffer, ExportComplete);
                }

                if (shouldImportAnchors && !isImportingAnchors && !isExportingAnchors)
                {
                    isImportingAnchors = true;
                    shouldImportAnchors = false;
                    WorldAnchorTransferBatch.ImportAsync(rawAnchorDownloadData, ImportComplete);
                }
            }
        }

        protected override void OnDestroy()
        {
            Disconnected();
            base.OnDestroy();
        }

        #endregion // Unity Methods

        #region Event Callbacks

        /// <summary>
        /// Callback function that contains the WorldAnchorStore object.
        /// </summary>
        /// <param name="anchorStore">The WorldAnchorStore to cache.</param>
        protected override void AnchorStoreReady(WorldAnchorStore anchorStore)
        {
            base.AnchorStoreReady(anchorStore);

            if (SharingStage.Instance != null && !SharingStage.Instance.KeepRoomAlive)
            {
                anchorStore.Clear();
            }
        }

        /// <summary>
        /// Called when the sharing stage connects to a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Events Arguments.</param>
        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;
            SharingStage.Instance.SharingManagerDisconnected += Disconnected;

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += "\nConnected to Server";
            }

            // Setup the room manager callbacks.
            roomManager = SharingStage.Instance.Manager.GetRoomManager();
            roomManagerListener = new RoomManagerAdapter();

            roomManagerListener.AnchorsDownloadedEvent += RoomManagerListener_AnchorsDownloaded;
            roomManagerListener.AnchorUploadedEvent += RoomManagerListener_AnchorUploaded;

            roomManager.AddListener(roomManagerListener);
        }

        /// <summary>
        /// Called when the sharing stage is disconnected from a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void Disconnected(object sender = null, EventArgs e = null)
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerDisconnected -= Disconnected;
                SharingStage.Instance.SharingManagerConnected += Connected;
            }

            if (AnchorDebugText != null)
            {
                AnchorDebugText.text += "\nDisconnected from Server";
            }

            if (roomManagerListener != null)
            {
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
        }

        /// <summary>
        /// Called when anchor upload operations complete.
        /// </summary>
        private void RoomManagerListener_AnchorUploaded(bool successful, XString failureReason)
        {
            if (successful)
            {
                string[] anchorIds = currentAnchorTransferBatch.GetAllIds();

                for (int i = 0; i < anchorIds.Length; i++)
                {
                    if (ShowDetailedLogs)
                    {
                        Debug.LogFormat("[WorldAnchorManager] Successfully uploaded anchor \"{0}\".", anchorIds[i]);
                    }

                    if (AnchorDebugText != null)
                    {
                        AnchorDebugText.text += string.Format("\nSuccessfully uploaded anchor \"{0}\".", anchorIds[i]);
                    }
                }
            }
            else
            {
                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nUpload failed: " + failureReason);
                }

                Debug.LogError("[WorldAnchorManager] Upload failed: " + failureReason);
            }

            rawAnchorUploadData.Clear();
            currentAnchorTransferBatch.Dispose();
            currentAnchorTransferBatch = null;
            isExportingAnchors = false;

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

                if (ShowDetailedLogs)
                {
                    Debug.LogFormat("[WorldAnchorManager] Downloaded {0} bytes.", dataSize.ToString());
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nDownloaded {0} bytes.", dataSize.ToString());
                }

                rawAnchorDownloadData = new byte[dataSize];
                request.GetData(rawAnchorDownloadData, dataSize);
                shouldImportAnchors = true;
            }
            else
            {
                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nAnchor DL failed " + failureReason);
                }

                Debug.LogWarning("[WorldAnchorManager] Anchor DL failed " + failureReason);
            }
        }

        #endregion // Event Callbacks

        protected override bool ImportAnchor(string anchorId, GameObject objectToAnchor)
        {
            // Check room anchor ids and we don't have them in the anchor store, attempt to download them.
            if (SharingStage.Instance.CurrentRoom != null)
            {
                int roomAnchorCount = SharingStage.Instance.CurrentRoom.GetAnchorCount();

                for (int i = 0; i < roomAnchorCount; i++)
                {
                    XString roomAnchorId = SharingStage.Instance.CurrentRoom.GetAnchorName(i);
                    if (roomAnchorId.GetString().Equals(anchorId))
                    {
                        roomManager.DownloadAnchor(SharingStage.Instance.CurrentRoom, anchorId);
                        return true;
                    }
                }
            }

            Debug.LogError("[WorldAnchorManager] Unable to import anchors! We were not in a room.");

            return false;
        }

        /// <summary>
        /// Exports and uploads the anchor to the sharing service.
        /// </summary>
        /// <param name="anchor">The anchor to export.</param>
        protected override void ExportAnchor(WorldAnchor anchor)
        {
            if (SharingStage.Instance == null ||
                SharingStage.Instance.Manager == null ||
                SharingStage.Instance.Manager.GetRoomManager() == null)
            {
                Debug.LogErrorFormat("[WorldAnchorManager] Failed to export anchor \"{0}\"!  The sharing service was not ready.", anchor.name);
                return;
            }

            if (!shouldExportAnchors)
            {
                if (currentAnchorTransferBatch == null)
                {
                    currentAnchorTransferBatch = new WorldAnchorTransferBatch();
                }
                else
                {
                    Debug.LogWarning("[WorldAnchorManager] We didn't properly cleanup our WorldAnchorTransferBatch!");
                }
            }

            currentAnchorTransferBatch.AddWorldAnchor(anchor.name, anchor);
            shouldExportAnchors = true;
        }

        /// <summary>
        /// Called by the WorldAnchorTransferBatch when anchor exporting is complete.
        /// </summary>
        /// <param name="status">Serialization Status.</param>
        private void ExportComplete(SerializationCompletionReason status)
        {
            if (status == SerializationCompletionReason.Succeeded &&
                rawAnchorUploadData.Count > MinTrustworthySerializedAnchorDataSize)
            {
                if (ShowDetailedLogs)
                {
                    Debug.LogFormat("[WorldAnchorManager] Exporting {0} anchors with {1} bytes.",
                        currentAnchorTransferBatch.anchorCount.ToString(),
                        rawAnchorUploadData.ToArray().Length.ToString());
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nExporting {0} anchors with {1} bytes.",
                        currentAnchorTransferBatch.anchorCount.ToString(),
                        rawAnchorUploadData.ToArray().Length.ToString());
                }

                string[] anchorNames = currentAnchorTransferBatch.GetAllIds();

                for (var i = 0; i < anchorNames.Length; i++)
                {
                    SharingStage.Instance.Manager.GetRoomManager().UploadAnchor(
                        SharingStage.Instance.CurrentRoom,
                        new XString(anchorNames[i]),
                        rawAnchorUploadData.ToArray(),
                        rawAnchorUploadData.Count);
                }
            }
            else
            {
                Debug.LogWarning("[WorldAnchorManager] Failed to upload anchor!");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += "\nFailed to upload anchor!";
                }

                if (rawAnchorUploadData.Count < MinTrustworthySerializedAnchorDataSize)
                {
                    Debug.LogWarning("[WorldAnchorManager] Anchor data was not valid.  Try creating the anchor again.");

                    if (AnchorDebugText != null)
                    {
                        AnchorDebugText.text += "\nAnchor data was not valid.  Try creating the anchor again.";
                    }
                }
            }
        }

        /// <summary>
        /// Called when a remote anchor has been deserialized.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="anchorBatch"></param>
        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch anchorBatch)
        {
            bool successful = status == SerializationCompletionReason.Succeeded;
            GameObject objectToAnchor = null;

            if (successful)
            {
                if (ShowDetailedLogs)
                {
                    Debug.LogFormat("[WorldAnchorManager] Successfully imported \"{0}\" anchors.", anchorBatch.anchorCount.ToString());
                }

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += string.Format("\nSuccessfully imported \"{0}\" anchors.", anchorBatch.anchorCount.ToString());
                }

                string[] anchorNames = anchorBatch.GetAllIds();

                for (var i = 0; i < anchorNames.Length; i++)
                {
                    if (AnchorGameObjectReferenceList.TryGetValue(anchorNames[i], out objectToAnchor))
                    {
                        AnchorStore.Save(anchorNames[i], anchorBatch.LockObject(anchorNames[i], objectToAnchor));
                    }
                    else
                    {
                        //TODO: Figure out how to get the GameObject reference from across the network.  For now it's best to use unique GameObject names.
                        Debug.LogWarning("[WorldAnchorManager] Unable to import anchor!  We don't know which GameObject to anchor!");
                    }
                }
            }
            else
            {
                Debug.LogError("[WorldAnchorManager] Import failed!");

                if (AnchorDebugText != null)
                {
                    AnchorDebugText.text += "\nImport failed!";
                }
            }

            if (AnchorDownloaded != null)
            {
                AnchorDownloaded(successful, objectToAnchor);
            }

            anchorBatch.Dispose();
            rawAnchorDownloadData = null;
            isImportingAnchors = false;
        }

        /// <summary>
        /// Called by the WorldAnchorTransferBatch as anchor data is available.
        /// </summary>
        /// <param name="data"></param>
        private void WriteBuffer(byte[] data)
        {
            rawAnchorUploadData.AddRange(data);
        }
    }
}
