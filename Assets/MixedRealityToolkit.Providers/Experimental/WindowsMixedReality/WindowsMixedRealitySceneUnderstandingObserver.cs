// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using UnityEngine;
using UnityEngine.Assertions;
using Microsoft.MixedReality.SceneUnderstanding;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.Extensions;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using Microsoft.Windows.Perception.Spatial.Preview;
using Microsoft.Windows.Perception.Spatial;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Experimental.SpatialAwareness
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Scene Understanding Observer",
        "Experimental/Profiles/DefaultSceneUnderstandingObserverProfile.asset",
        "MixedRealityToolkit.SDK")]
    public class WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver : BaseSpatialSceneObserver, IMixedRealityOnDemandObserver, IWindowsMixedRealitySceneUnderstanding
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        {
            ReadProfile();
        }

        #region IMixedRealityToolkit implementation

        /// <inheritdoc />
        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

        public override void Initialize()
        {
            //Debug.Log("Initialize");
            Debug.Log($"Initialize() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            base.Initialize();
            MeshExtensions.CreateMeshFromQuad(normalizedQuadMesh, 1, 1);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            //Debug.Log("Enable()");

            StartUpdateTimers();

            shouldAutoStart = StartupBehavior == AutoStartBehavior.AutoStart;
            isRemoting = UnityEngine.XR.WSA.HolographicRemoting.ConnectionState == UnityEngine.XR.WSA.HolographicStreamerConnectionState.Connected;

            // This will kill the background thread when we stop in editor.
            killToken = killTokenSource.Token;

            var x = RunObserver(killToken).ConfigureAwait(true);
        }

        private void StartUpdateTimers()
        {
            // setup and start service timers

            updateTimer = new System.Timers.Timer
            {
                Interval = Math.Max(UpdateInterval, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                AutoReset = false
            };

            if (AutoUpdate)
            {
                updateTimer.AutoReset = true;
            };

            updateTimer.Elapsed += UpdateTimerEventHandler;

            firstUpdateTimer = new System.Timers.Timer()
            {
                Interval = Math.Max(FirstUpdateDelay, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                AutoReset = false
            };

            // After an initial delay, start the auto update
            firstUpdateTimer.Elapsed += (sender, e) =>
            {
                if (shouldAutoStart)
                {
                    updateTimer.Start();
                }
            };

            firstUpdateTimer.Start();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (instantiationQueue.Count > 0)
            {
                //Debug.Log($"Got {instantiationQueue.Count} things to instantiate.");

                // Make our new objects in batches and tell observers about it
                int batchCount = Math.Min(InstantiationBatchRate, instantiationQueue.Count);

                SpatialAwarenessSceneObject saso = null;

                for (int i = 0; i < batchCount; ++i)
                {
                    bool status = instantiationQueue.TryDequeue(out saso);

                    if (CreateGameObjects)
                    {
                        Instantiate(saso);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Debug.Log("Destroy()");
            killTokenSource.Cancel();
            CleanupObserver();
        }

        #endregion IMixedRealityToolkit

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            base.Dispose(disposing);

            Suspend();

            if (disposing)
            {
                CleanupDebugGameObjects();
            }

            disposed = true;
        }

        #region IMixedRealitySpatialAwarenessObserver

        private GameObject observedObjectParent = null;

        protected virtual GameObject ObservedObjectParent => observedObjectParent ?? (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObservationParent("WindowsMixedRealitySceneUnderstandingObserver"));

        /// <inheritdoc/>
        public override void Resume()
        {
            updateTimer.Enabled = true;
        }

        /// <inheritdoc/>
        public override void Suspend()
        {
            if (updateTimer != null)
            {
                updateTimer.Enabled = false;
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver

        #region IMixedRealityOnDemandObserver

        public float FirstUpdateDelay { get; set; }

        /// <inheritdoc/>
        public override void UpdateOnDemand()
        {
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled || (SpatialAwarenessSystem == null))
            {
                return;
            }

            observerState = ObserverState.GetScene;
        }

        #endregion IMixedRealityOnDemandObserver

        #region Public Profile
        public Material DefaultMaterial { get; set; }

        /// <summary>
        /// File path to load previously saved scene data
        /// </summary>
        public TextAsset SerializedScene
        {
            get { return serializedScene; }
            set
            {
                if (serializedScene != value)
                {
                    serializedScene = value;
                    sceneBytes = serializedScene.bytes;
                }
            }
        }

        public bool UpdateOnLoad
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion Profile

        #region Private Fields

        private System.Timers.Timer firstUpdateTimer = null;
        private System.Timers.Timer updateTimer = null;
        private Dictionary<Guid, SceneMesh> cachedSceneMeshes = new Dictionary<Guid, SceneMesh>(128);
        private Dictionary<Guid, SpatialAwarenessSceneObject.Quad> cachedSceneQuads = new Dictionary<Guid, SpatialAwarenessSceneObject.Quad>(128);
        private ConcurrentQueue<SpatialAwarenessSceneObject> instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
        private TextAsset serializedScene = null;
        private byte[] sceneBytes;
        private Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;
        CancellationTokenSource killTokenSource = new System.Threading.CancellationTokenSource();

#if WINDOWS_UWP
        /// <summary>
        /// Folder location for saving serialized scene data
        /// </summary>
        private StorageFolder folderLocation;

        /// <summary>
        /// File for saving serialized scene data
        /// </summary>
        private IStorageFile SUfile;
#endif // WINDOWS_UWP

        #endregion Private Fields

        #region Public Methods
        public bool TryGetOcclusionMask(Guid quadId, ushort textureWidth, ushort textureHeight, out byte[] mask)
        {
            SpatialAwarenessSceneObject.Quad quad;

            if (!cachedSceneQuads.TryGetValue(quadId, out quad))
            {
                mask = null;
                return false;
            }

            mask = quad.occlusionMask;

            return true;
        }

        /// <summary>
        /// Returns best placement position in local space to the plane
        /// </summary>
        /// <param name="plane">The <see cref="SpatialAwarenessMeshObject"/> who's plane will be used for placement</param>
        /// <param name="objExtents">Total width and height of object to be placed in meters.</param>
        /// <param name="placementPosOnPlane">Base position on plane in local space.</param>
        /// <returns>returns <see cref="false"/> if API returns null.</returns>
        public bool TryFindCentermostPlacement(SpatialAwarenessSceneObject.Quad quad, UnityEngine.Vector2 objExtents, out UnityEngine.Vector2 placementPosOnPlane)
        {
            placementPosOnPlane = UnityEngine.Vector2.zero;

            SpatialAwarenessSceneObject.Quad associatedQuad;

            if (!cachedSceneQuads.TryGetValue(quad.guid, out associatedQuad))
            {
                return false;
            }

            System.Numerics.Vector2 ext = new System.Numerics.Vector2(objExtents.x, objExtents.y);
            System.Numerics.Vector2 pos = new System.Numerics.Vector2();

            //associatedQuad.FindCentermostPlacement(ext, out pos);

            placementPosOnPlane.Set(pos.X, pos.Y);

            return true;
        }

        #endregion Public Methods

        #region Private

        private List<SceneObject> filteredResult = new List<SceneObject>(256);
        private List<SpatialAwarenessSceneObject> convertedObjects = new List<SpatialAwarenessSceneObject>(256);

        private async Task RunObserver(CancellationToken cancellationToken)
        {
            Debug.Log($"RunObserver() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Scene scene = null;
            Scene previousScene = null;
            var sasos = new List<SpatialAwarenessSceneObject>(256);
            var sceneTransform = System.Numerics.Matrix4x4.Identity;

            while (!cancellationToken.IsCancellationRequested)
            {
                switch (observerState)
                {
                    case ObserverState.Idle:
                        //Debug.Log(ObserverState.Idle.ToString());
                        await new WaitForUpdate();
                        continue;

                    case ObserverState.WaitForAccess:
                        Debug.Log(ObserverState.WaitForAccess.ToString());

                        await new WaitForUpdate();
                        await SceneObserver.RequestAccessAsync();
                        if (shouldAutoStart)
                        {
                            observerState = ObserverState.GetScene;
                        }
                        else
                        {
                            observerState = ObserverState.Idle;
                        }
                        continue;

                    case ObserverState.GetScene:
                        Debug.Log(ObserverState.GetScene.ToString());

                        await new WaitForBackgroundThread();
                        {
                            scene = await GetSceneAsync(previousScene);
                            if (UsePersistentObjects)
                            {
                                previousScene = scene;
                            }
                            originSpatialGraphNodeId = scene.OriginSpatialGraphNodeId;
                        }
                        await new WaitForUpdate();

                        sceneTransform = await GetSceneTransformAsync();

                        Debug.Log($"sceneTransform = {sceneTransform}");

                        await new WaitForBackgroundThread();
                        {
                            sasos = await ConvertSceneObjectsAsync(scene, sceneTransform);
                        }
                        await new WaitForUpdate();

                        if (!UsePersistentObjects)
                        {
                            CleanupDebugGameObjects();
                            instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
                            sceneObjects.Clear();
                        }

                        await new WaitForBackgroundThread();
                        {
                            AddUniqueTo(sasos, instantiationQueue);
                        }
                        await new WaitForUpdate();

                        // Add new objects to observer
                        // notify subscribers

                        foreach (var saso in sasos)
                        {
                            if (!sceneObjects.ContainsKey(saso.Guid))
                            {
                                sceneObjects.Add(saso.Guid, saso);
                                SendSceneObjectAdded(saso, saso.Guid);
                            }
                        }

                        observerState = ObserverState.Idle;
                        continue;

                    default:
                        Debug.LogError("Default case - why are you here?");
                        await new WaitForUpdate();
                        continue;
                }
            }
        }

        private async Task<System.Numerics.Matrix4x4> GetSceneTransformAsync()
        {
            Debug.Log($"GetSceneTransformAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            await Task.Yield();

            var result = System.Numerics.Matrix4x4.Identity;

            if (Application.isEditor && !isRemoting)
            {
                return result;
            }

            //#if WINDOWS_UWP            
            SpatialCoordinateSystem origin = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(originSpatialGraphNodeId);
            var nativePtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
            SpatialCoordinateSystem unitySpatialCoordinateSystem = SpatialCoordinateSystem.FromNativePtr(nativePtr);
            System.Numerics.Matrix4x4? sceneToUnityTransform = origin.TryGetTransformTo(unitySpatialCoordinateSystem);

            if (sceneToUnityTransform.HasValue)
            {
                Debug.Log("sceneToUnityTransform.HasValue");
                result = SwapRuntimeAndUnityCoordinateSystem(sceneToUnityTransform.Value);
            }
            else
            {
                Debug.LogWarning("Getting coordinate system failed!");
            }
            //#endif

            return result;
        }

        private async Task<List<SpatialAwarenessSceneObject>> ConvertSceneObjectsAsync(Scene scene, System.Numerics.Matrix4x4 sceneTransform)
        {
            Debug.Log($"ConvertSceneObjectsAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            var result = new List<SpatialAwarenessSceneObject>(256);

            var rotatedScene = sceneTransform;

            // Convert from Scene objects to Spatial Awareness objects
            Debug.Log($"Got {scene.SceneObjects.Count} objects");

            // If not on HoloLens....
            // Orient data so floor with largest area is aligned to XZ plane
            // this algorithm is not full-proof and may be optionally disabled

            if (OrientScene)
            {
                if (isRemoting)
                {
                    Debug.Log("Orienting scene");
                    UnityEngine.Quaternion toUp = ToUpFromBiggestFloor(scene.SceneObjects);
                    var rotation = UnityEngine.Matrix4x4.TRS(UnityEngine.Vector3.zero, toUp, UnityEngine.Vector3.one).ToSystemNumerics();
                    rotatedScene = rotation * sceneTransform;
                }
            }

            // Add scene objects we're interested in to the stack of GameObjects to instantiate
            filteredResult.Clear();
            convertedObjects.Clear();

            if (scene.SceneObjects.Count == 0)
            {
                return convertedObjects;
            }

            filteredResult = FilterSelectedSurfaceTypes(scene.SceneObjects);

            convertedObjects = ConvertSceneObjects(filteredResult, rotatedScene);

            await Task.Yield();

            return convertedObjects;
        }

        private async Task<Scene> GetSceneAsync(Scene previousScene)
        {
            Debug.Log($"GetSceneAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Scene scene = null;

            if (ShouldLoadFromFile)
            {
                if (sceneBytes == null)
                {
                    Debug.LogError("sceneBytes is null!");
                }
                else
                {
                    Debug.Log($"Found bytes with length {sceneBytes.Length}");
                }

                // Move onto a background thread for the expensive scene loading stuff

                if (UsePersistentObjects && previousScene != null)
                {
                    scene = Scene.Deserialize(sceneBytes, previousScene);
                }
                else
                {
                    // This happens first time through as we have no history yet
                    //Debug.Log("GetSceneWithBytes() Scene.Deserialize(sceneData)");
                    scene = Scene.Deserialize(sceneBytes);
                }
            }
            else if (!Application.isEditor)
            {
                Debug.Log("Making SceneQuerySettings");

                SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
                {
                    EnableSceneObjectQuads = RequestPlaneData,
                    EnableSceneObjectMeshes = RequestMeshData,
                    EnableOnlyObservedSceneObjects = InferRegions,
                    EnableWorldMesh = SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World),
                    RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(LevelOfDetail)
                };

                Task<Scene> task;

                if (UsePersistentObjects)
                {
                    //Debug.Log("UsePersistentObjects");

                    if (previousScene != null)
                    {
                        task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene);
                    }
                    else
                    {
                        //Debug.Log("SceneObserver.ComputeAsync");

                        // first time through, we have no history
                        task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius);
                    }
                }
                else
                {
                    task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius);
                }

                //Debug.Log("Assigning scene result");

                await task;

                //Debug.Log($"task status = {task.Status}");

                scene = task.Result;

            }

            //Debug.Log($"Found {scene.SceneObjects.Count} scene objects");

            return scene;
        }

        private SpatialAwarenessSceneObject ConvertSceneObject(SceneObject sceneObject, System.Numerics.Matrix4x4 sceneTransform)
        {
            //Debug.Log($"ConvertSceneObject() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            int quadCount = sceneObject.Quads.Count;
            int meshCount = sceneObject.Meshes.Count;

            List<SpatialAwarenessSceneObject.Quad> quads = new List<SpatialAwarenessSceneObject.Quad>(quadCount);
            List<SpatialAwarenessSceneObject.MeshData> meshes = new List<SpatialAwarenessSceneObject.MeshData>(meshCount);

            if (RequestPlaneData)
            {
                SceneQuad sceneQuad = null;

                for (int i = 0; i < quadCount; ++i)
                {
                    sceneQuad = sceneObject.Quads[i];

                    var quadIdKey = sceneQuad.Id;

                    byte[] occlusionMaskBytes = null;

                    if (RequestOcclusionMask)
                    {
                        occlusionMaskBytes = new byte[OcclusionMaskResolution.x * OcclusionMaskResolution.y];
                        sceneQuad.GetSurfaceMask((ushort)OcclusionMaskResolution.x, (ushort)OcclusionMaskResolution.y, occlusionMaskBytes);
                    }

                    var extents = new UnityEngine.Vector2(sceneQuad.Extents.X, sceneQuad.Extents.Y);

                    var quad = new SpatialAwarenessSceneObject.Quad(quadIdKey, extents, occlusionMaskBytes);

                    quads.Add(quad);

                    if (!cachedSceneQuads.ContainsKey(quadIdKey))
                    {
                        cachedSceneQuads.Add(quadIdKey, quad);
                    }
                }
            }

            if (RequestMeshData)
            {
                SceneMesh sceneMesh = null;

                for (int i = 0; i < meshCount; ++i)
                {
                    sceneMesh = sceneObject.Meshes[i];
                    var meshData = MeshData(sceneMesh);
                    meshes.Add(meshData);

                    var key = sceneMesh.Id;
                    if (!cachedSceneMeshes.ContainsKey(key))
                    {
                        cachedSceneMeshes.Add(key, sceneMesh);
                    }
                }
            }

            // Apply scene transform to scene objects

            var localTransform = sceneObject.GetLocationAsMatrix(); // local space

            System.Numerics.Matrix4x4 worldTranform = localTransform * sceneTransform;

            System.Numerics.Vector3 worldTranslation;
            System.Numerics.Quaternion worldRotation;
            System.Numerics.Vector3 localScale;

            System.Numerics.Matrix4x4.Decompose(worldTranform, out localScale, out worldRotation, out worldTranslation);

            var result = new SpatialAwarenessSceneObject(
                sceneObject.Id,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                worldTranslation.ToUnityVector3(),
                worldRotation.ToUnityQuaternion(),
                quads,
                meshes);

            return result;
        }

        private List<SpatialAwarenessSceneObject> convertedResult = new List<SpatialAwarenessSceneObject>(256);

        private List<SpatialAwarenessSceneObject> ConvertSceneObjects(IReadOnlyList<SceneObject> sceneObjects, System.Numerics.Matrix4x4 sceneTransform)
        {
            Debug.Log($"ConvertSceneObjects() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Assert.IsTrue(sceneObjects.Count > 0);

            convertedResult.Clear();

            int sceneObjectCount = sceneObjects.Count;

            for (int i = 0; i < sceneObjectCount; ++i)
            {
                var saso = ConvertSceneObject(sceneObjects[i], sceneTransform);
                convertedResult.Add(saso);
            }

            return convertedResult;
        }

        private void UpdateTimerEventHandler(object sender, ElapsedEventArgs args)
        {
            observerState = ObserverState.GetScene;
            return;
        }

        private void ReadProfile()
        {
            //Debug.Log("ReadProfile");

            if (ConfigurationProfile == null)
            {
                return;
            }

            MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile;
            if (profile == null)
            {
                Debug.LogError("Windows Mixed Reality Scene Understanding Observer's configuration profile must be a MixedRealitySceneUnderstandingObserverProfile.");
                return;
            }

            StartupBehavior = profile.StartupBehavior;
            AutoUpdate = profile.AutoUpdate;
            DefaultMaterial = profile.DefaultMaterial;
            SurfaceTypes = profile.SurfaceTypes;
            RequestMeshData = profile.RequestMeshData;
            RequestPlaneData = profile.RequestPlaneData;
            CreateGameObjects = profile.CreateGameObjects;
            UsePersistentObjects = profile.UsePersistentObjects;
            UpdateInterval = profile.UpdateInterval;
            FirstUpdateDelay = profile.FirstUpdateDelay;
            ShouldLoadFromFile = profile.ShouldLoadFromFile;
            SerializedScene = profile.SerializedScene;
            InferRegions = profile.InferRegions;
            LevelOfDetail = profile.LevelOfDetail;
            InstantiationBatchRate = profile.InstantiationBatchRate;
            ObservationExtents = profile.ObservationExtents;
            QueryRadius = profile.QueryRadius;
            RequestOcclusionMask = profile.RequestOcclusionMask;
            OcclusionMaskResolution = profile.OcclusionMaskResolution;
            OrientScene = profile.OrientScene;
        }

        private void CleanupObserver()
        {
            Dispose(true);
        }

        private List<SceneObject> filteredSelectedSurfaceTypesResult = new List<SceneObject>(128);
        private enum ObserverState
        {
            Idle = 0,
            WaitForAccess,
            GetScene,
            GetSceneTransform,
            Convert
        }
        private ObserverState observerState = ObserverState.WaitForAccess;
        private CancellationToken killToken;
        private bool shouldAutoStart;
        private bool isRemoting;
        private Guid originSpatialGraphNodeId;

        private System.Numerics.Matrix4x4 sharedSceneTransform;

        private List<SceneObject> FilterSelectedSurfaceTypes(IReadOnlyList<SceneObject> newObjects)
        {
            filteredSelectedSurfaceTypesResult.Clear();

            int count = newObjects.Count;

            for (int i = 0; i < count; ++i)
            {
                if (!SurfaceTypes.HasFlag(SpatialAwarenessSurfaceType(newObjects[i].Kind)))
                {
                    continue;
                }

                filteredSelectedSurfaceTypesResult.Add(newObjects[i]);
            }

            return filteredSelectedSurfaceTypesResult;
        }

        private void AddUniqueTo(List<SpatialAwarenessSceneObject> newObjects, ConcurrentQueue<SpatialAwarenessSceneObject> existing)
        {
            int length = newObjects.Count;

            for (int i = 0; i < length; ++i)
            {
                if (!sceneObjects.ContainsKey(newObjects[i].Guid))
                {
                    existing.Enqueue(newObjects[i]);
                }
            }
        }

        private void Instantiate(SpatialAwarenessSceneObject saso)
        {
            // Until this point the SASO has been a data representation

            surfaceTypeName = $"{saso.SurfaceType.ToString()} {saso.Guid.ToString()}";

            saso.GameObject = new GameObject(surfaceTypeName)
            {
                layer = PhysicsLayer
            };

            saso.GameObject.transform.position = saso.Position;
            saso.GameObject.transform.rotation = saso.Rotation;

            saso.GameObject.transform.SetParent(ObservedObjectParent.transform);

            // Maybe make GameObjects for Quads and Meshes

            if (!RequestPlaneData && !RequestMeshData)
            {
                return;
            }

            if (RequestPlaneData)
            {
                // Add MeshFilter, attach shared quad and scale it
                // later, we can update scale of existing quads if they change size
                // (as opposed to modifying the vertexs directly, when persisting objects)
                int quadCount = saso.Quads.Count;

                for (int i = 0; i < quadCount; ++i)
                {
                    var go = new GameObject("Quad");

                    var quad = saso.Quads[i];

                    var meshFilter = go.AddComponent<MeshFilter>();
                    meshFilter.mesh = normalizedQuadMesh;

                    var meshRenderer = go.AddComponent<MeshRenderer>();

                    if (DefaultMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultMaterial;
                    }

                    if (RequestOcclusionMask)
                    {
                        if (quad.occlusionMask != null)
                        {
                            var occlusionTexture = OcclulsionTexture(quad.occlusionMask);
                            meshRenderer.material.mainTexture = occlusionTexture;
                        }
                    }

                    var scale = new UnityEngine.Vector3(quad.extents.x, quad.extents.y, 0);
                    go.transform.localScale = scale;

                    go.AddComponent<BoxCollider>();

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            if (RequestMeshData)
            {
                int meshCount = saso.Meshes.Count;

                for (int i = 0; i < meshCount; ++i)
                {
                    var go = new GameObject("Mesh");

                    var mf = go.AddComponent<MeshFilter>();
                    mf.mesh = UnityMeshFromMeshData(saso.Meshes[i]);

                    var mr = go.AddComponent<MeshRenderer>();

                    if (DefaultMaterial)
                    {
                        mr.sharedMaterial = DefaultMaterial;
                    }
                    go.AddComponent<MeshCollider>();

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            return;
        }

        private Texture2D OcclulsionTexture(byte[] textureBytes)
        {
            Assert.IsNotNull(textureBytes);

            // Create a new texture.
            Texture2D result = new Texture2D(OcclusionMaskResolution.x, OcclusionMaskResolution.y);
            //result.filterMode = FilterMode.Bilinear;
            result.wrapMode = TextureWrapMode.Clamp;

            // Transfer the invalidation mask onto the texture.
            Color[] pixels = result.GetPixels();

            var numPixels = pixels.Length;

            for (int i = 0; i < numPixels; ++i)
            {
                var value = textureBytes[i];

                switch (value)
                {
                    case (byte)SceneRegionSurfaceKind.NotSurface:
                        pixels[i] = Color.clear;
                        break;
                    case (byte)SceneRegionSurfaceKind.SurfaceObserved:
                        pixels[i] = Color.cyan;
                        break;
                    case (byte)SceneRegionSurfaceKind.SurfaceInferred:
                        pixels[i] = Color.yellow;
                        break;
                    default:
                        Debug.LogWarning($"Got unknown surface kind {value.ToString()}");
                        pixels[i] = Color.magenta;
                        break;
                }
            }

            result.SetPixels(pixels);
            result.Apply(true);

            return result;
        }

        private void CleanupDebugGameObjects()
        {
            Debug.Log($"CleanupDebugGameObjects() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (observedObjectParent != null)
            {
                int kidCount = observedObjectParent.transform.childCount;

                for (int i = 0; i < kidCount; ++i)
                {
                    UnityEngine.Object.Destroy(observedObjectParent.transform.GetChild(i).gameObject);
                }
            }
        }

        public override void SaveScene(string filename)
        {
            SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
            {
                EnableSceneObjectQuads = RequestPlaneData,
                EnableSceneObjectMeshes = RequestMeshData,
                EnableOnlyObservedSceneObjects = InferRegions,
                EnableWorldMesh = SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World),
                RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(LevelOfDetail)
            };

            var serializedScene = SceneObserver.ComputeSerializedAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();

            var bytes = new byte[serializedScene.Size];
            serializedScene.GetData(bytes);

            SaveToFile(bytes);
        }

#if WINDOWS_UWP
        /// <summary>
        /// Asynchronously saves the <see cref="SceneUnderstanding.SceneProcessor"/>'s data stream as a file for later use
        /// </summary>
        /// <param name="sceneBuffer">the <see cref="byte[]"/></param>
        private async void WriteSUFileAsync(IStorageFile file, byte[] dataBuffer)
        {
            await FileIO.WriteBytesAsync(file, dataBuffer);
        }
#endif // WINDOWS_UWP

        /// <summary>
        /// Saves the <see cref="SceneUnderstanding.SceneProcessor"/>'s data stream as a file for later use
        /// </summary>
        /// <param name="sceneBuffer">the <see cref="byte[]"/></param>
        private void SaveToFile(byte[] sceneBuffer)
        {
#if WINDOWS_UWP
            DateTime currDateTime = DateTime.Now;
            string currDateTimeString = $"{currDateTime.Date.Year}-{currDateTime.Date.Month}-{currDateTime.Date.Day}_{currDateTime.TimeOfDay.Hours}-{currDateTime.TimeOfDay.Minutes}-{currDateTime.TimeOfDay.Seconds}";

            folderLocation = ApplicationData.Current.LocalFolder;
            SUfile = folderLocation.CreateFileAsync("SU_" + currDateTimeString).GetAwaiter().GetResult();
            WriteSUFileAsync(SUfile, sceneBuffer);
#endif // WINDOWS_UWP
        }

        /// <summary>
        /// Converts a MRTK/platform agnostic <see cref="SpatialAwarenessMeshLevelOfDetail"/> to a <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneMeshLevelOfDetail"/>.
        /// </summary>
        /// <param name="levelofDetail">The <see cref="SpatialAwarenessMeshLevelOfDetail"/> to convert.</param>
        /// <returns>The equivalent <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneMeshLevelOfDetail"/></returns>
        private SceneMeshLevelOfDetail LevelOfDetailToMeshLOD(SpatialAwarenessMeshLevelOfDetail levelofDetail)
        {
            switch (levelofDetail)
            {
                case SpatialAwarenessMeshLevelOfDetail.Custom:
                    Debug.LogWarning("SceneUnderstanding LOD is set to custom, falling back to Medium");
                    return SceneMeshLevelOfDetail.Medium;

                case SpatialAwarenessMeshLevelOfDetail.Coarse:
                    return SceneMeshLevelOfDetail.Coarse;

                case SpatialAwarenessMeshLevelOfDetail.Medium:
                    return SceneMeshLevelOfDetail.Medium;

                case SpatialAwarenessMeshLevelOfDetail.Fine:
                    return SceneMeshLevelOfDetail.Fine;

                default:
                    return SceneMeshLevelOfDetail.Medium;
            }
        }

        /// <summary>
        /// Helper to convert from Right hand to Left hand coordinates using <see cref="System.Numerics.Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix">The Right handed <see cref="System.Numerics.Matrix4x4"/>.</param>
        /// <returns>The <see cref="System.Numerics.Matrix4x4"/> in left hand form.</returns>
        public static System.Numerics.Matrix4x4 SwapRuntimeAndUnityCoordinateSystem(System.Numerics.Matrix4x4 matrix)
        {
            matrix.M13 = -matrix.M13;
            matrix.M23 = -matrix.M23;
            matrix.M43 = -matrix.M43;

            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M34 = -matrix.M34;

            return matrix;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        private static Mesh UnityMeshFromMeshData(SpatialAwarenessSceneObject.MeshData meshData)
        {
            Mesh unityMesh = new Mesh();

            // Unity has a limit of 65,535 vertices in a mesh.
            // This limit exists because by default Unity uses 16-bit index buffers.
            // Starting with 2018.1, Unity allows one to use 32-bit index buffers.
            if (meshData.vertices.Length > 65535)
            {
                unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            unityMesh.SetVertices(new List<UnityEngine.Vector3>(meshData.vertices));
            unityMesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);
            unityMesh.RecalculateNormals();

            return unityMesh;
        }

        /// <summary>
        /// Orients the root game object, such that the Scene Understanding floor lies on the Unity world's X-Z plane.
        /// The floor type with the largest area is choosen as the reference.
        /// If no floor is found....???
        /// </summary>
        /// <param name="scene">Scene Understanding scene.</param>
        private UnityEngine.Quaternion ToUpFromBiggestFloor(IReadOnlyList<SceneObject> sasos)
        {
            float areaForlargestFloorSoFar = 0;
            SceneObject floorSceneObject = null;
            SceneQuad floorQuad = null;

            // Find the largest floor quad.
            var count = sasos.Count;
            for (var i = 0; i < count; ++i)
            {
                if (sasos[i].Kind == SceneObjectKind.Floor)
                {
                    var quads = sasos[i].Quads;

                    Assert.IsNotNull(quads);

                    var qcount = quads.Count;
                    for (int j = 0; j < qcount; j++)
                    {
                        float quadArea = quads[j].Extents.X * quads[j].Extents.Y;

                        if (quadArea > areaForlargestFloorSoFar)
                        {
                            areaForlargestFloorSoFar = quadArea;
                            floorSceneObject = sasos[i];
                            floorQuad = quads[j];
                        }
                    }
                }
            }

            if (floorQuad != null)
            {
                // Compute the floor quad's normal.
                float halfWidthMeters = floorQuad.Extents.X * .5f;
                float halfHeightMeters = floorQuad.Extents.Y * .5f;

                System.Numerics.Vector3 point1 = new System.Numerics.Vector3(-halfWidthMeters, -halfHeightMeters, 0);
                System.Numerics.Vector3 point2 = new System.Numerics.Vector3(halfWidthMeters, -halfHeightMeters, 0);
                System.Numerics.Vector3 point3 = new System.Numerics.Vector3(-halfWidthMeters, halfHeightMeters, 0);

                System.Numerics.Matrix4x4 objectTransform = floorSceneObject.GetLocationAsMatrix();

                objectTransform = SwapRuntimeAndUnityCoordinateSystem(objectTransform);

                System.Numerics.Vector3 tPoint1 = System.Numerics.Vector3.Transform(point1, objectTransform);
                System.Numerics.Vector3 tPoint2 = System.Numerics.Vector3.Transform(point2, objectTransform);
                System.Numerics.Vector3 tPoint3 = System.Numerics.Vector3.Transform(point3, objectTransform);

                System.Numerics.Vector3 p21 = tPoint2 - tPoint1;
                System.Numerics.Vector3 p31 = tPoint3 - tPoint1;

                System.Numerics.Vector3 floorNormal = System.Numerics.Vector3.Cross(p21, p31);

                // Numerics to Unity conversion.
                UnityEngine.Vector3 floorNormalUnity = new UnityEngine.Vector3(floorNormal.X, floorNormal.Y, floorNormal.Z);

                // Get the rotation between the floor normal and Unity world's up vector.
                return UnityEngine.Quaternion.FromToRotation(floorNormalUnity, UnityEngine.Vector3.up);
            }

            return UnityEngine.Quaternion.identity;
        }

        private SpatialAwarenessSceneObject.MeshData MeshData(SceneMesh sceneMesh)
        {
            // Indices

            var indices = new int[sceneMesh.TriangleIndexCount];

            var meshIndices = new uint[sceneMesh.TriangleIndexCount];
            sceneMesh.GetTriangleIndices(meshIndices);

            var ilength = meshIndices.Length;

            for (int i = 0; i < ilength; ++i)
            {
                indices[i] = (int)meshIndices[i];
            }

            // Vertices

            var vertices = new UnityEngine.Vector3[sceneMesh.VertexCount];

            var meshVertices = new System.Numerics.Vector3[sceneMesh.VertexCount];
            sceneMesh.GetVertexPositions(meshVertices);

            var vlength = meshVertices.Length;

            for (int i = 0; i < vlength; ++i)
            {
                vertices[i] = new UnityEngine.Vector3(meshVertices[i].X, meshVertices[i].Y, -meshVertices[i].Z);
            }

            var result = new SpatialAwarenessSceneObject.MeshData
            {
                indices = indices,
                vertices = vertices,
                guid = sceneMesh.Id
            };

            return result;
        }

        /// <summary>
        /// Converts a <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObjectKind"/> to a MRTK/platform agnostic <see cref="SpatialAwarenessSurfaceTypes"/>
        /// </summary>
        /// <param name="label">The <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObjectKind"/> to convert.</param>
        /// <returns>The equivalent <see cref="SpatialAwarenessSurfaceTypes"/></returns>
        private SpatialAwarenessSurfaceTypes SpatialAwarenessSurfaceType(SceneObjectKind label)
        {
            switch (label)
            {
                case SceneObjectKind.Background:
                    return SpatialAwarenessSurfaceTypes.Background;

                case SceneObjectKind.Wall:
                    return SpatialAwarenessSurfaceTypes.Wall;

                case SceneObjectKind.Floor:
                    return SpatialAwarenessSurfaceTypes.Floor;

                case SceneObjectKind.Ceiling:
                    return SpatialAwarenessSurfaceTypes.Ceiling;

                case SceneObjectKind.Platform:
                    return SpatialAwarenessSurfaceTypes.Platform;

                case SceneObjectKind.World:
                    return SpatialAwarenessSurfaceTypes.World;

                case SceneObjectKind.CompletelyInferred:
                    return SpatialAwarenessSurfaceTypes.CompletelyInferred;

                case SceneObjectKind.Unknown:
                default:
                    return SpatialAwarenessSurfaceTypes.Unknown;
            }
        }

        #endregion Private Methods
    }
}
