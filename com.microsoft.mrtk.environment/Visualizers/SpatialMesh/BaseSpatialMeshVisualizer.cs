// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    public abstract class BaseSpatialMeshVisualizer : MonoBehaviour
    {
        /// <summary>
        /// Default dedicated layer for spatial awareness layer used by most components in MRTK
        /// </summary>
        public const int DefaultSpatialAwarenessLayer = 31;

        /// <summary>
        /// Configuration for the spatial mesh visualizer.
        /// </summary>
        public abstract BaseSpatialMeshVisualizerConfig Config { get; set; }

        // Reference to the active XRMeshSubsystem.
        protected XRMeshSubsystem meshSubsystem;

        /// <summary>
        /// Indicates whether or not the spatial observer is to remain in a fixed location.
        /// </summary>
        public bool IsStationaryObserver { get; set; } = false;

        /// <summary>
        /// Gets or sets the orientation of the volume in world space.
        /// </summary>
        /// <remarks>
        /// This is only used when <see cref="ObserverVolumeType"/> is set to <see cref="Microsoft.MixedReality.Toolkit.Utilities.VolumeType.UserAlignedCube"/>
        /// </remarks>
        protected Quaternion ObserverRotation { get; set; } = Quaternion.identity;

        /// <summary>
        /// Gets or sets the origin, in world space, of the observer.
        /// </summary>
        /// <remarks>
        /// <para>Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.</para>
        /// </remarks>
        protected Vector3 ObserverOrigin { get; set; } = Vector3.zero;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents { get; set; } = Vector3.one * 3f; // 3 meter sides / radius

        /// <summary>
        /// The frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval { get; set; } = 3.5f; // 3.5 seconds

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => (1 << MeshPhysicsLayer);
        private int meshPhysicsLayer = 31;
        /// <inheritdoc />
        public int MeshPhysicsLayer
        {
            get { return meshPhysicsLayer; }
            set
            {
                if ((value < 0) || (value > 31))
                {
                    Debug.LogError("Specified MeshPhysicsLayer is out of bounds. Please set a value between 0 and 31, inclusive.");
                    return;
                }

                meshPhysicsLayer = value;
                ApplyUpdatedPhysicsLayer();
            }
        }

        /// <inheritdoc />
        public bool RecalculateNormals { get; set; } = true;

        /// <inheritdoc />
        public int TrianglesPerCubicMeter { get; set; } = 0;

        private SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;

        /// <inheritdoc />
        public SpatialAwarenessMeshDisplayOptions DisplayOption
        {
            get { return displayOption; }
            set
            {
                displayOption = value;
                if (Application.isPlaying)
                {
                    ApplyUpdatedMeshDisplayOption(displayOption);
                }
            }
        }

        private SpatialMeshLevelOfDetail levelOfDetail = SpatialMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public SpatialMeshLevelOfDetail LevelOfDetail
        {
            get { return levelOfDetail; }
            set
            {
                if (value != SpatialMeshLevelOfDetail.Custom)
                {
                    TrianglesPerCubicMeter = LookupTriangleDensity(value);
                }

                levelOfDetail = value;
            }
        }

        /// <summary>
        /// The backing field for Meshes, to allow the mesh observer implementation to track its meshes.
        /// </summary>
        protected readonly Dictionary<int, SpatialAwarenessMeshObject> meshes = new Dictionary<int, SpatialAwarenessMeshObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes => new Dictionary<int, SpatialAwarenessMeshObject>(meshes);

        private Material occlusionMaterial = null;

        /// <inheritdoc />
        public Material OcclusionMaterial
        {
            get { return occlusionMaterial; }
            set
            {
                if (value != occlusionMaterial)
                {
                    occlusionMaterial = value;

                    if (Application.isPlaying && DisplayOption == SpatialAwarenessMeshDisplayOptions.Occlusion)
                    {
                        ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions.Occlusion);
                    }
                }
            }
        }

        private PhysicMaterial physicsMaterial;

        public PhysicMaterial PhysicsMaterial
        {
            get { return physicsMaterial; }
            set
            {
                if (value != physicsMaterial)
                {
                    physicsMaterial = value;
                    ApplyUpdatedMeshPhysics();
                }
            }
        }

        private Material visibleMaterial = null;

        /// <inheritdoc />
        public Material VisibleMaterial
        {
            get { return visibleMaterial; }
            set
            {
                if (value != visibleMaterial)
                {
                    visibleMaterial = value;

                    if (Application.isPlaying && DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible)
                    {
                        ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions.Visible);
                    }
                }
            }
        }

        public void OnEnable()
        {
            meshSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<XRMeshSubsystem>();
            if (meshSubsystem == null)
            {
                Debug.LogWarning("Mesh subsystem not found (or is not running). Waiting until it is.");
                StartCoroutine(EnableWhenSubsystemAvailable());
            }
            else
            {
                Debug.Log("Mesh subsystem found");
                ReadConfig();
                ConfigureObserverVolume();
            }
        }

        private IEnumerator EnableWhenSubsystemAvailable()
        {
            yield return new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<XRMeshSubsystem>() != null);
            Debug.Log("Mesh subsystem now found");
            OnEnable();
        }

        /// <summary>
        /// Applies config data from stored config scriptableObject to local variables
        /// </summary>
        protected virtual void ReadConfig()
        {
            if (Config == null)
            {
                Debug.LogError($"Spatial mesh visualization requires a configuration profile to run properly.");
                return;
            }

            // Spatial observer options
            IsStationaryObserver = Config.IsStationaryObserver;
            ObservationExtents = Config.ObservationExtents;
            UpdateInterval = Config.UpdateInterval;

            // Mesh visualization options
            DisplayOption = Config.DisplayOption;
            TrianglesPerCubicMeter = Config.TrianglesPerCubicMeter; // Set this before LevelOfDetail so it doesn't overwrite in the non-Custom case
            LevelOfDetail = Config.LevelOfDetail;
            MeshPhysicsLayer = Config.MeshPhysicsLayer;
            OcclusionMaterial = Config.OcclusionMaterial;
            PhysicsMaterial = Config.PhysicsMaterial;
            RecalculateNormals = Config.RecalculateNormals;
            VisibleMaterial = Config.VisibleMaterial;
        }

        private static readonly ProfilerMarker ApplyUpdatedMeshDisplayOptionPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.ApplyUpdatedMeshDisplayOption");

        /// <summary>
        /// Applies the mesh display option to existing meshes when modified at runtime.
        /// </summary>
        /// <param name="option">The <see cref="SpatialAwarenessMeshDisplayOptions"/> value to be used to determine the appropriate material.</param>
        protected virtual void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            using (ApplyUpdatedMeshDisplayOptionPerfMarker.Auto())
            {
                bool enable = (option != SpatialAwarenessMeshDisplayOptions.None);

                foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
                {
                    if (meshObject?.Renderer == null) { continue; }

                    if (enable)
                    {
                        meshObject.Renderer.sharedMaterial = (option == SpatialAwarenessMeshDisplayOptions.Visible) ?
                            VisibleMaterial :
                            OcclusionMaterial;
                    }

                    meshObject.Renderer.enabled = enable;
                }
            }
        }

        private static readonly ProfilerMarker ApplyUpdatedMeshPhysicsPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.ApplyUpdatedMeshPhysics");

        /// <summary>
        /// Applies the physical material to existing meshes when modified at runtime.
        /// </summary>
        protected virtual void ApplyUpdatedMeshPhysics()
        {
            using (ApplyUpdatedMeshPhysicsPerfMarker.Auto())
            {
                foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
                {
                    if (meshObject?.Collider == null) { continue; }
                    meshObject.Collider.material = PhysicsMaterial;
                }
            }
        }

        protected virtual int LookupTriangleDensity(SpatialMeshLevelOfDetail levelOfDetail)
        {
            // For non-custom levels, the enum value is the appropriate triangles per cubic meter.
            int level = (int)levelOfDetail;
            if (meshSubsystem != null)
            {
                if (levelOfDetail == SpatialMeshLevelOfDetail.Unlimited)
                {
                    meshSubsystem.meshDensity = 1;
                }
                else
                {
                    meshSubsystem.meshDensity = level / (float)SpatialMeshLevelOfDetail.Fine; // For now, map Coarse to 0.0 and Fine to 1.0
                }
            }
            return level;
        }
        private static readonly ProfilerMarker ApplyUpdatedPhysicsLayerPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.ApplyUpdatedPhysicsLayer");

        /// <summary>
        /// Updates the mesh physics layer for current mesh observations.
        /// </summary>
        protected virtual void ApplyUpdatedPhysicsLayer()
        {
            using (ApplyUpdatedPhysicsLayerPerfMarker.Auto())
            {
                foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
                {
                    if (meshObject?.GameObject == null) { continue; }

                    meshObject.GameObject.layer = MeshPhysicsLayer;
                }
            }
        }
        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.Update");

        public void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                UpdateObserver();
            }
        }

        /// <summary>
        /// A queue of MeshId that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<MeshId> meshWorkQueue = new Queue<MeshId>();

        private readonly List<MeshInfo> meshInfos = new List<MeshInfo>();

        /// <summary> 
        /// To prevent too many meshes from being generated at the same time, we will 
        /// only request one mesh to be created at a time. This variable will track 
        /// if a mesh creation request is in flight.
        /// </summary> 
        private SpatialAwarenessMeshObject outstandingMeshObject = null;

        /// <summary>
        /// When surfaces are replaced or removed, rather than destroying them, we'll keep
        /// one as a spare for use in outstanding mesh requests. That way, we'll have fewer
        /// game object create/destroy cycles, which should help performance.
        /// </summary>
        private SpatialAwarenessMeshObject spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        private static readonly ProfilerMarker ClearObservationsPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.ClearObservations");
        public void ClearObservations()
        {
            using (ClearObservationsPerfMarker.Auto())
            {
                IReadOnlyList<int> observations = new List<int>(Meshes.Keys);
                foreach (int meshId in observations)
                {
                    RemoveMeshObject(meshId);
                }
            }
        }

        #region Helpers

        private static readonly ProfilerMarker UpdateObserverPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.UpdateObserver");

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (meshSubsystem == null) { return; }

            using (UpdateObserverPerfMarker.Auto())
            {
                // Only update the observer if it is running.
                if (outstandingMeshObject == null)
                {
                    // If we have a mesh to work on...
                    if (meshWorkQueue.Count > 0)
                    {
                        // We're using a simple first-in-first-out rule for requesting meshes, but a more sophisticated algorithm could prioritize
                        // the queue based on distance to the user or some other metric.
                        RequestMesh(meshWorkQueue.Dequeue());
                    }
                    // If enough time has passed since the previous observer update...
                    else if (Time.time - lastUpdated >= UpdateInterval)
                    {
                        ObserverRotation = CameraCache.Main.transform.rotation;

                        // Update the observer location if it is not stationary
                        if (!IsStationaryObserver)
                        {
                            ObserverOrigin = CameraCache.Main.transform.position;
                        }

                        // The application can update the observer volume at any time, make sure we are using the latest.
                        ConfigureObserverVolume();

                        if (meshSubsystem.TryGetMeshInfos(meshInfos))
                        {
                            UpdateMeshes(meshInfos);
                        }

                        lastUpdated = Time.time;
                    }
                }
            }
        }

        private static readonly ProfilerMarker RequestMeshPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.RequestMesh");

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="meshId">ID of the mesh to bake.</param>
        private void RequestMesh(MeshId meshId)
        {
            using (RequestMeshPerfMarker.Auto())
            {
                string meshName = ("SpatialMesh - " + meshId);

                SpatialAwarenessMeshObject newMesh;

                if (spareMeshObject == null)
                {
                    newMesh = SpatialAwarenessMeshObject.Create(
                        null,
                        MeshPhysicsLayer,
                        meshName,
                        meshId.GetHashCode());
                }
                else
                {
                    newMesh = spareMeshObject;
                    spareMeshObject = null;

                    newMesh.GameObject.name = meshName;
                    newMesh.Id = meshId.GetHashCode();
                    newMesh.GameObject.SetActive(true);
                }

                meshSubsystem.GenerateMeshAsync(
                    meshId,
                    newMesh.Filter.mesh,
                    newMesh.Collider,
                    MeshVertexAttributes.Normals,
                    (MeshGenerationResult meshGenerationResult) => MeshGenerationAction(meshGenerationResult));
                outstandingMeshObject = newMesh;
            }
        }

        private static readonly ProfilerMarker RemoveMeshObjectPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.RemoveMeshObject");

        /// <summary>
        /// Removes the <see cref="SpatialAwareness.SpatialAwarenessMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int id)
        {
            using (RemoveMeshObjectPerfMarker.Auto())
            {
                SpatialAwarenessMeshObject mesh;
                if (meshes.TryGetValue(id, out mesh))
                {
                    // Remove the mesh object from the collection.
                    meshes.Remove(id);

                    // Reclaim the mesh object for future use.
                    ReclaimMeshObject(mesh);

                    // Send the mesh removed event
                    // meshEventData.Initialize(this, id, null);
                    // Service?.HandleEvent(meshEventData, OnMeshRemoved);
                }
            }
        }

        private static readonly ProfilerMarker ReclaimMeshObjectPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.ReclaimMeshObject");

        /// <summary>
        /// Reclaims the <see cref="SpatialAwareness.SpatialAwarenessMeshObject"/> to allow for later reuse.
        /// </summary>
        protected void ReclaimMeshObject(SpatialAwarenessMeshObject availableMeshObject)
        {
            using (ReclaimMeshObjectPerfMarker.Auto())
            {
                if (spareMeshObject == null)
                {
                    // Cleanup the mesh object.
                    // Do not destroy the game object, destroy the meshes.
                    SpatialAwarenessMeshObject.Cleanup(availableMeshObject, false);

                    availableMeshObject.GameObject.name = "Unused Spatial Mesh";
                    availableMeshObject.GameObject.SetActive(false);

                    spareMeshObject = availableMeshObject;
                }
                else
                {
                    // Cleanup the mesh object.
                    // Destroy the game object, destroy the meshes.
                    SpatialAwarenessMeshObject.Cleanup(availableMeshObject);
                }
            }
        }

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        protected abstract void ConfigureObserverVolume();

        private static readonly ProfilerMarker UpdateMeshesPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.UpdateMeshes");

        /// <summary>
        /// Updates meshes based on the result of the MeshSubsystem.TryGetMeshInfos method.
        /// </summary>
        private void UpdateMeshes(List<MeshInfo> meshInfos)
        {
            // if (!IsRunning) { return; }

            using (UpdateMeshesPerfMarker.Auto())
            {
                foreach (MeshInfo meshInfo in meshInfos)
                {
                    switch (meshInfo.ChangeState)
                    {
                        case MeshChangeState.Added:
                        case MeshChangeState.Updated:
                            meshWorkQueue.Enqueue(meshInfo.MeshId);
                            break;

                        case MeshChangeState.Removed:
                            RemoveMeshObject(meshInfo.MeshId.GetHashCode());
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker MeshGenerationActionPerfMarker =
            new ProfilerMarker("[MRTK] BaseSpatialMeshVisualizer.MeshGenerationAction");

        private void MeshGenerationAction(MeshGenerationResult meshGenerationResult)
        {
            // if (!IsRunning) { return; }

            using (MeshGenerationActionPerfMarker.Auto())
            {
                if (outstandingMeshObject == null)
                {
                    Debug.LogWarning($"MeshGenerationAction called for mesh id {meshGenerationResult.MeshId} while no request was outstanding.");
                    return;
                }

                switch (meshGenerationResult.Status)
                {
                    case MeshGenerationStatus.InvalidMeshId:
                    case MeshGenerationStatus.Canceled:
                    case MeshGenerationStatus.UnknownError:
                        outstandingMeshObject = null;
                        break;
                    case MeshGenerationStatus.Success:
                        // Since there is only one outstanding mesh object, update the id to match
                        // the one received after baking.
                        SpatialAwarenessMeshObject meshObject = outstandingMeshObject;
                        meshObject.Id = meshGenerationResult.MeshId.GetHashCode();
                        outstandingMeshObject = null;

                        // Check to see if this is a new or updated mesh.
                        bool isMeshUpdate = meshes.ContainsKey(meshObject.Id);

                        // We presume that if the display option is not occlusion, that we should 
                        // default to the visible material. 
                        // Note: We check explicitly for a display option of none later in this method.
                        Material material = (DisplayOption == SpatialAwarenessMeshDisplayOptions.Occlusion) ?
                            OcclusionMaterial : VisibleMaterial;

                        // If this is a mesh update, we want to preserve the mesh's previous material.
                        material = isMeshUpdate ? meshes[meshObject.Id].Renderer.sharedMaterial : material;

                        // Apply the appropriate material.
                        meshObject.Renderer.sharedMaterial = material;

                        // Recalculate the mesh normals if requested.
                        if (RecalculateNormals)
                        {
                            meshObject.Filter.sharedMesh.RecalculateNormals();
                        }

                        // Check to see if the display option is set to none. If so, we disable
                        // the renderer.
                        meshObject.Renderer.enabled = (DisplayOption != SpatialAwarenessMeshDisplayOptions.None);

                        // Set the physics material
                        if (meshObject.Renderer.enabled)
                        {
                            meshObject.Collider.material = PhysicsMaterial;
                        }

                        // Add / update the mesh to our collection
                        if (isMeshUpdate)
                        {
                            // Reclaim the old mesh object for future use.
                            ReclaimMeshObject(meshes[meshObject.Id]);
                            meshes.Remove(meshObject.Id);
                        }
                        meshes.Add(meshObject.Id, meshObject);

                        // This is important. We need to preserve the mesh's local transform here, not its global pose.
                        // Think of it like this. If we set the camera's coordinates 3 meters to the left, the physical camera
                        // hasn't moved, only its coordinates have changed. Likewise, the physical room hasn't moved (relative to
                        // the physical camera), so we also want to set its coordinates 3 meters to the left.
                        Transform meshObjectParent = transform;
                        meshObject.GameObject.transform.SetParent(meshObjectParent, false);

                        // meshEventData.Initialize(this, meshObject.Id, meshObject);
                        // if (isMeshUpdate)
                        // {
                        //     Service?.HandleEvent(meshEventData, OnMeshUpdated);
                        // }
                        // else
                        // {
                        //     Service?.HandleEvent(meshEventData, OnMeshAdded);
                        // }
                        break;
                }
            }
        }

        #endregion Helpers
    }
}
