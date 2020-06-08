// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.SceneUnderstanding;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.Windows.Perception.Spatial;
using Microsoft.Windows.Perception.Spatial.Preview;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Experimental.SpatialAwareness
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Scene Understanding Observer",
        "Experimental/WindowsMixedReality/SceneUnderstanding/DefaultSceneUnderstandingObserverProfile.asset",
        "MixedRealityToolkit.Providers")]
    public class WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver :
        BaseSpatialObserver,
        IMixedRealitySpatialAwarenessSceneUnderstandingObserver
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

        #region IMixedRealityService

        /// <inheritdoc />
        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            sceneEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>(EventSystem.current);
            CreateQuadFromExtents(normalizedQuadMesh, 1, 1);

            var accessStatus = SceneObserver.RequestAccessAsync().GetAwaiter().GetResult();
            if (accessStatus == SceneObserverAccessStatus.Allowed)
            {
                IsRunning = true;
                observerState = ObserverState.Idle;
                StartUpdateTimers();
            }
            else
            {
                Debug.LogError("Something went terribly wrong getting scene observer access!");
            }

            if (UpdateOnceOnLoad)
            {
                observerState = ObserverState.GetScene;
                //doUpdateOnceOnLoad = true;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            isRemoting = UnityEngine.XR.WSA.HolographicRemoting.ConnectionState == UnityEngine.XR.WSA.HolographicStreamerConnectionState.Connected;

            // This will kill the background thread when we stop in editor.
            killToken = killTokenSource.Token;

            // there is odd behavior with WaitForBackgroundTask
            // it will sometimes run on the main thread unless we start it this way
            var x = RunObserverAsync(killToken).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (instantiationQueue.Count > 0)
            {
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
            killTokenSource.Cancel();
            CleanupObserver();
        }

        #endregion IMixedRealityService

        #region BaseService

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

        #endregion BaseService

        #region IMixedRealitySpatialAwarenessObserver

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

        #region IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        /// <inheritdoc/>
        public IReadOnlyDictionary<System.Guid, SpatialAwarenessSceneObject> SceneObjects { get; set; }
        /// <inheritdoc/>
        public SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }
        /// <inheritdoc/>
        public bool ShouldLoadFromFile { get; set; }
        /// <inheritdoc/>
        public int InstantiationBatchRate { get; set; }
        /// <inheritdoc/>
        public bool InferRegions { get; set; }
        /// <inheritdoc/>
        public bool RequestMeshData { get; set; }
        /// <inheritdoc/>
        public bool RequestPlaneData { get; set; }
        /// <inheritdoc/>
        public bool RequestOcclusionMask { get; set; }
        /// <inheritdoc/>
        public bool UsePersistentObjects { get; set; }
        /// <inheritdoc/>
        public float QueryRadius { get; set; }
        /// <inheritdoc/>
        public Vector2Int OcclusionMaskResolution { get; set; }
        /// <inheritdoc/>
        public bool CreateGameObjects { get; set; }
        /// <inheritdoc/>
        public bool AutoUpdate { get; set; }
        /// <inheritdoc/>
        public bool OrientScene { get; set; }
        /// <inheritdoc/>
        public SpatialAwarenessMeshLevelOfDetail WorldMeshLevelOfDetail { get; set; }

        #endregion IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        #region IMixedRealityOnDemandObserver

        /// <inheritdoc/>
        public float FirstUpdateDelay { get; set; }

        /// <inheritdoc/>
        public void UpdateOnDemand()
        {
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled || (SpatialAwarenessSystem == null))
            {
                return;
            }

            observerState = ObserverState.GetScene;
        }

        public bool UpdateOnceOnLoad { get; set; }

        #endregion IMixedRealityOnDemandObserver

        #region Public Profile
        public Material DefaultMaterial { get; set; } // Need references so they are included for runtime
        public Material DefaultWorldMeshMaterial { get; set; } // Need references so they are included for runtime

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


        #endregion Profile

        #region Private Fields

        private Dictionary<System.Guid, SpatialAwarenessSceneObject> sceneObjects = new Dictionary<System.Guid, SpatialAwarenessSceneObject>(256);
        private GameObject observedObjectParent = null;
        protected virtual GameObject ObservedObjectParent => observedObjectParent ?? (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObservationParent("WindowsMixedRealitySceneUnderstandingObserver"));
        private System.Timers.Timer firstUpdateTimer = null;
        private System.Timers.Timer updateTimer = null;
        private Dictionary<Guid, Tuple<SceneQuad, SceneObject>> cachedSceneQuads = new Dictionary<Guid, Tuple<SceneQuad, SceneObject>>(256);
        private ConcurrentQueue<SpatialAwarenessSceneObject> instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
        private TextAsset serializedScene = null;
        private byte[] sceneBytes;
        private Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;
        private CancellationTokenSource killTokenSource = new System.Threading.CancellationTokenSource();
        private System.Numerics.Matrix4x4 correctOrientation = System.Numerics.Matrix4x4.Identity;
        private List<SpatialAwarenessSceneObject> convertedObjects = new List<SpatialAwarenessSceneObject>(256);
        private enum ObserverState
        {
            Idle = 0,
            GetScene,
            GetSceneTransform,
            Working
        }
        private ObserverState observerState;
        private CancellationToken killToken;
        private bool isRemoting;
        //private bool doUpdateOnceOnLoad = false;
        private Guid sceneOriginId;
        private System.Numerics.Matrix4x4 sceneToWorldXformSystem;
        private List<SceneObject> filteredSelectedSurfaceTypesResult = new List<SceneObject>(128);
        private Texture defaultTexture;

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
        public bool TryFindCentermostPlacement(Guid quadGuid, Vector2 objExtents, out Vector3 placementPosOnPlane)
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

            // best placement origin is top left (2d sheet of paper)
            centerPosition -= quad.Extents / new System.Numerics.Vector2(2.0f);

            var centerUnity = new Vector3(centerPosition.X, centerPosition.Y, 0);
            placementPosOnPlane = (sceneObject.GetLocationAsMatrix() * sceneToWorldXformSystem * correctOrientation).ToUnity().MultiplyPoint(centerUnity);

            return true;
        }

        #endregion Public Methods

        #region Private

        private MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> sceneEventData = null;

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationAdded(spatialEventData);
            };

        /// <summary>
        /// Sends SceneObject Added event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being created</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectAdded(SpatialAwarenessSceneObject sceneObj, Guid id)
        {
            // Send the mesh removed event
            sceneEventData.Initialize(this, id, sceneObj);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectAdded);
        }

        private void StartUpdateTimers()
        {
            // setup and start service timers

            updateTimer = new System.Timers.Timer
            {
                Interval = Math.Max(UpdateInterval, Mathf.Epsilon) * 1000.0, // convert to milliseconds
            };

            updateTimer.Elapsed += (sender, e) =>
            {
                //if (AutoUpdate || doUpdateOnceOnLoad)
                if (AutoUpdate)
                {
                    observerState = ObserverState.GetScene;
                    //doUpdateOnceOnLoad = false;
                }
            };

            firstUpdateTimer = new System.Timers.Timer()
            {
                Interval = Math.Max(FirstUpdateDelay, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                AutoReset = false
            };

            // After an initial delay, start a load once or the auto update
            firstUpdateTimer.Elapsed += (sender, e) =>
            {
                updateTimer.Start();
            };

            firstUpdateTimer.Start();
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
                //1, 3, 0,
                //3, 2, 0
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(quadTriangles, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, new List<Vector2>(quadUVs));
        }

        private async Task RunObserverAsync(CancellationToken cancellationToken)
        {
            Scene scene = null;
            Scene previousScene = null;
            var sasos = new List<SpatialAwarenessSceneObject>(256);

            while (!cancellationToken.IsCancellationRequested)
            {
                switch (observerState)
                {
                    case ObserverState.Idle:
                        await new WaitForUpdate();
                        continue;

                    case ObserverState.GetScene:
                        observerState = ObserverState.Working;

                        await new WaitForBackgroundThread();
                        {
                            scene = GetSceneAsync(previousScene);

                            previousScene = scene;

                            sceneOriginId = scene.OriginSpatialGraphNodeId;
                        }
                        await new WaitForUpdate();

                        sceneToWorldXformSystem = GetSceneToWorldTransform();

                        if (!UsePersistentObjects)
                        {
                            ClearObservations();
                        }

                        if (OrientScene && Application.isEditor)
                        {
                            var toUp = System.Numerics.Vector3.Zero;

                            await new WaitForBackgroundThread();
                            {
                                toUp = ToUpFromBiggestFloor(scene.SceneObjects);
                            }
                            await new WaitForUpdate();

                            var floorNormalUnity = new Vector3(toUp.X, toUp.Y, toUp.Z);

                            // Get the rotation between the floor normal and Unity world's up vector.
                            var upRotation = Quaternion.FromToRotation(floorNormalUnity, Vector3.down);
                            correctOrientation = Matrix4x4.TRS(Vector3.zero, upRotation, Vector3.one).ToSystemNumerics();
                        }

                        await new WaitForBackgroundThread();
                        {
                            sasos = await ConvertSceneObjectsAsync(scene);
                        }
                        await new WaitForUpdate();

                        await new WaitForBackgroundThread();
                        {
                            AddUniqueTo(sasos, instantiationQueue);
                        }
                        await new WaitForUpdate();

                        // Add new objects to observer
                        // notify subscribers of event

                        foreach (var saso in sasos)
                        {
                            if (!sceneObjects.ContainsKey(saso.Guid))
                            {
                                sceneObjects.Add(saso.Guid, saso);
                                SendSceneObjectAdded(saso, saso.Guid);
                            }
                        }

                        if (observerState == ObserverState.Working)
                        {
                            observerState = ObserverState.Idle;
                        }

                        continue;

                    default:
                        await new WaitForUpdate();
                        continue;
                }
            }
        }

        private System.Numerics.Matrix4x4 GetSceneToWorldTransform()
        {
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
                Debug.LogError("Getting coordinate system failed!");
            }

            return result;
        }

        private Scene GetSceneAsync(Scene previousScene)
        {
            Scene scene = null;

            if (Application.isEditor && ShouldLoadFromFile)
            {
                if (sceneBytes == null)
                {
                    Debug.LogError("sceneBytes is null!");
                }

                // Move onto a background thread for the expensive scene loading stuff

                if (UsePersistentObjects && previousScene != null)
                {
                    scene = Scene.Deserialize(sceneBytes, previousScene);
                }
                else
                {
                    // This happens first time through as we have no history yet
                    scene = Scene.Deserialize(sceneBytes);
                }
            }
            else
            {
                SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
                {
                    EnableSceneObjectQuads = RequestPlaneData,
                    EnableSceneObjectMeshes = RequestMeshData,
                    EnableOnlyObservedSceneObjects = !InferRegions,
                    EnableWorldMesh = SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World),
                    RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(WorldMeshLevelOfDetail)
                };

                // Ideally you'd call SceneObserver.ComputeAsync() like this:
                // scene = await SceneObserver.ComputeAsync(...);
                // however this has has been problematic (buggy?)
                // For the time being we force it to be synchronous with the ...GetAwaiter().GetResult() pattern

                if (UsePersistentObjects)
                {
                    if (previousScene != null)
                    {
                        scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene).GetAwaiter().GetResult();
                    }
                    else
                    {
                        // first time through, we have no history
                        scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
                }
            }

            return scene;
        }

        private SpatialAwarenessSceneObject ConvertSceneObject(SceneObject sceneObject)
        {
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

            System.Numerics.Matrix4x4 worldXformSystem = sceneObject.GetLocationAsMatrix() * sceneToWorldXformSystem * correctOrientation;

            System.Numerics.Vector3 worldTranslationSystem;
            System.Numerics.Quaternion worldRotationSytem;
            System.Numerics.Vector3 localScale;

            System.Numerics.Matrix4x4.Decompose(worldXformSystem, out localScale, out worldRotationSytem, out worldTranslationSystem);

            var result = new SpatialAwarenessSceneObject(
                sceneObject.Id,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                worldTranslationSystem.ToUnityVector3(),
                worldRotationSytem.ToUnityQuaternion(),
                quads,
                meshes);

            return result;
        }

        private async Task<List<SpatialAwarenessSceneObject>> ConvertSceneObjectsAsync(Scene scene)
        {
            var result = new List<SpatialAwarenessSceneObject>(256);

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

        private void ReadProfile()
        {
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

            AutoUpdate = profile.AutoUpdate;
            UpdateOnceOnLoad = profile.UpdateOnceOnLoad;
            DefaultMaterial = profile.DefaultMaterial;
            DefaultWorldMeshMaterial = profile.DefaultWorldMeshMaterial;
            SurfaceTypes = profile.SurfaceTypes;
            RequestMeshData = profile.RequestMeshData;
            RequestPlaneData = profile.RequestPlaneData;
            InferRegions = profile.InferRegions;
            CreateGameObjects = profile.CreateGameObjects;
            UsePersistentObjects = profile.UsePersistentObjects;
            UpdateInterval = profile.UpdateInterval;
            FirstUpdateDelay = profile.FirstUpdateDelay;
            ShouldLoadFromFile = profile.ShouldLoadFromFile;
            SerializedScene = profile.SerializedScene;
            WorldMeshLevelOfDetail = profile.WorldMeshLevelOfDetail;
            InstantiationBatchRate = profile.InstantiationBatchRate;
            ObservationExtents = profile.ObservationExtents;
            QueryRadius = profile.QueryRadius;
            RequestOcclusionMask = profile.RequestOcclusionMask;
            OcclusionMaskResolution = profile.OcclusionMaskResolution;
            OrientScene = profile.OrientScene;
        }

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
                layer = DefaultPhysicsLayer
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

                    if (saso.SurfaceType == SpatialAwarenessSurfaceTypes.World && DefaultWorldMeshMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultWorldMeshMaterial;
                    }

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            return;
        }

        private Color ColorForSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            // shout-out to solarized!

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
            if (observedObjectParent != null)
            {
                int kidCount = observedObjectParent.transform.childCount;

                for (int i = 0; i < kidCount; ++i)
                {
                    UnityEngine.Object.Destroy(observedObjectParent.transform.GetChild(i).gameObject);
                }
            }
        }

        public override void ClearObservations()
        {
            base.ClearObservations();
            //cachedSceneQuads.Clear();
            CleanupDebugGameObjects();
            instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
            sceneObjects.Clear();
        }

        public void SaveScene(string prefix)
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
                EnableSceneObjectQuads = true,
                EnableSceneObjectMeshes = true,
                EnableOnlyObservedSceneObjects = false,
                EnableWorldMesh = true,
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
                    throw new NotImplementedException();
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
        private System.Numerics.Vector3 ToUpFromBiggestFloor(IReadOnlyList<SceneObject> sasos)
        {
            float areaForlargestFloorSoFar = 0;
            SceneObject floorSceneObject = null;
            SceneQuad floorQuad = null;

            var result = System.Numerics.Vector3.Zero;

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

                result = System.Numerics.Vector3.Cross(p21, p31);
            }

            return result;
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

            for (int i = 0; i < vlength; ++i)
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

        public void LoadScene(byte[] serializedScene)
        {
            throw new NotImplementedException();
        }

        #endregion Private Methods
    }
}
