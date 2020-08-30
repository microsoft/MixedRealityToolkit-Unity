// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#if SCENE_UNDERSTANDING_PRESENT
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.SceneUnderstanding;
using Microsoft.Windows.Perception.Spatial;
using Microsoft.Windows.Perception.Spatial.Preview;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
#endif // SCENE_UNDERSTANDING_PRESENT

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Scene Understanding Observer",
        "Experimental/WindowsSceneUnderstanding/Profiles/DefaultSceneUnderstandingObserverProfile.asset",
        "MixedRealityToolkit.Providers",
        true)]
    public class WindowsSceneUnderstandingObserver :
        BaseSpatialObserver,
        IMixedRealitySpatialAwarenessSceneUnderstandingObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsSceneUnderstandingObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        {
            ReadProfile();
        }

        /// <summary>
        /// Reads the observer's configuration profile.
        /// </summary>
        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                return;
            }

            SceneUnderstandingObserverProfile profile = ConfigurationProfile as SceneUnderstandingObserverProfile;
            if (profile == null)
            {
                Debug.LogError("Windows Scene Understanding Observer's configuration profile must be a SceneUnderstandingObserverProfile.");
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

#if SCENE_UNDERSTANDING_PRESENT

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
                Debug.LogError("Something went wrong getting scene observer access!");
            }

            if (UpdateOnceOnLoad)
            {
                observerState = ObserverState.GetScene;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            isRemoting = UnityEngine.XR.WSA.HolographicRemoting.ConnectionState == UnityEngine.XR.WSA.HolographicStreamerConnectionState.Connected;

            // Terminate the background thread when we stop in editor.
            cancelToken = cancelTokenSource.Token;

            // there is odd behavior with WaitForBackgroundTask
            // it will sometimes run on the main thread unless we start it this way
            var x = RunObserverAsync(cancelToken).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (instantiationQueue.Count > 0)
            {
                // Make our new objects in batches and tell observers about it
                int batchCount = Math.Min(InstantiationBatchRate, instantiationQueue.Count);

                for (int i = 0; i < batchCount; ++i)
                {
                    if (instantiationQueue.TryDequeue(out SpatialAwarenessSceneObject saso) && CreateGameObjects)
                    {
                        InstantiateSceneObject(saso);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            cancelTokenSource.Cancel();
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
                CleanupInstantiatedSceneObjects();
            }

            disposed = true;
        }

        #endregion BaseService

#endif // SCENE_UNDERSTANDING_PRESENT

        #region IMixedRealitySpatialAwarenessObserver

        /// <inheritdoc/>
        public override void Resume()
        {
#if SCENE_UNDERSTANDING_PRESENT
            updateTimer.Enabled = true;
#endif // SCENE_UNDERSTANDING_PRESENT
        }

        /// <inheritdoc/>
        public override void Suspend()
        {
#if SCENE_UNDERSTANDING_PRESENT
            if (updateTimer != null)
            {
                updateTimer.Enabled = false;
            }
#endif // SCENE_UNDERSTANDING_PRESENT
        }

        #endregion IMixedRealitySpatialAwarenessObserver

        #region IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        /// <inheritdoc/>
        public IReadOnlyDictionary<Guid, SpatialAwarenessSceneObject> SceneObjects
        {
#if SCENE_UNDERSTANDING_PRESENT
            get => sceneObjects;
#else // SCENE_UNDERSTANDING_PRESENT
            get => null;
#endif // SCENE_UNDERSTANDING_PRESENT
        }

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

        /// <inheritdoc/>
        public void LoadScene(byte[] serializedScene)
        {
            // todo
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SaveScene(string filenamePrefix)
        {
#if WINDOWS_UWP
            SaveToFile(filenamePrefix);
#else // WINDOWS_UWP
            Debug.LogWarning("SaveScene() only supported at runtime! Ignoring request.");
#endif // WINDOWS_UWP
        }

        #endregion IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        #region IMixedRealityOnDemandObserver

        /// <inheritdoc/>
        public float FirstUpdateDelay { get; set; }

        /// <inheritdoc/>
        public void UpdateOnDemand()
        {
#if SCENE_UNDERSTANDING_PRESENT
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled || (SpatialAwarenessSystem == null))
            {
                return;
            }

            observerState = ObserverState.GetScene;
#endif // SCENE_UNDERSTANDING_PRESENT
        }

        /// <inheritdoc />
        public bool UpdateOnceOnLoad { get; set; }

#endregion IMixedRealityOnDemandObserver

        #region Public Profile

        /// <summary>
        /// 
        /// </summary>
        public Material DefaultMaterial { get; set; } // Need references so they are included for runtime
        
        /// <summary>
        /// 
        /// </summary>
        public Material DefaultWorldMeshMaterial { get; set; } // Need references so they are included for runtime

        private byte[] sceneBytes;

        private TextAsset serializedScene = null;

        /// <summary>
        /// The saved scene understanding file
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

#if SCENE_UNDERSTANDING_PRESENT

        #region Private Fields

        private Dictionary<Guid, SpatialAwarenessSceneObject> sceneObjects = new Dictionary<Guid, SpatialAwarenessSceneObject>(256);
        private GameObject observedObjectParent = null;
        protected virtual GameObject ObservedObjectParent => observedObjectParent != null ? observedObjectParent : (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObservationParent("WindowsMixedRealitySceneUnderstandingObserver"));
        private System.Timers.Timer firstUpdateTimer = null;
        private System.Timers.Timer updateTimer = null;
        private Dictionary<Guid, Tuple<SceneQuad, SceneObject>> cachedSceneQuads = new Dictionary<Guid, Tuple<SceneQuad, SceneObject>>(256);
        private ConcurrentQueue<SpatialAwarenessSceneObject> instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
        private Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
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
        private CancellationToken cancelToken;
        private bool isRemoting;
        private Guid sceneOriginId;
        private System.Numerics.Matrix4x4 sceneToWorldTransformMatrix;
        private List<SceneObject> filteredSelectedSurfaceTypesResult = new List<SceneObject>(128);
        private Texture defaultTexture;

        #endregion Private Fields

#endif // SCENE_UNDERSTANDING_PRESENT

        #region Public Methods

        /// <summary>
        /// Gets the occlusion mask from a scene quad
        /// </summary>
        /// <param name="quadGuid">Guid of the quad</param>
        /// <param name="textureWidth">Width of the mask</param>
        /// <param name="textureHeight">Height of the mask</param>
        /// <param name="mask">Mask result</param>
        /// <returns></returns>
        public bool TryGetOcclusionMask(
            Guid quadGuid, 
            ushort textureWidth, 
            ushort textureHeight, 
            out byte[] mask)
        {
            mask = null;

#if SCENE_UNDERSTANDING_PRESENT
           Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadGuid, out result))
            {
               return false;
            }

            SceneQuad quad = result.Item1;
            SceneObject sceneObject = result.Item2;

            byte[] maskResult = new byte[textureWidth * textureHeight];
            quad.GetSurfaceMask(textureWidth, textureHeight, maskResult);
            mask = maskResult;

            return true;
#else
            return false;
#endif // SCENE_UNDERSTANDING_PRESENT
        }

        /// <summary>
        /// Returns best placement position in local space to the quad
        /// </summary>
        /// <param name="quadGuid">The Guid of quad that will be used for placement</param>
        /// <param name="objExtents">Total width and height of object to be placed in meters.</param>
        /// <param name="placementPosOnQuad">Base position on plane in local space.</param>
        /// <returns>returns <see cref="false"/> if API returns null.</returns>
        public bool TryFindCentermostPlacement(
            Guid quadGuid, 
            Vector2 objExtents, 
            out Vector3 placementPosOnQuad)
        {
            placementPosOnQuad = Vector3.zero;

#if SCENE_UNDERSTANDING_PRESENT

            Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadGuid, out result))
            {
                placementPosOnQuad = Vector2.zero;
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
            placementPosOnQuad = (sceneObject.GetLocationAsMatrix() * sceneToWorldTransformMatrix * correctOrientation).ToUnity().MultiplyPoint(centerUnity);

            return true;
#else
            return false;
#endif // SCENE_UNDERSTANDING_PRESENT
        }

        #endregion Public Methods

#if SCENE_UNDERSTANDING_PRESENT

        #region Private

        private MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> sceneEventData = null;

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationAdded(spatialEventData);
            };

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectUpdated =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationUpdated(spatialEventData);
            };

        /// <summary>
        /// Sends SceneObject Added event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being created</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectAdded(SpatialAwarenessSceneObject sceneObj, Guid id)
        {
            sceneEventData.Initialize(this, id, sceneObj);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectAdded);
        }

        /// <summary>
        /// Sends SceneObject Updated event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being updated</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectUpdated(SpatialAwarenessSceneObject sceneObj, Guid id)
        {
            sceneEventData.Initialize(this, id, sceneObj);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectUpdated);
        }

        /// <summary>
        /// Sets up and starts update timers
        /// </summary>
        private void StartUpdateTimers()
        {
            updateTimer = new System.Timers.Timer
            {
                Interval = Math.Max(UpdateInterval, Mathf.Epsilon) * 1000.0, // convert to milliseconds
            };

            updateTimer.Elapsed += (sender, e) =>
            {
                if (AutoUpdate)
                {
                    observerState = ObserverState.GetScene;
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

        /// <summary>
        /// Creates a quad based on extents
        /// </summary>
        /// <param name="mesh">Mesh to contain the quad</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
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

        /// <summary>
        /// Runs the observer asynchronously
        /// </summary>
        /// <param name="cancellationToken">CancellationToken of the task</param>
        /// <returns>The async task</returns>
        private async Task RunObserverAsync(CancellationToken cancellationToken)
        {
            Scene scene = null;
            Scene previousScene = null;
            List<SpatialAwarenessSceneObject> sasos;

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

                        sceneToWorldTransformMatrix = GetSceneToWorldTransform();

                        if (!UsePersistentObjects)
                        {
                            ClearObservations();
                        }

                        if (OrientScene && Application.isEditor)
                        {
                            System.Numerics.Vector3 toUp;

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
                            else
                            {
                                sceneObjects[saso.Guid] = saso;
                                SendSceneObjectUpdated(saso, saso.Guid);
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

        /// <summary>
        /// Gets the matrix representing the transform from scene space to world space
        /// </summary>
        /// <returns>The transform matrix</returns>
        private System.Numerics.Matrix4x4 GetSceneToWorldTransform()
        {
            var result = System.Numerics.Matrix4x4.Identity;

            if (Application.isEditor && !isRemoting)
            {
                return result;
            }

            SpatialCoordinateSystem sceneOrigin = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(sceneOriginId);
#pragma warning disable 618
            var nativePtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
#pragma warning restore 618
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

        /// <summary>
        /// Gets scene asynchronously from file or SceneObserver
        /// </summary>
        /// <param name="previousScene"></param>
        /// <returns>The retrieved scene</returns>
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

                if (UsePersistentObjects && previousScene != null)
                {
                    scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene).GetAwaiter().GetResult();
                }
                else
                {
                    scene = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius).GetAwaiter().GetResult();
                }
            }

            return scene;
        }

        /// <summary>
        /// Converts a <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObject"/> to a MRTK/platform agnostic <see cref="SpatialAwarenessSceneObject"/>.
        /// </summary>
        /// <param name="sceneObject">The <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObject"/> to convert</param>
        /// <returns>The converted <see cref="SpatialAwarenessSceneObject"/></returns>
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

                    var extents = new Vector2(sceneQuad.Extents.X, sceneQuad.Extents.Y);

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
                for (int i = 0; i < meshCount; ++i)
                {
                    var meshData = MeshData(sceneObject.Meshes[i]);
                    meshes.Add(meshData);
                }
            }

            // World space conversion
            System.Numerics.Matrix4x4 worldTransformMatrix = sceneObject.GetLocationAsMatrix() * sceneToWorldTransformMatrix * correctOrientation;

            System.Numerics.Vector3 worldTranslationSystem;
            System.Numerics.Quaternion worldRotationSytem;
            System.Numerics.Vector3 localScale;

            System.Numerics.Matrix4x4.Decompose(worldTransformMatrix, out localScale, out worldRotationSytem, out worldTranslationSystem);

            var result = new SpatialAwarenessSceneObject(
                sceneObject.Id,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                worldTranslationSystem.ToUnityVector3(),
                worldRotationSytem.ToUnityQuaternion(),
                quads,
                meshes);

            return result;
        }

        /// <summary>
        /// Converts all SurfaceType-matching <see cref="SceneObject"/>s in a scene to <see cref="SpatialAwarenessSceneObject"/>s.
        /// </summary>
        /// <param name="scene">The scene containing <see cref="SceneObject"/>s to convert</param>
        /// <returns>Task containing the resulting list of converted <see cref="SpatialAwarenessSceneObject"/>s</returns>
        private async Task<List<SpatialAwarenessSceneObject>> ConvertSceneObjectsAsync(Scene scene)
        {
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

        /// <summary>
        /// Filters SceneObject of selected SurfaceTypes
        /// </summary>
        /// <param name="newObjects">List of SceneObjects to be filtered</param>
        /// <returns>The filtered list</returns>
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

        /// <summary>
        /// Adds new SpatialAwarenessSceneObjects to the existing queue
        /// </summary>
        /// <param name="newObjects">List of SpatialAwarenessSceneObjects to be added</param>
        /// <param name="existing">The queue where new SpatialAwarenessSceneObjects will be added</param>
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

        /// <summary>
        /// Instantiate a SceneObject in the scene
        /// </summary>
        /// <param name="saso">The SpatialAwarenessSceneObject to instantiate</param>
        private void InstantiateSceneObject(SpatialAwarenessSceneObject saso)
        {
            // Until this point the SASO has been a data representation
            surfaceTypeName = $"{saso.SurfaceType} {saso.Guid}";

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

                    var meshGo = new GameObject($"Mesh {meshAlias.guid}");

                    var meshFilter = meshGo.AddComponent<MeshFilter>();
                    meshFilter.mesh = UnityMeshFromMeshData(meshAlias);

                    var meshRenderer = meshGo.AddComponent<MeshRenderer>();

                    meshGo.AddComponent<MeshCollider>();

                    if (DefaultMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultMaterial;
                        meshRenderer.material.color = ColorForSurfaceType(saso.SurfaceType);
                    }

                    if (saso.SurfaceType == SpatialAwarenessSurfaceTypes.World && DefaultWorldMeshMaterial)
                    {
                        meshRenderer.sharedMaterial = DefaultWorldMeshMaterial;
                    }

                    meshGo.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surfaceType"></param>
        /// <returns></returns>
        // todo: this should be in a demo scene and not in the observer itself
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
                case SpatialAwarenessSurfaceTypes.Inferred:
                    return new Color32(42, 161, 152, 255); // cyan
                default:
                    return new Color32(220, 50, 47, 255); // red
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textureBytes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Destroy the instantiated SceneObjects
        /// </summary>
        private void CleanupInstantiatedSceneObjects()
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

        /// <inheritdoc />
        public override void ClearObservations()
        {
            base.ClearObservations();
            //cachedSceneQuads.Clear();
            CleanupInstantiatedSceneObjects();
            instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
            sceneObjects.Clear();
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
        /// Generates Unity Mesh from MeshData
        /// </summary>
        /// <param name="meshData">The MeshData to get data from</param>
        /// <returns>The resulting Unity Mesh</returns>
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

        /// <summary>
        /// Generates MeshData from SceneMesh
        /// </summary>
        /// <param name="sceneMesh">The SceneMesh to get data from</param>
        /// <returns>The resulting MeshData</returns>
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
            var vertices = new Vector3[sceneMesh.VertexCount];
            var uvs = new Vector2[sceneMesh.VertexCount];

            var meshVertices = new System.Numerics.Vector3[sceneMesh.VertexCount];
            sceneMesh.GetVertexPositions(meshVertices);

            var vertexCount = meshVertices.Length;

            var minx = meshVertices[0].X;
            var miny = meshVertices[0].Y;
            var maxx = minx;
            var maxy = miny;

            for (int i = 0; i < vertexCount; ++i)
            {
                var x = meshVertices[i].X;
                var y = meshVertices[i].Y;

                vertices[i] = new Vector3(x, y, -meshVertices[i].Z);
                minx = Math.Min(minx, x);
                miny = Math.Min(miny, y);
                maxx = Math.Max(maxx, x);
                maxy = Math.Max(maxy, y);
            }

            // UVs - planar square projection

            float smallestDimension = Math.Min(minx, miny);
            float biggestDimension = Math.Max(maxx, maxy);

            for (int i = 0; i < vertexCount; ++i)
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
                    return SpatialAwarenessSurfaceTypes.Inferred;

                case SceneObjectKind.Unknown:
                default:
                    return SpatialAwarenessSurfaceTypes.Unknown;
            }
        }

        #endregion Private

#endif // SCENE_UNDERSTANDING_PRESENT
    }
}
