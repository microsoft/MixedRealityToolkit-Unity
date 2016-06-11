// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA;
using System.Collections.Generic;

public abstract class SMBaseAbstract : MonoBehaviour
{
    /// <summary>
    /// Toggles whether to stop requesting changes to observed region.
    /// </summary>
    public bool FreezeMeshUpdates;

    /// <summary>
    /// Use a sphere as the bounding volume instead of a cube
    /// </summary>
    public bool UseSphereBounds = false;

    /// <summary>
    /// The extents of the observation volume.
    /// </summary>
    public Vector3 Extents = Vector3.one * 10.0f;

    /// <summary>
    /// The radius of the observation sphere volume.
    /// </summary>
    public float SphereRadius = 10.0f;

    /// <summary>
    /// The Level of Detail for the mesh
    /// </summary>
    public enum MeshLevelOfDetail { High = 0, Medium = 1, Low = 2 }


    /// <summary>
    /// Whether or not the Level of Detail has been set to know to use _levelOfDetail or the default
    /// </summary>
    [SerializeField]
    private bool _levelOfDetailSet = false;
    /// <summary>
    /// The Level of Detail to use if set. If not set, we will use the default from GetDefaultLevelOfDetail
    /// </summary>
    [SerializeField]
    private MeshLevelOfDetail _levelOfDetail;
    /// <summary>
    /// The level of detail to use in the mesh
    /// </summary>
    public MeshLevelOfDetail LevelOfDetail
    {
        get
        {
            if (_levelOfDetailSet)
            {
                return _levelOfDetail;
            }
            return GetDefaultLevelOfDetail();
        }
        set
        {
            _levelOfDetail = value;
            _levelOfDetailSet = true;
        }
    }

    /// <summary>
    /// An array that converts the logical level of detail to the Triangles per Cubic Meter needed by the API
    /// </summary>
    protected int[] LevelOfDetailToTPCM =
    {
        2000, 750, 100
    };
    /// <summary>
    /// The current Level of Detail setting once converted to Triangles per Cubic Meter
    /// </summary>
    protected int TrianglesPerCubicMeter { get { return LevelOfDetailToTPCM[(int)LevelOfDetail]; } }

    /// <summary>
    /// How long to wait (in sec) between Spatial Mapping updates.
    /// </summary>
    public float TimeBetweenUpdates = 2.5f;

    /// <summary>
    /// The SurfaceObserver we will use to get data about the physical surroundings
    /// </summary>
    protected SurfaceObserver surfaceObserver;
    /// <summary>
    /// The collection of all of the active meshes indexed by SurfaceId.
    /// 
    /// The mesh itself can be accessed by getting the MeshFilter off of the GameObject
    /// </summary>
    public Dictionary<SurfaceId, GameObject> SpatialMeshObjects = new Dictionary<SurfaceId, GameObject>();

    /// <summary>
    /// Set this value to change how quickly objects will be actually removed from the scene
    /// 
    /// When set to less than 1, objects will immediately be removed even if only transiently removed.
    /// When 1 or greater, at least that number of update intervals will occur before objects will actually leave the scene.
    /// Update intervals will not be processed if the object is within an observed region and the user is > 10.0 from the center
    /// </summary>
    public int NumUpdatesBeforeRemoval = 10;

    /// <summary>
    /// The struct that will be used when caching meshes marked for deletion before the NumUpdatesBeforeRemoval has been reached
    /// </summary>
    public struct RemovedSurfaceHolder
    {
        /// <summary>
        /// The number of remaining updates before the referenced mesh will be removed
        /// </summary>
        public int updatesBeforeRemoval;
        /// <summary>
        /// The GameObject that holds the MeshFilter as well as any other components associated with this mesh (colliders, renderers, etc.)
        /// </summary>
        public GameObject gameObject;
        /// <summary>
        /// The identifier of this surface
        /// </summary>
        public SurfaceId id;
        /// <summary>
        /// Whether the MeshCollider was present and enabled when this object was marked for removal.
        /// 
        /// If this object is re-added before it is Destroyed, the MeshCollider.enabled state will be restored assuming the MeshCollider still exists
        /// </summary>
        public bool wasMeshColliderEnabled;
        /// <summary>
        /// Whether the MeshRenderer was present and enabled when this object was marked for removal.
        /// 
        /// If this object is re-added before it is Destroyed, the MehsRenderer.enabled state will be restored assuming the MeshRenderer still exists
        /// </summary>
        public bool wasMeshRendererEnabled;

        /// <summary>
        /// Decrements the number of updates remaining by one
        /// </summary>
        public void DecrementUpdatesRemaining()
        {
            if (updatesBeforeRemoval > 0)
            {
                updatesBeforeRemoval--;
            }
        }

        /// <summary>
        /// Updates whether to enable the collider if the mesh is re-added
        /// </summary>
        /// <param name="colliderEnabled">Whether or not to re-enable the collider if the mesh is re-added</param>
        public void SetColliderEnabled(bool colliderEnabled)
        {
            wasMeshColliderEnabled = colliderEnabled;
        }

        /// <summary>
        /// Updates whether to enable the renderer if the mesh is re-added
        /// </summary>
        /// <param name="rendererEnabled">Whether or not to re-enable the renderer if the mesh is re-added</param>
        public void SetRendererEnabled(bool rendererEnabled)
        {
            wasMeshRendererEnabled = rendererEnabled;
        }

        /// <summary>
        /// Constructor for the struct taking all the parameters in
        /// </summary>
        /// <param name="updatesBeforeRemoval">The number of updates which will pass before this object is removed.</param>
        /// <param name="gameObject">The GameObject owner of all of the mesh components</param>
        /// <param name="id">The SurfaceId identifying this surface</param>
        /// <param name="wasMeshColliderEnabled">Whether or not a MeshCollider was present and enabled</param>
        /// <param name="wasMeshRendererEnabled">Whether or not a MeshRenderer was present and enabled</param>
        public RemovedSurfaceHolder(int updatesBeforeRemoval, GameObject gameObject, SurfaceId id, bool wasMeshColliderEnabled, bool wasMeshRendererEnabled)
        {
            this.updatesBeforeRemoval = updatesBeforeRemoval;
            this.gameObject = gameObject;
            this.id = id;
            this.wasMeshColliderEnabled = wasMeshColliderEnabled;
            this.wasMeshRendererEnabled = wasMeshRendererEnabled;
        }
    }

    /// <summary>
    /// The collection of meshes that may have been removed by the system but are being cached currently
    /// 
    /// These objects may have been removed from the system due to being too far away or due to tracking loss as well
    /// actually not needing to exist. To disable this behavior, set [SurfaceObserver].NumUpdatesBeforeRemoval = 0 or manually clear this dictionary
    /// Surfaces in this collection will move back to the SpatialMeshObjects if they return to the scene before the number of update intervals has elapsed
    /// </summary>
    public Dictionary<SurfaceId, RemovedSurfaceHolder> RemovedMeshObjects = new Dictionary<SurfaceId, RemovedSurfaceHolder>();

    /// <summary>
    /// Whether or not the bake meshes (generate a collider) when processing
    /// </summary>
    protected bool bakeMeshes = false;

    /// <summary>
    /// The bounds described by the extents of the observer
    /// 
    /// This property is used to determine if a mesh is contained within the observed bounds when not using a sphere observed region
    /// </summary>
    protected Bounds bounds;

    /// <summary>
    /// Standard initialization method creating our properties
    /// </summary>
    protected virtual void Start()
    {
        surfaceObserver = new SurfaceObserver();
        UpdateSurfaceObserverPosition();
        StartCoroutine(UpdateLoop());
        bounds = new Bounds(transform.position, Extents);
    }

    /// <summary>
    /// A helper to correctly update the bounding volume for the SurfaceObserver
    /// </summary>
    private void UpdateSurfaceObserverPosition()
    {
        if (UseSphereBounds)
        {
            surfaceObserver.SetVolumeAsSphere(transform.position, SphereRadius);
        }
        else
        {
            surfaceObserver.SetVolumeAsAxisAlignedBox(transform.position, Extents);
            bounds.center = transform.position;
            bounds.extents = Extents;
        }
    }

    /// <summary>
    /// Standard method called when the component is destroyed.
    /// 
    /// When this component is destroyed, we clean up all of our tracked GameObjects
    /// </summary>
    protected virtual void OnDestroy()
    {
        foreach (GameObject go in SpatialMeshObjects.Values)
        {
            if (go != null)
            {
                FinalDestroy(go);
            }
        }
        SpatialMeshObjects.Clear();

        foreach (RemovedSurfaceHolder rsh in RemovedMeshObjects.Values)
        {
            if (rsh.gameObject != null)
            {
                FinalDestroy(rsh.gameObject);
            }
        }
        RemovedMeshObjects.Clear();
    }

    /// <summary>
    /// Standard method called when the component is disabled
    /// 
    /// When this component is disabled, we disable all of the tracked GameObjects
    /// </summary>
    protected virtual void OnDisable()
    {
        foreach (GameObject go in SpatialMeshObjects.Values)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }

        foreach (RemovedSurfaceHolder rsh in RemovedMeshObjects.Values)
        {
            if (rsh.gameObject != null)
            {
                rsh.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Standard method called when the component is enabled
    /// 
    /// When this component is enabled, we enable all of the tracked GameObjects
    /// </summary>
    protected virtual void OnEnable()
    {
        foreach (GameObject go in SpatialMeshObjects.Values)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }

        foreach (RemovedSurfaceHolder rsh in RemovedMeshObjects.Values)
        {
            if (rsh.gameObject != null)
            {
                rsh.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Handler for when the SurfaceObserver completes RequestMeshAsync
    /// 
    /// The base class defines this function to make it easier for subclasses to modify meshes upon completion without needing to handle the actual processing
    /// </summary>
    /// <param name="bakedData">The processed SurfaceData</param>
    /// <param name="outputWritten">Whether or not output was written</param>
    /// <param name="elapsedBakeTimeSeconds">The time in seconds it took to request and populate the mesh</param>
    protected virtual void SurfaceObserver_OnDataReady(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
    {
        // Passthrough
    }

    /// <summary>
    /// The Coroutine which actually updates the meshes and processes our removed mesh's list
    /// 
    /// This Coroutine runs forever and checks that the component is enabled and FreezeMeshUpdates has not been specified. If either are not true, we do nothing here and continue waiting.
    /// </summary>
    /// <returns>The WaitForSeconds to continue to wait</returns>
    protected IEnumerator UpdateLoop()
    {
        var wait = new WaitForSeconds(TimeBetweenUpdates);
        while (true)
        {
            if (enabled && !FreezeMeshUpdates)
            {
                // Make sure we ProcessRemoveList after Update because it might add some surfaces back
                UpdateManager.AddToUpdateQueue(() => { surfaceObserver.Update(SurfaceObserver_OnSurfaceChanged); ProcessRemoveList(); });
            }
            yield return wait;
        }
    }

    /// <summary>
    /// Standard Update loop
    /// 
    /// During our Update, we update the observed volume for the SurfaceObserver and ensure we do one step of processing
    /// </summary>
    protected virtual void Update()
    {
        UpdateSurfaceObserverPosition();
        UpdateManager.DoProcessingForFrame();
    }

    /// <summary>
    /// Handler for calling SurfaceObserver.Update which will then handle the changes
    /// 
    /// The actual changes will be handled via HandleAdd (for SurfaceChange.Added and SurfaceChange.Updated) and HandleDelete (for SurfaceChange.Removed)
    /// </summary>
    /// <param name="surfaceId">The identifier of the surface in question</param>
    /// <param name="changeType">What type of change this is (add, update, or remove)</param>
    /// <param name="bounds">The bounds of the mesh</param>
    /// <param name="updateTime">The time the update occurred</param>
    protected virtual void SurfaceObserver_OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, System.DateTime updateTime)
    {
        switch (changeType)
        {
            case SurfaceChange.Added:
            case SurfaceChange.Updated:
                HandleAdd(surfaceId, updateTime, bakeMeshes);
                break;
            case SurfaceChange.Removed:
                HandleDelete(surfaceId);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Handles cleaning up a known mesh or moving it to the RemovedMeshObjects list
    /// 
    /// If NumUpdatesBeforeRemoval < 1, the mesh will immediately be destroyed upon removal
    /// Else we will create a new RemoveSurfaceHolder and add that to the RemovedMeshObjects list to cache the mesh until we believe it should actually be removed
    /// </summary>
    /// <param name="surfaceId"></param>
    protected virtual void HandleDelete(SurfaceId surfaceId)
    {
        GameObject obj = SpatialMeshObjects[surfaceId];
        SpatialMeshObjects.Remove(surfaceId);

        if (NumUpdatesBeforeRemoval < 1)
        {
            FinalDestroy(obj);
        }
        else if (obj != null)
        {
            bool shouldDisable = !ShouldBeActiveWhileRemoved(obj);

            MeshCollider collider = obj.GetComponent<MeshCollider>();
            bool collisionsEnabled = (collider != null && collider.enabled);
            if (collisionsEnabled && shouldDisable)
            {
                collider.enabled = false;
            }

            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            bool rendererEnabled = (mr != null && mr.enabled);
            if (rendererEnabled && shouldDisable)
            {
                mr.enabled = false;
            }
            RemovedSurfaceHolder holder = new RemovedSurfaceHolder(NumUpdatesBeforeRemoval + 1 /*+1 because we are going to process it now*/, obj, surfaceId, collisionsEnabled, rendererEnabled);
            RemovedMeshObjects.Add(surfaceId, holder);
        }
    }

    /// <summary>
    /// Destroys the provided GameObject and The MeshFilter.mesh if present
    /// </summary>
    /// <param name="surfaceObject">The GameObject to destroy</param>
    protected void FinalDestroy(GameObject surfaceObject)
    {
        if (surfaceObject != null)
        {
            MeshFilter filter = surfaceObject.GetComponent<MeshFilter>();
            if (filter != null)
            {
                if (filter.mesh != null)
                {
                    Destroy(filter.mesh);
                }
            }

            Destroy(surfaceObject);
        }
    }

    /// <summary>
    /// Handles when a surface is added or updated by either creating the needed components or finding them in either the RemovedMeshObjects collection or the SpatialMeshObjects collection
    /// 
    /// If a surface is contained in the RemovedMeshObjects collection, the enabled state for its MeshCollider or MeshRenderer is restored if appropriate. The GameObject will be moved into SpatialMeshObjects, and the RemovedMeshHolder will be removed from RemovedMeshObject
    /// If a surface is not found in either collection, a new GameObject will be created for it and it will be added to SpatialMeshObjects indexed by its id.
    /// After the GameObject is handled as appropriately, SurfaceObserver.RequestMeshAsync will be called for the appropriate settings.
    /// </summary>
    /// <param name="surfaceId">The id of the surface that was added or updated</param>
    /// <param name="updateTime">The time at which the surface was modified</param>
    /// <param name="bake">Whether or not this component should request to back a collider for the surface</param>
    protected virtual void HandleAdd(SurfaceId surfaceId, System.DateTime updateTime, bool bake)
    {
        if (RemovedMeshObjects.ContainsKey(surfaceId))
        {
            SpatialMeshObjects[surfaceId] = RemovedMeshObjects[surfaceId].gameObject;
            if (RemovedMeshObjects[surfaceId].wasMeshColliderEnabled)
            {
                MeshCollider collider = SpatialMeshObjects[surfaceId].GetComponent<MeshCollider>();
                collider.enabled = true;
            }
            if (RemovedMeshObjects[surfaceId].wasMeshRendererEnabled)
            {
                MeshRenderer mr = SpatialMeshObjects[surfaceId].GetComponent<MeshRenderer>();
                mr.enabled = true;
            }

            RemovedMeshObjects.Remove(surfaceId);
        }
        else if (!SpatialMeshObjects.ContainsKey(surfaceId))
        {
            SpatialMeshObjects[surfaceId] = new GameObject("spatial-mapping-" + surfaceId.handle);
            SpatialMeshObjects[surfaceId].transform.parent = this.transform;
        }
        GameObject target = SpatialMeshObjects[surfaceId];
        SurfaceData sd = new SurfaceData(
                //the surface id returned from the system
                surfaceId,
                //the mesh filter that is populated with the spatial mapping data for this mesh
                (target.GetComponent<MeshFilter>() == null) ? target.AddComponent<MeshFilter>() : target.GetComponent<MeshFilter>(),
                //the world anchor used to position the spatial mapping mesh in the world
                (target.GetComponent<WorldAnchor>() == null) ? target.AddComponent<WorldAnchor>() : target.GetComponent<WorldAnchor>(),
                //the mesh collider that is populated with collider data for this mesh, if true is passed to bakeMeshes below
                (target.GetComponent<MeshCollider>() == null) ? target.AddComponent<MeshCollider>() : target.GetComponent<MeshCollider>(),
                //triangles per cubic meter requested for this mesh
                TrianglesPerCubicMeter,
                //bakeMeshes - if true, the mesh collider is populated, if false, the mesh collider is empty.
                bake
                );
        surfaceObserver.RequestMeshAsync(sd, SurfaceObserver_OnDataReady);
    }

    /// <summary>
    /// Iterates through the RemovedMeshObjects list and decrements the updates remaining and removes objects if appropriate
    /// 
    /// The criteria for decrementing the update count and removing the entry if appropriate is either
    ///  - The mesh is not within the observed bounds (so in theory this observer does not actually care to observe it
    ///  - Or the main camera (user) is within 10m of the surface (if the user is that close to the mesh, the HoloLens is likely actively reporting on that space)
    /// </summary>
    protected virtual void ProcessRemoveList()
    {
        foreach (var kvp in RemovedMeshObjects)
        {
            // Only tick the count if either the object is not in the observed bounds or the user is within 10m of it
            if (!BoundsContains(kvp.Value.gameObject.transform.position) ||
                    Vector3.Distance(kvp.Value.gameObject.transform.position, Camera.main.transform.position) < 10.0f)
            {
                kvp.Value.DecrementUpdatesRemaining();
                if (kvp.Value.updatesBeforeRemoval < 1)
                {
                    FinalDestroy(kvp.Value.gameObject);
                    RemovedMeshObjects.Remove(kvp.Key);
                }
            }
        }
    }

    /// <summary>
    /// Gets the default Level of Detail. Subclasses can override to change the default Level of Detail.
    /// </summary>
    /// <returns>MeshLevelOfDetail.Low</returns>
    protected virtual MeshLevelOfDetail GetDefaultLevelOfDetail()
    {
        return MeshLevelOfDetail.Low;
    }

    /// <summary>
    /// Helper method to determine if the bounds of this component contains the provided coordinate
    /// </summary>
    /// <param name="position">The position to test if it is within the current bounds of the component</param>
    /// <returns>Whether or not the position is contained within the current bounds of this component</returns>
    protected bool BoundsContains(Vector3 position)
    {
        if (UseSphereBounds)
        {
            return (Vector3.Distance(transform.position, position) <= SphereRadius);
        }
        else
        {
            return bounds.Contains(position);
        }
    }

    /// <summary>
    /// Helper to determine if the GameObject should continue to render and/or have physics even if marked for removal by the system
    /// 
    /// An object should render and/or have physics if it is within the bounds of the observer and not parented to the camera
    /// </summary>
    /// <param name="go">The GameObject in question</param>
    /// <returns>Whether or not the GameObject should remain active even if marked for removal by the system</returns>
    protected bool ShouldBeActiveWhileRemoved(GameObject go)
    {
        // If this is parented to the main camera, we have to disable it regardless of it being within bounds.
        // If we don't, the mesh could appear to be locked to the user's head which can make them feel sick.
        bool parentedToCamera = Camera.main.gameObject == go;
        Transform parent = go.transform.parent;
        while (!parentedToCamera && parent != null)
        {
            if (parent.gameObject == Camera.main.gameObject)
            {
                parentedToCamera = true;
                break;
            }
            parent = parent.parent;
        }

        bool containedSurface = BoundsContains(go.transform.position);

        return containedSurface && !parentedToCamera;
    }

    /// <summary>
    /// Helper static class to ensure only one SurfaceObserver is processed per frame
    /// </summary>
    protected static class UpdateManager
    {
        /// <summary>
        /// The queue of pending updates to process
        /// </summary>
        private static Queue<System.Action> updatesToProcess = new Queue<System.Action>();
        /// <summary>
        /// The Time.frameCount of the last frame where we executed an update
        /// 
        /// If the current Time.frameCount != lastFrameProcessed, calling DoProcessingForFrame will execute the next update
        /// If the current Time.frameCount == lastFrameProcessed, calling DoProcessingForFrame will do nothing
        /// </summary>
        private static int lastFrameProcessed = -1;

        /// <summary>
        /// Queues the provided command to be processed on its own frame
        /// </summary>
        /// <param name="command">The command to be processed</param>
        public static void AddToUpdateQueue(System.Action command)
        {
            updatesToProcess.Enqueue(command);
        }

        /// <summary>
        /// Executes the next update if no updates have been executed on this frame or does nothing
        /// </summary>
        public static void DoProcessingForFrame()
        {
            if (lastFrameProcessed != Time.frameCount && updatesToProcess.Count > 0)
            {
                System.Action update = updatesToProcess.Dequeue();
                update();
                lastFrameProcessed = Time.frameCount;
            }
        }
    }
}
