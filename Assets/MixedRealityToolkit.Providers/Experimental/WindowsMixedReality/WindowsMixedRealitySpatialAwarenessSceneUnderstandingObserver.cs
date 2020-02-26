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
using System.Threading;
using System.Collections.Concurrent;

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
    public class WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver :
        BaseSpatialSceneObserver,
        IMixedRealityOnDemandObserver
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

        private void CreateQuadFromExtents(Mesh mesh, float x, float y)
        {
            List<Vector3> vertices = new List<Vector3>()
            {
                new Vector3(-x / 2, -y / 2, 0),
                new Vector3( x / 2, -y / 2, 0),
                new Vector3(-x / 2,  y / 2, 0),
                new Vector3( x / 2,  y / 2, 0)
            };

            Vector2[] quadUVs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            int[] quadTriangles = new int[]
            {
                0, 3, 1,
                0, 2, 3,
                1, 3, 0,
                3, 2, 0
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(quadTriangles, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, new List<Vector2>(quadUVs));
        }

        public override void Initialize()
        {
            //Debug.Log($"Initialize() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            base.Initialize();
            CreateQuadFromExtents(normalizedQuadMesh, 1, 1);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            //Debug.Log("Enable()");

            shouldAutoStart = StartupBehavior == AutoStartBehavior.AutoStart;

            StartUpdateTimers();

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
                        InstantiateSceneObject(saso);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            //Debug.Log("Destroy()");
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
        private Dictionary<Guid, Tuple<SceneQuad, SceneObject>> cachedSceneQuads = new Dictionary<Guid, Tuple<SceneQuad, SceneObject>>(256);
        private ConcurrentQueue<SpatialAwarenessSceneObject> instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
        private TextAsset serializedScene = null;
        private byte[] sceneBytes;
        private Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;
        private CancellationTokenSource killTokenSource = new System.Threading.CancellationTokenSource();

        #endregion Private Fields

        #region Public Methods
        public bool TryGetOcclusionMask(Guid quadId, ushort textureWidth, ushort textureHeight, out byte[] mask)
        {
            Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadId, out result))
            {
                mask = null;
                return false;
            }

            SceneQuad quad = result.Item1;
            SceneObject sceneObject = result.Item2;

            var maskResult = new byte[textureWidth * textureHeight];

            quad.GetSurfaceMask(textureWidth, textureHeight, maskResult);

            mask = maskResult;

            return true;
        }

        /// <summary>
        /// Returns best placement position in local space to the plane
        /// </summary>
        /// <param name="plane">The <see cref="SpatialAwarenessMeshObject"/> who's plane will be used for placement</param>
        /// <param name="objExtents">Total width and height of object to be placed in meters.</param>
        /// <param name="placementPosOnPlane">Base position on plane in local space.</param>
        /// <returns>returns <see cref="false"/> if API returns null.</returns>
        public override bool TryFindCentermostPlacement(Guid quadGuid, Vector2 objExtents, out Vector3 placementPosOnPlane)
        {
            Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadGuid, out result))
            {
                placementPosOnPlane = Vector2.zero;
                return false;
            }

            SceneQuad quad = result.Item1;
            SceneObject sceneObject = result.Item2;

            System.Numerics.Vector2 ext = new System.Numerics.Vector2(objExtents.x, objExtents.y);
            System.Numerics.Vector2 centerPosition = new System.Numerics.Vector2();

            quad.FindCentermostPlacement(ext, out centerPosition);

            // The best position is expressed realative to it's quad
            // Expresed as a hierarchy:
            // Scene
            //   |_Quad
            //       |_Placement
            // We're returning everything to the user in world space,
            // to avoid having to manage parenting or scenes

            Debug.Log($"placementPosition = {centerPosition}");

            // SU placement origin is top left (2d sheet of paper)

            centerPosition -= quad.Extents / new System.Numerics.Vector2(2.0f);

            var centerUnity = new Vector3(centerPosition.X, centerPosition.Y, 0);
            placementPosOnPlane = (sceneObject.GetLocationAsMatrix() * sceneToWorldXformSystem).ToUnity().MultiplyPoint(centerUnity);

            return true;
        }

        #endregion Public Methods

        #region Private

        private async Task RunObserver(CancellationToken cancellationToken)
        {
            Debug.Log($"RunObserver() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Scene scene = null;
            Scene previousScene = null;
            var sasos = new List<SpatialAwarenessSceneObject>(256);

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
                        var task = await SceneObserver.RequestAccessAsync();

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
                            scene = GetSceneAsync(previousScene);

                            previousScene = scene;

                            sceneOriginId = scene.OriginSpatialGraphNodeId;
                        }
                        await new WaitForUpdate();

                        sceneToWorldXformSystem = await GetSceneToWorldTransform();
                        Debug.Log($"sceneToWorldXformSystem = {sceneToWorldXformSystem}");

                        if (!UsePersistentObjects)
                        {
                            // these are cached when we do ConvertSceneObject()
                            cachedSceneQuads.Clear();
                        }

                        await new WaitForBackgroundThread();
                        {
                            sasos = await ConvertSceneObjectsAsync(scene);

                            // If not on HoloLens....
                            // Orient data so floor with largest area is aligned to XZ plane
                            // this algorithm relies on floors exisiting so it may fail
                            // it can be disabled.

                            //if (OrientScene)
                            //{
                            //    //if (isRemoting)
                            //    //{
                            //        Debug.Log("Orienting scene");
                            //        Quaternion toUp = ToUpFromBiggestFloor(scene.SceneObjects);
                            //        var rotation = Matrix4x4.TRS(UnityEngine.Vector3.zero, toUp, UnityEngine.Vector3.one).ToSystemNumerics();

                            //        var count = sasos.Count;
                            //        for (int i = 0; i < count; ++i)
                            //        {
                            //        sasos[i].Rotation *= rotation;
                            //        }
                            //    //}
                            //}
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

        private async Task<System.Numerics.Matrix4x4> GetSceneToWorldTransform()
        {
            Debug.Log($"GetSceneTransformAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            await Task.Yield();

            var result = System.Numerics.Matrix4x4.Identity;

            if (Application.isEditor && !isRemoting)
            {
                return result;
            }

            SpatialCoordinateSystem sceneOrigin = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(sceneOriginId);

            var nativePtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
            SpatialCoordinateSystem worldOrigin = SpatialCoordinateSystem.FromNativePtr(nativePtr);

            var sceneToWorld = sceneOrigin.TryGetTransformTo(worldOrigin);

            if (sceneToWorld.HasValue)
            {
                result = sceneToWorld.Value; // numerics
            }
            else
            {
                Debug.LogWarning("Getting coordinate system failed!");
            }

            return result;
        }

        private Scene GetSceneAsync(Scene previousScene)
        {
            Debug.Log($"GetSceneAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Scene scene = null;

            if (Application.isEditor && ShouldLoadFromFile)
            {
                if (sceneBytes == null)
                {
                    Debug.LogError("sceneBytes is null!");
                }
                //else
                //{
                //    Debug.Log($"Found bytes with length {sceneBytes.Length}");
                //}

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
            else
            {
                Debug.Log("Making SceneQuerySettings");

                SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
                {
                    EnableSceneObjectQuads = RequestPlaneData,
                    EnableSceneObjectMeshes = RequestMeshData,
                    EnableOnlyObservedSceneObjects = InferRegions,
                    EnableWorldMesh = SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World),
                    RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(WorldMeshLevelOfDetail)
                };

                //Task<Scene> task;

                if (UsePersistentObjects)
                {
                    Debug.Log("UsePersistentObjects");

                    if (previousScene != null)
                    {
                        scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Debug.Log("SceneObserver.ComputeAsync");

                        // first time through, we have no history
                        scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
                }
            }

            Debug.Log($"Found {scene.SceneObjects.Count} scene objects");

            return scene;
        }

        private SpatialAwarenessSceneObject ConvertSceneObject(SceneObject sceneObject)
        {
            Debug.Log($"ConvertSceneObject() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

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

                    // Store a cache so we can retrieve best position on plane later.

                    if (!cachedSceneQuads.ContainsKey(quadIdKey))
                    {
                        cachedSceneQuads.Add(quadIdKey, new Tuple<SceneQuad, SceneObject>(sceneQuad, sceneObject));
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
                }
            }

            // World space conversion

            //Debug.Log($"sceneObjectTransform = {sceneObjectXformSystem}");

            System.Numerics.Matrix4x4 worldXformSystem = sceneObject.GetLocationAsMatrix() * sceneToWorldXformSystem;

            System.Numerics.Vector3 worldTranslationSystem;
            System.Numerics.Quaternion worldRotationSytem;
            System.Numerics.Vector3 localScale;

            System.Numerics.Matrix4x4.Decompose(worldXformSystem, out localScale, out worldRotationSytem, out worldTranslationSystem);

            //Debug.Log($"worldTranslation = {worldTranslation}");

            var result = new SpatialAwarenessSceneObject(
                sceneObject.Id,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                worldTranslationSystem.ToUnityVector3(),
                worldRotationSytem.ToUnityQuaternion(),
                quads,
                meshes);

            return result;
        }

        private List<SpatialAwarenessSceneObject> convertedObjects = new List<SpatialAwarenessSceneObject>(256);

        private async Task<List<SpatialAwarenessSceneObject>> ConvertSceneObjectsAsync(Scene scene)
        {
            Debug.Log($"ConvertSceneObjectsAsync() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            var result = new List<SpatialAwarenessSceneObject>(256);

            // Convert from Scene objects to Spatial Awareness objects
            Debug.Log($"Got {scene.SceneObjects.Count} objects");

            // Add scene objects we're interested in to the stack of GameObjects to instantiate
            convertedObjects.Clear();

            if (scene.SceneObjects.Count == 0)
            {
                return convertedObjects;
            }

            var filteredSceneObjects = FilterSelectedSurfaceTypes(scene.SceneObjects);

            int sceneObjectCount = filteredSceneObjects.Count;

            for (int i = 0; i < sceneObjectCount; ++i)
            {
                var saso = ConvertSceneObject(filteredSceneObjects[i]);
                convertedObjects.Add(saso);
            }

            await Task.Yield();

            return convertedObjects;
        }

        private void UpdateTimerEventHandler(object sender, ElapsedEventArgs args)
        {
            if (AutoUpdate)
            {
                observerState = ObserverState.GetScene;
            }
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
            WorldMeshLevelOfDetail = profile.WorldMeshLevelOfDetail;
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
        private Guid sceneOriginId;

        private System.Numerics.Matrix4x4 sceneToWorldXformSystem;

        private List<SceneObject> filteredSelectedSurfaceTypesResult = new List<SceneObject>(128);
        private Texture defaultTexture;

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

        private void InstantiateSceneObject(SpatialAwarenessSceneObject saso)
        {
            // Until this point the SASO has been a data representation

            surfaceTypeName = $"{saso.SurfaceType.ToString()} {saso.Guid.ToString()}";

            saso.GameObject = new GameObject(surfaceTypeName)
            {
                layer = PhysicsLayer
            };

            saso.GameObject.transform.SetParent(ObservedObjectParent.transform);

            saso.GameObject.transform.localPosition = saso.Position;
            saso.GameObject.transform.localRotation = saso.Rotation;
            saso.GameObject.transform.localScale = Vector3.one;

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
                    var quad = saso.Quads[i];

                    var quadGo = new GameObject($"Quad {quad.guid}");

                    var meshFilter = quadGo.AddComponent<MeshFilter>();
                    meshFilter.mesh = normalizedQuadMesh;

                    var meshRenderer = quadGo.AddComponent<MeshRenderer>();

                    quadGo.AddComponent<BoxCollider>();

                    if (DefaultMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultMaterial;
                        if (defaultTexture == null)
                        {
                            defaultTexture = DefaultMaterial.mainTexture;
                        }
                        meshRenderer.material.color = ColorForSurfaceType(saso.SurfaceType);
                    }

                    if (RequestOcclusionMask)
                    {
                        if (quad.occlusionMask != null)
                        {
                            var occlusionTexture = OcclulsionTexture(quad.occlusionMask);
                            meshRenderer.material.mainTexture = occlusionTexture;
                        }
                    }
                    else
                    {
                        meshRenderer.material.mainTexture = defaultTexture;
                    }

                    quadGo.transform.SetParent(saso.GameObject.transform);

                    quadGo.transform.localPosition = UnityEngine.Vector3.zero;
                    quadGo.transform.localRotation = UnityEngine.Quaternion.identity;
                    quadGo.transform.localScale = new UnityEngine.Vector3(quad.extents.x, quad.extents.y, 0);

                }
            }

            if (RequestMeshData)
            {
                int meshCount = saso.Meshes.Count;

                for (int i = 0; i < meshCount; ++i)
                {
                    var meshAlias = saso.Meshes[i];

                    var go = new GameObject($"Mesh {meshAlias.guid}");

                    var mf = go.AddComponent<MeshFilter>();
                    mf.mesh = UnityMeshFromMeshData(meshAlias);

                    var meshRenderer = go.AddComponent<MeshRenderer>();

                    go.AddComponent<MeshCollider>();

                    if (DefaultMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultMaterial;
                        meshRenderer.material.color = ColorForSurfaceType(saso.SurfaceType);
                    }

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            return;
        }

        private Color ColorForSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            switch (surfaceType)
            {
                case SpatialAwarenessSurfaceTypes.Unknown:
                    return new Color32(220, 50, 47, 255); // red
                case SpatialAwarenessSurfaceTypes.Floor:
                    return new Color32(38, 139, 210, 255); // blue
                case SpatialAwarenessSurfaceTypes.Ceiling:
                    return new Color32(108, 113, 196, 255); // violet
                case SpatialAwarenessSurfaceTypes.Wall:
                    return new Color32(181, 137, 0, 255); // yellow
                case SpatialAwarenessSurfaceTypes.Platform:
                    return new Color32(133, 153, 0, 255); // green
                case SpatialAwarenessSurfaceTypes.Background:
                    return new Color32(203, 75, 22, 255); // orange
                case SpatialAwarenessSurfaceTypes.World:
                    return new Color32(211, 54, 130, 255); // magenta
                case SpatialAwarenessSurfaceTypes.CompletelyInferred:
                    return new Color32(42, 161, 152, 255); // cyan
                default:
                    return new Color32(220, 50, 47, 255); // red
            }
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
            //Debug.Log($"CleanupDebugGameObjects() ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (observedObjectParent != null)
            {
                int kidCount = observedObjectParent.transform.childCount;

                for (int i = 0; i < kidCount; ++i)
                {
                    UnityEngine.Object.Destroy(observedObjectParent.transform.GetChild(i).gameObject);
                }
            }
        }

        public override void SaveScene(string prefix)
        {
#if WINDOWS_UWP
            SaveToFile(prefix);
#else // WINDOWS_UWP
            Debug.LogWarning("SaveScene() only supported at runtime! Ignoring request.");
#endif // WINDOWS_UWP
        }

#if WINDOWS_UWP
        /// <summary>
        /// Saves the <see cref="SceneUnderstanding.SceneProcessor"/>'s data stream as a file for later use
        /// </summary>
        /// <param name="sceneBuffer">the <see cref="byte[]"/></param>
        private async void SaveToFile(string prefix)
        {
            SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
            {
                EnableSceneObjectQuads = RequestPlaneData,
                EnableSceneObjectMeshes = RequestMeshData,
                EnableOnlyObservedSceneObjects = InferRegions,
                EnableWorldMesh = SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World),
                RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(WorldMeshLevelOfDetail)
            };

            var serializedScene = SceneObserver.ComputeSerializedAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
            var bytes = new byte[serializedScene.Size];
            serializedScene.GetData(bytes);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            var filename = $"SceneUnderStanding_{timestamp}.bytes";
            if (prefix != "")
            {
                filename = $"{prefix}_{timestamp}.bytes";
            }
            StorageFolder folderLocation = ApplicationData.Current.LocalFolder;
            Debug.Log($"filename = {filename}, folderLocation = {folderLocation.ToString()}");
            IStorageFile storageFile = await folderLocation.CreateFileAsync(filename);
            await FileIO.WriteBytesAsync(storageFile, bytes);
        }
#endif // WINDOWS_UWP

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

                case SpatialAwarenessMeshLevelOfDetail.Unlimited:
                    return SceneMeshLevelOfDetail.Unlimited;

                default:
                    return SceneMeshLevelOfDetail.Medium;
            }
        }

        /// <summary>
        /// Helper to convert from Right hand to Left hand coordinates using <see cref="System.Numerics.Matrix4x4"/>.
        /// https://docs.microsoft.com/en-us/windows/mixed-reality/unity-xrdevice-advanced#converting-between-coordinate-systems
        /// </summary>
        /// <param name="matrix">The Right handed <see cref="System.Numerics.Matrix4x4"/>.</param>
        /// <returns>The <see cref="System.Numerics.Matrix4x4"/> in left hand form.</returns>
        public static System.Numerics.Matrix4x4 RightToLeftHanded(System.Numerics.Matrix4x4 matrix)
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

            unityMesh.SetVertices(new List<Vector3>(meshData.vertices));
            unityMesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);
            unityMesh.SetUVs(0, new List<Vector2>(meshData.uvs));
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

                System.Numerics.Matrix4x4 objectToSceneOrigin = floorSceneObject.GetLocationAsMatrix();

                objectToSceneOrigin = RightToLeftHanded(objectToSceneOrigin);

                System.Numerics.Vector3 tPoint1 = System.Numerics.Vector3.Transform(point1, objectToSceneOrigin);
                System.Numerics.Vector3 tPoint2 = System.Numerics.Vector3.Transform(point2, objectToSceneOrigin);
                System.Numerics.Vector3 tPoint3 = System.Numerics.Vector3.Transform(point3, objectToSceneOrigin);

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
            var uvs = new UnityEngine.Vector2[sceneMesh.VertexCount];

            var meshVertices = new System.Numerics.Vector3[sceneMesh.VertexCount];
            sceneMesh.GetVertexPositions(meshVertices);

            var vlength = meshVertices.Length;

            var minx = meshVertices[0].X;
            var miny = meshVertices[0].Y;
            var maxx = minx;
            var maxy = miny;
            float x = minx;
            float y = miny;

            for (int i = 0; i < vlength; ++i)
            {
                x = meshVertices[i].X;
                y = meshVertices[i].Y;

                vertices[i] = new UnityEngine.Vector3(x, y, -meshVertices[i].Z);
                minx = Math.Min(minx, x);
                miny = Math.Min(miny, y);
                maxx = Math.Max(maxx, x);
                maxy = Math.Max(maxy, y);
            }

            // UVs - planar square projection

            float smallestDimension = Math.Min(minx, miny);
            float biggestDimension = Math.Max(maxx, maxy);

            for (int i = 0; i< vlength; ++i)
            {
                uvs[i] = new Vector2(
                    Mathf.InverseLerp(smallestDimension, biggestDimension, vertices[i].x),
                    Mathf.InverseLerp(smallestDimension, biggestDimension, vertices[i].y));
            }

            var result = new SpatialAwarenessSceneObject.MeshData
            {
                indices = indices,
                vertices = vertices,
                guid = sceneMesh.Id,
                uvs = uvs
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
