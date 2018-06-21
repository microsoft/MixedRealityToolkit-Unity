// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Networking;
#if UNITY_WSA
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA.Sharing;
#else
using UnityEngine.VR;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;
#endif
using HoloToolkit.Unity.SpatialMapping;
#endif

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// Creates, exports, and imports anchors as required.
    /// </summary>
    public class UNetAnchorManager : NetworkBehaviour
    {
        /// <summary>
        ///  Since we aren't a MonoBehavior we can't just use the singleton class
        ///  so we'll reroll it as a one off here.
        /// </summary>
        private static UNetAnchorManager _Instance;

        public static UNetAnchorManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<UNetAnchorManager>();
                }
                return _Instance;
            }
        }

        /// <summary>
        /// Keeps track of the name of the world anchor to use.
        /// </summary>
        [SyncVar]
        public string AnchorName = "";
        
        /// <summary>
        /// Use the spatial mapping to find a position for the anchor
        /// </summary>
        public bool UseSpatialMapping = true;

        /// <summary>
        /// Tracks if we have a shared anchor established
        /// </summary>
        public bool AnchorEstablished { get; set; }

        /// <summary>
        /// Tracks if an import is in flight.
        /// </summary>
        public bool ImportInProgress { get; private set; }

        /// <summary>
        /// Tracks if a download is in flight.
        /// </summary>
        public bool DownloadingAnchor { get; private set; }

        /// <summary>
        /// The UNet network manager in the scene.
        /// </summary>
        private NetworkManager networkManager;

        /// <summary>
        /// The UNetNetworkTransmitter in the scene which can send an anchor to another device.
        /// </summary>
        private GenericNetworkTransmitter networkTransmitter;

#if UNITY_WSA
        const string SavedAnchorKey = "SavedAnchorName";

        /// <summary>
        /// Tracks if we had to manually start the observer so we should turn it off when we don't need it.
        /// </summary>
        private bool StartedObserver = false;

        /// <summary>
        /// While seeking a good place to put an anchor, we use spatial mapping;
        /// </summary>
        private SpatialMappingManager spatialMapping;

        /// <summary>
        /// The object to attach the anchor to when created or imported.
        /// </summary>
        private GameObject objectToAnchor;

        /// <summary>
        /// Sometimes we'll see a really small anchor blob get generated.
        /// These tend to not work, so we have a minimum trustworthy size.
        /// </summary>
        private const uint MinTrustworthySerializedAnchorDataSize = 500000;

        /// <summary>
        /// List of bytes that represent the anchor data to export.
        /// </summary>
        private List<byte> exportingAnchorBytes = new List<byte>(0);

        /// <summary>
        /// Keeps track of if we created the anchor.
        /// </summary>
        private bool createdAnchor = false;

        /// <summary>
        /// Previous anchor name.
        /// </summary>
        private string oldAnchorName = "";

        /// <summary>
        /// The anchorData to import.
        /// </summary>
        private byte[] anchorData = null;

        /// <summary>
        /// Tracks if we have updated data to import.
        /// </summary>
        private bool gotOne = false;

        /// <summary>
        /// Keeps track of the name of the anchor we are exporting.
        /// </summary>
        private string exportingAnchorName;
#endif

        /// <summary>
        /// Ensures that the scene has what we need to continue.
        /// </summary>
        /// <returns>True if we can proceed, false otherwise.</returns>
        private bool CheckConfiguration()
        {
            networkTransmitter = GenericNetworkTransmitter.Instance;
            if (networkTransmitter == null)
            {
                Debug.Log("No UNetNetworkTransmitter found in scene");
                return false;
            }

            networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                Debug.Log("No NetworkManager found in scene");
                return false;
            }

            if (SharedCollection.Instance == null)
            {
                Debug.Log("No SharedCollection found in scene");
                return false;
            }

#if UNITY_WSA
            objectToAnchor = SharedCollection.Instance.gameObject;

            if (UseSpatialMapping) 
            {
                spatialMapping = SpatialMappingManager.Instance;
                if (spatialMapping == null)
                {
                    Debug.Log("Spatial mapping not found in scene. Better anchor locations can be found if a SpatialMappingManager is in the scene");
                }
            }
#endif

            return true;
        }

        private void Start()
        {
            if (!CheckConfiguration())
            {
                Debug.Log("Missing required component for UNetAnchorManager");
                Destroy(this);
                return;
            }

#if UNITY_WSA && !(ENABLE_IL2CPP && NET_STANDARD_2_0)
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque)
#else
            if (!VRDevice.isPresent)
#endif
            {
                AnchorEstablished = true;
            }
            else
            {
                networkTransmitter.DataReadyEvent += NetworkTransmitter_DataReadyEvent;
            }
#else
            AnchorEstablished = true;
#endif

            // If we have a debug panel, then we have debug data for the panel. 
            DebugPanel debugPanel = DebugPanel.Instance;
            if (debugPanel != null)
            {
                DebugPanel.Instance.RegisterExternalLogCallback(GenerateDebugData);
            }
        }

        private void Update()
        {
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque)
            {
                return;
            }
#else
            if (!VRDevice.isPresent)
            {
                return;
            }
#endif

            if (gotOne)
            {
                Debug.Log("Importing");
                gotOne = false;
                ImportInProgress = true;
                WorldAnchorTransferBatch.ImportAsync(anchorData, ImportComplete);
            }

            if (oldAnchorName != AnchorName && !createdAnchor)
            {
                Debug.LogFormat("New anchor name {0} => {1}", oldAnchorName, AnchorName);
                oldAnchorName = AnchorName;
                if (string.IsNullOrEmpty(AnchorName))
                {
                    Debug.Log("Anchor is empty");
                    AnchorEstablished = false;
                }
                else if (!AttachToCachedAnchor(AnchorName))
                {
                    Debug.Log("Requesting download of anchor data");
                    WaitForAnchor();
                }
            }
#endif
        }

        /// <summary>
        /// Creates a debug string with information about the anchor state.
        /// </summary>
        /// <returns>The calculated string</returns>
        private string GenerateDebugData()
        {
#if UNITY_WSA
            return string.Format("Anchor Name: {0}\nAnchor Size: {1}\nAnchor Established?: {2}\nImporting?: {3}\nDownloading? {4}\n",
                AnchorName,
                anchorData == null ? exportingAnchorBytes.Count : anchorData.Length,
                AnchorEstablished.ToString(),
                ImportInProgress.ToString(),
                DownloadingAnchor.ToString());
#else
            return "No Anchor data";
#endif
        }

        /// <summary>
        /// If we are supposed to create the anchor for export, this is the function to call.
        /// </summary>
        public void CreateAnchor()
        {
#if UNITY_WSA
            exportingAnchorBytes.Clear();
            GenericNetworkTransmitter.Instance.SetData(null);
            objectToAnchor = SharedCollection.Instance.gameObject;
            FindAnchorPosition();
#endif
        }

#if UNITY_WSA
        /// <summary>
        /// Finds a good position to set the anchor.
        /// 1. If we have an anchor stored in the player prefs/ anchor store, use that
        /// 2. If we don't have spatial mapping, just use where the object happens to be 
        /// 3. if we do have spatial mapping, anchor at a vertex dense portion of spatial mapping
        /// </summary>
        private void FindAnchorPosition()
        {
            // 1. recover a stored anchor if we can
            if (PlayerPrefs.HasKey(SavedAnchorKey) && AttachToCachedAnchor(PlayerPrefs.GetString(SavedAnchorKey)))
            {
                exportingAnchorName = PlayerPrefs.GetString(SavedAnchorKey);
                Debug.Log("found " + exportingAnchorName + " again");
                ExportAnchor();
            }
            // 2. just use the current object position if we don't have access to spatial mapping
            else if (spatialMapping == null)
            {
                if (UseSpatialMapping)
                {
                    Debug.Log("No spatial mapping...");
                }
                
                ExportAnchorAtPosition(objectToAnchor.transform.position);
            }
            // 3. seek a vertex dense portion of spatial mapping
            else
            {
                ReadOnlyCollection<SpatialMappingSource.SurfaceObject> surfaces = spatialMapping.GetSurfaceObjects();
                if (surfaces == null || surfaces.Count == 0)
                {
                    // If we aren't getting surfaces we may need to start the observer.
                    if (spatialMapping.IsObserverRunning() == false)
                    {
                        spatialMapping.StartObserver();
                        StartedObserver = true;
                    }

                    // And try again after the observer has a chance to get an update.
                    Invoke("FindAnchorPosition", spatialMapping.GetComponent<SpatialMappingObserver>().TimeBetweenUpdates);
                }
                else
                {
                    float startTime = Time.realtimeSinceStartup;
                    // If we have surfaces, we need to iterate through them to find a dense area
                    // of geometry, which should provide a good spot for an anchor.
                    Mesh bestMesh = null;
                    MeshFilter bestFilter = null;
                    int mostVerts = 0;

                    for (int index = 0; index < surfaces.Count; index++)
                    {
                        // If the current surface doesn't have a filter or a mesh, skip to the next one
                        // This happens as a surface is being processed.  We need to track both the mesh 
                        // and the filter because the mesh has the verts in local space and the filter has the transform to 
                        // world space.
                        MeshFilter currentFilter = surfaces[index].Filter;
                        if (currentFilter == null)
                        {
                            continue;
                        }

                        Mesh currentMesh = currentFilter.sharedMesh;
                        if (currentMesh == null)
                        {
                            continue;
                        }

                        // If we have a collider we can use the extents to estimate the volume.
                        MeshCollider currentCollider = surfaces[index].Collider;
                        float volume = currentCollider == null ? 1.0f : currentCollider.bounds.extents.magnitude;

                        // get th verts divided by the volume if any
                        int meshVerts = (int)(currentMesh.vertexCount / volume);

                        // and if this is most verts/volume we've seen, record this mesh as the current best.
                        mostVerts = Mathf.Max(meshVerts, mostVerts);
                        if (mostVerts == meshVerts)
                        {
                            bestMesh = currentMesh;
                            bestFilter = currentFilter;
                        }
                    }

                    // If we have a good area to use, then use it.
                    if (bestMesh != null && mostVerts > 100)
                    {
                        // Get the average of the vertices
                        Vector3[] verts = bestMesh.vertices;
                        Vector3 avgVert = verts.Average();

                        // transform the average into world space.
                        Vector3 center = bestFilter.transform.TransformPoint(avgVert);

                        Debug.LogFormat("found a good mesh mostVerts = {0} processed {1} meshes in {2} ms", mostVerts, surfaces.Count, 1000 * (Time.realtimeSinceStartup - startTime));
                        // then export the anchor where we've calculated.
                        ExportAnchorAtPosition(center);
                    }
                    else
                    {
                        // If we didn't find a good mesh, try again a little later.
                        Debug.LogFormat("Failed to find a good mesh mostVerts = {0} processed {1} meshes in {2} ms", mostVerts, surfaces.Count, 1000 * (Time.realtimeSinceStartup - startTime));
                        Invoke("FindAnchorPosition", spatialMapping.GetComponent<SpatialMappingObserver>().TimeBetweenUpdates);
                    }
                }
            }
        }

        /// <summary>
        /// Creates and exports the anchor at the specified world position
        /// </summary>
        /// <param name="worldPos">The position to place the anchor</param>
        private void ExportAnchorAtPosition(Vector3 worldPos)
        {
            // Need to remove any anchor that is on the object before we can move the object.
            WorldAnchor worldAnchor = objectToAnchor.GetComponent<WorldAnchor>();
            if (worldAnchor != null)
            {
                DestroyImmediate(worldAnchor);
            }

            // Move the object to the specified place
            objectToAnchor.transform.position = worldPos;

            // Attach a new anchor
            worldAnchor = objectToAnchor.AddComponent<WorldAnchor>();

            // Name the anchor
            exportingAnchorName = Guid.NewGuid().ToString();
            Debug.Log("preparing " + exportingAnchorName);

            // Register for on tracking changed in case the anchor isn't already located
            worldAnchor.OnTrackingChanged += WorldAnchor_OnTrackingChanged;

            // And call our callback in line just in case it is already located.
            WorldAnchor_OnTrackingChanged(worldAnchor, worldAnchor.isLocated);
        }

        /// <summary>
        /// Callback for when tracking changes for an anchor
        /// </summary>
        /// <param name="self">The anchor that tracking has changed for.</param>
        /// <param name="located">Bool if the anchor is located</param>
        private void WorldAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                // If we have located the anchor we can export it.
                Debug.Log("exporting " + exportingAnchorName);
                // And we don't need this callback anymore
                self.OnTrackingChanged -= WorldAnchor_OnTrackingChanged;

                ExportAnchor();
            }
        }

        /// <summary>
        /// Exports the anchor on the objectToAnchor.
        /// </summary>
        private void ExportAnchor()
        {
            WorldAnchorTransferBatch watb = new WorldAnchorTransferBatch();
            WorldAnchor worldAnchor = objectToAnchor.GetComponent<WorldAnchor>();
            watb.AddWorldAnchor(exportingAnchorName, worldAnchor);
            WorldAnchorTransferBatch.ExportAsync(watb, WriteBuffer, ExportComplete);

            // If we started the observer to find a good anchor position, then we need to
            // stop the observer.
            if (StartedObserver)
            {
                spatialMapping.StopObserver();
                StartedObserver = false;
            }
        }

        /// <summary>
        /// If we don't have the anchor already, call this to download the anchor.
        /// </summary>
        public void WaitForAnchor()
        {
            DownloadingAnchor = networkTransmitter.RequestAndGetData();
            if (!DownloadingAnchor)
            {
                Invoke("WaitForAnchor", 0.5f);
            }
        }

        /// <summary>
        /// Attempts to attach to  an anchor by anchorName in the local store..
        /// </summary>
        /// <returns>True if it attached, false if it could not attach</returns>
        private bool AttachToCachedAnchor(string cachedAnchorName)
        {
            if (string.IsNullOrEmpty(cachedAnchorName))
            {
                Debug.Log("Ignoring empty name");
                return false;
            }

            WorldAnchorStore anchorStore = WorldAnchorManager.Instance.AnchorStore;
            Debug.Log("Looking for " + cachedAnchorName);
            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                if (ids[index] == cachedAnchorName)
                {
                    Debug.Log("Using what we have");
                    anchorStore.Load(ids[index], objectToAnchor);
                    AnchorEstablished = true;
                    return true;
                }
            }

            // Didn't find the anchor.
            return false;
        }

        /// <summary>
        /// Called when anchor data is ready.
        /// </summary>
        /// <param name="data">The data blob to import.</param>
        private void NetworkTransmitter_DataReadyEvent(byte[] data)
        {
            Debug.Log("Anchor data arrived.");
            anchorData = data;
            Debug.Log(data.Length);
            DownloadingAnchor = false;
            gotOne = true;
        }

        /// <summary>
        /// Called when a remote anchor has been deserialized
        /// </summary>
        /// <param name="status">Tracks if the import worked</param>
        /// <param name="wat">The WorldAnchorTransferBatch that has the anchor information.</param>
        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch wat)
        {
            if (status == SerializationCompletionReason.Succeeded && wat.GetAllIds().Length > 0)
            {
                Debug.Log("Import complete");

                string first = wat.GetAllIds()[0];
                Debug.Log("Anchor name: " + first);

                WorldAnchor existingAnchor = objectToAnchor.GetComponent<WorldAnchor>();
                if (existingAnchor != null)
                {
                    DestroyImmediate(existingAnchor);
                }

                WorldAnchor anchor = wat.LockObject(first, objectToAnchor);
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
                Anchor_OnTrackingChanged(anchor, anchor.isLocated);

                ImportInProgress = false;
            }
            else
            {
                // if we failed, we can simply try again.
                gotOne = true;
                Debug.Log("Import fail");
            }
        }

        private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                AnchorEstablished = true;
                WorldAnchorManager.Instance.AnchorStore.Save(AnchorName, self);
                self.OnTrackingChanged -= Anchor_OnTrackingChanged;
            }
        }

        /// <summary>
        /// Called as anchor data becomes available to export
        /// </summary>
        /// <param name="data">The next chunk of data.</param>
        private void WriteBuffer(byte[] data)
        {
            exportingAnchorBytes.AddRange(data);
        }

        /// <summary>
        /// Called when serializing an anchor is complete.
        /// </summary>
        /// <param name="status">If the serialization succeeded.</param>
        private void ExportComplete(SerializationCompletionReason status)
        {
            if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > MinTrustworthySerializedAnchorDataSize)
            {
                AnchorName = exportingAnchorName;
                anchorData = exportingAnchorBytes.ToArray();
                GenericNetworkTransmitter.Instance.SetData(anchorData);
                createdAnchor = true;
                Debug.Log("Anchor ready " + exportingAnchorBytes.Count);
                GenericNetworkTransmitter.Instance.ConfigureAsServer();
                AnchorEstablished = true;
            }
            else
            {
                Debug.Log("Create anchor failed " + status + " " + exportingAnchorBytes.Count);
                exportingAnchorBytes.Clear();
                objectToAnchor = SharedCollection.Instance.gameObject;
                DestroyImmediate(objectToAnchor.GetComponent<WorldAnchor>());
                CreateAnchor();
            }
        }

        /// <summary>
        /// Call this when a remote system has locked onto the same anchor, and we can feel fairly safe about
        /// storing the anchor in the playerprefs for the next run.
        /// </summary>
        public void AnchorFoundRemotely()
        {
            Debug.Log("Setting saved anchor to " + AnchorName);
            WorldAnchorManager.Instance.AnchorStore.Save(AnchorName, objectToAnchor.GetComponent<WorldAnchor>());
            PlayerPrefs.SetString(SavedAnchorKey, AnchorName);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Call this when a new anchor is desired.
        /// </summary>
        public void MakeNewAnchor()
        {
            // forget our cached anchor if we have one.
            if (PlayerPrefs.HasKey(SavedAnchorKey))
            {
                PlayerPrefs.DeleteKey(SavedAnchorKey);
            }

            // remove the world anchor from the object if it is there.
            WorldAnchor currentAnchor = objectToAnchor.GetComponent<WorldAnchor>();
            if (currentAnchor != null)
            {
                DestroyImmediate(currentAnchor);
            }

            // reset the anchor name so that other participants see that the current anchor is no longer valid.
            AnchorName = "";

            // and then go to create the anchor.
            CreateAnchor();
        }
#endif
    }
}
