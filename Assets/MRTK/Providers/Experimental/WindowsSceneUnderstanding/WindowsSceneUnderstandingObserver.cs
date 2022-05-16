// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.SceneUnderstanding;
#if MSFT_OPENXR
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.OpenXR.Remoting;
using Microsoft.MixedReality.Toolkit.XRSDK;
using UnityEngine.XR.OpenXR;
#endif // MSFT_OPENXR
#if WINDOWS_UWP
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
#endif // WINDOWS_UWP
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental
{
    /// <summary>
    /// A Spatial Awareness observer with Scene Understanding capabilities. 
    /// </summary>
    /// <remarks>
    /// Only works with HoloLens 2 and Unity 2019.4+
    /// </remarks>
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Scene Understanding Observer",
        "Experimental/WindowsSceneUnderstanding/Profiles/DefaultSceneUnderstandingObserverProfile.asset",
        "MixedRealityToolkit.Providers",
        true)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/scene-understanding")]
    public class WindowsSceneUnderstandingObserver :
        BaseSpatialObserver,
        IMixedRealitySceneUnderstandingObserver
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
            UpdateOnceInitialized = profile.UpdateOnceInitialized;
            DefaultPhysicsLayer = profile.DefaultPhysicsLayer;
            DefaultMaterial = profile.DefaultMaterial;
            DefaultWorldMeshMaterial = profile.DefaultWorldMeshMaterial;
            SurfaceTypes = profile.SurfaceTypes;
            RequestMeshData = profile.RequestMeshData;
            RequestPlaneData = profile.RequestPlaneData;
            InferRegions = profile.InferRegions;
            CreateGameObjects = profile.CreateGameObjects;
            UsePersistentObjects = profile.UsePersistentObjects;
            UpdateInterval = profile.UpdateInterval;
            FirstAutoUpdateDelay = profile.FirstAutoUpdateDelay;
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

        #region IMixedRealityService

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        /// <inheritdoc />
        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        /// <inheritdoc />
        public override void Initialize()
        {
#if !(UNITY_WSA && SCENE_UNDERSTANDING_PRESENT)
            if (Application.isPlaying)
            {
                Debug.LogWarning("The required package Microsoft.MixedReality.SceneUnderstanding is not installed or properly configured. Please visit https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/scene-understanding for more information.");
            }
#else
            base.Initialize();
#if MSFT_OPENXR
            isOpenXRLoaderActive = LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>() ?? false;
            isOpenXRRemotingConnected = AppRemoting.TryGetConnectionState(out ConnectionState state, out _) && state == ConnectionState.Connected;
#elif WINDOWS_UWP
            isOpenXRLoaderActive = false;
#endif // MSFT_OPENXR
            sceneEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>(EventSystem.current);
            CreateQuadFromExtents(normalizedQuadMesh, 1, 1);

            SceneObserverAccessStatus accessStatus = Task.Run(RequestAccess).GetAwaiter().GetResult();
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

            if (UpdateOnceInitialized)
            {
                observerState = ObserverState.GetScene;
            }
#endif // !(UNITY_WSA && SCENE_UNDERSTANDING_PRESENT)
        }

        /// <inheritdoc />
        public override void Enable()
        {
#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            base.Enable();
            // Terminate the background thread when we stop in editor.
            cancelToken = cancelTokenSource.Token;

            task = Task.Run(() => RunObserverAsync(cancelToken)).ContinueWith(t =>
            {
                Debug.LogError($"{t.Exception.InnerException.GetType().Name}: {t.Exception.InnerException.Message} {t.Exception.InnerException.StackTrace}");
            }, TaskContinuationOptions.OnlyOnFaulted);
#else
            IsEnabled = false;
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        /// <inheritdoc />
        public override void Update()
        {
            if (instantiationQueue.Count > 0)
            {
                // Make our new objects in batches and tell observers about it
                int batchCount = CreateGameObjects ? Math.Min(InstantiationBatchRate, instantiationQueue.Count) : instantiationQueue.Count;

                for (int i = 0; i < batchCount; ++i)
                {
                    if (instantiationQueue.TryDequeue(out SpatialAwarenessSceneObject saso) && CreateGameObjects)
                    {
                        InstantiateSceneObject(saso);
                        SendSceneObjectAdded(saso, saso.Id);
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

#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        #endregion IMixedRealityService

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

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

#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        #region IMixedRealitySpatialAwarenessObserver

        /// <inheritdoc/>
        public override void Resume()
        {
#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            updateTimer.Enabled = true;
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

        /// <inheritdoc/>
        public override void Suspend()
        {
#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            if (updateTimer != null)
            {
                updateTimer.Enabled = false;
            }
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

        #endregion IMixedRealitySpatialAwarenessObserver

        #region IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        /// <inheritdoc/>
        public IReadOnlyDictionary<int, SpatialAwarenessSceneObject> SceneObjects
        {
#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            get => sceneObjects;
#else // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            get => null;
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
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
        public void SaveScene(string filenamePrefix)
        {
#if WINDOWS_UWP && SCENE_UNDERSTANDING_PRESENT
            Task.Run(() => SaveToFile(filenamePrefix)).ContinueWith(t =>
            {
                Debug.LogError($"{t.Exception.InnerException.GetType().Name}: {t.Exception.InnerException.Message} {t.Exception.InnerException.StackTrace}");
            }, TaskContinuationOptions.OnlyOnFaulted);
#else // WINDOWS_UWP && SCENE_UNDERSTANDING_PRESENT
            Debug.LogWarning("SaveScene() only supported at runtime! Ignoring request.");
#endif // WINDOWS_UWP && SCENE_UNDERSTANDING_PRESENT
        }

        #endregion IMixedRealitySpatialAwarenessSceneUnderstandingObserver

        #region IMixedRealityOnDemandObserver

        /// <inheritdoc/>
        public float FirstAutoUpdateDelay { get; set; }

        /// <inheritdoc/>
        public void UpdateOnDemand()
        {
#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled || (Service == null))
            {
                return;
            }

            observerState = ObserverState.GetScene;
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

        /// <inheritdoc />
        public bool UpdateOnceInitialized { get; set; }

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

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        #region Private Fields
        private Task task;
        private readonly Dictionary<int, SpatialAwarenessSceneObject> sceneObjects = new Dictionary<int, SpatialAwarenessSceneObject>(256);
        private System.Timers.Timer firstUpdateTimer = null;
        private System.Timers.Timer updateTimer = null;
        private Dictionary<int, Tuple<SceneQuad, SceneObject>> cachedSceneQuads = new Dictionary<int, Tuple<SceneQuad, SceneObject>>(256);
        private ConcurrentQueue<SpatialAwarenessSceneObject> instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
        private readonly Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;
        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private System.Numerics.Matrix4x4 correctOrientation = System.Numerics.Matrix4x4.Identity;
        private readonly List<SpatialAwarenessSceneObject> convertedObjects = new List<SpatialAwarenessSceneObject>(256);
        private readonly Dictionary<int, Guid> IdToGuidLookup = new Dictionary<int, Guid>();

        private enum ObserverState
        {
            Idle = 0,
            GetScene,
            GetSceneTransform,
            Working
        }

        private ObserverState observerState;
        private CancellationToken cancelToken;
        private Guid sceneOriginId;
        private System.Numerics.Matrix4x4 sceneToWorldTransformMatrix;
        private List<SceneObject> filteredSelectedSurfaceTypesResult = new List<SceneObject>(128);
        private Texture defaultTexture;
#if WINDOWS_UWP || MSFT_OPENXR
        private bool isOpenXRLoaderActive;
#endif // WINDOWS_UWP || MSFT_OPENXR
#if MSFT_OPENXR
        private bool isOpenXRRemotingConnected;
#endif // MSFT_OPENXR

        #endregion Private Fields

#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

        #region Public Methods

        /// <summary>
        /// Gets the occlusion mask from a scene quad
        /// </summary>
        /// <param name="quadId">Guid of the quad</param>
        /// <param name="textureWidth">Width of the mask</param>
        /// <param name="textureHeight">Height of the mask</param>
        /// <param name="mask">Mask result</param>
        /// <returns>Returns false if fails to get the mask</returns>
        public bool TryGetOcclusionMask(
            int quadId,
            ushort textureWidth,
            ushort textureHeight,
            out byte[] mask)
        {
            mask = null;

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
           Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadId, out result))
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
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

        /// <inheritdoc/>
        public bool TryFindCentermostPlacement(
            int quadId,
            Vector2 objExtents,
            out Vector3 placementPosOnQuad)
        {
            placementPosOnQuad = Vector3.zero;

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

            Tuple<SceneQuad, SceneObject> result;

            if (!cachedSceneQuads.TryGetValue(quadId, out result))
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
#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
        }

        #endregion Public Methods

#if SCENE_UNDERSTANDING_PRESENT && UNITY_WSA

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

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectRemoved =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationRemoved(spatialEventData);
            };

        /// <summary>
        /// Sends SceneObject Added event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The SpatialAwarenessSceneObject being created</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectAdded(SpatialAwarenessSceneObject sceneObj, int id)
        {
            sceneEventData.Initialize(this, id, sceneObj);
            Service?.HandleEvent(sceneEventData, OnSceneObjectAdded);
        }

        /// <summary>
        /// Sends SceneObject Updated event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The SpatialAwarenessSceneObject being updated</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectUpdated(SpatialAwarenessSceneObject sceneObj, int id)
        {
            sceneEventData.Initialize(this, id, sceneObj);
            Service?.HandleEvent(sceneEventData, OnSceneObjectUpdated);
        }

        /// <summary>
        /// Sends SceneObject Removed event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="id">the id associated with the removed SceneObject</param>
        protected virtual void SendSceneObjectRemoved(int id)
        {
            sceneEventData.Initialize(this, id, null);
            Service?.HandleEvent(sceneEventData, OnSceneObjectRemoved);
        }

        private async Task<SceneObserverAccessStatus> RequestAccess()
        {
            return await SceneObserver.RequestAccessAsync();
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

            // If AutoUpdate we set up the timer to wait until we pass FirstAutoUpdateDelay before starting the first automatic update
            if (AutoUpdate)
            {
                firstUpdateTimer = new System.Timers.Timer()
                {
                    Interval = Math.Max(FirstAutoUpdateDelay, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                    AutoReset = false
                };

                // After an initial delay, start a load once or the auto update
                firstUpdateTimer.Elapsed += (sender, e) =>
                {
                    updateTimer.Start();
                    observerState = ObserverState.GetScene;
                };
                firstUpdateTimer.Start();
            }
            // If AutoUpdate is false then can we start the update timer right away.
            // Note we still want to start this timer in case the user set AutoUpdate to true later
            else
            {
                updateTimer.Start();
            }
        }

        /// <summary>
        /// Creates a quad based on extents
        /// </summary>
        /// <param name="mesh">Mesh to contain the quad</param>
        /// <param name="x">Length of the quad</param>
        /// <param name="y">Width of the quad</param>
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
                // 1, 3, 0,
                // 3, 2, 0
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
                        while (CreateGameObjects && instantiationQueue.Count > 0)
                        {
                            await new WaitForUpdate();
                        }
                        await new WaitForBackgroundThread();
                        {
                            scene = await GetSceneAsync(previousScene);
                            previousScene = scene;
                            sceneOriginId = scene.OriginSpatialGraphNodeId;
                        }
                        await new WaitForUpdate();

                        System.Numerics.Matrix4x4? transformResult = GetSceneToWorldTransform();
                        if (transformResult.HasValue)
                        {
                            sceneToWorldTransformMatrix = transformResult.Value;
                        }
                        else
                        {
                            await new WaitForUpdate();
                            observerState = ObserverState.GetScene;
                            continue;
                        }
                        
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
                            if (!sceneObjects.ContainsKey(saso.Id))
                            {
                                sceneObjects.Add(saso.Id, saso);
                                
                                // If creating GameObjects, delay the sending of the event until the creation is finished
                                if (!CreateGameObjects)
                                {
                                    SendSceneObjectAdded(saso, saso.Id);
                                }
                            }
                            else
                            {
                                if (CreateGameObjects)
                                {
                                    UpdateInstantiatedSceneObject(sceneObjects[saso.Id], saso);
                                }
                                sceneObjects[saso.Id] = saso;
                                SendSceneObjectUpdated(saso, saso.Id);
                            }
                        }

                        List<int> removedSasoIds = new List<int>();
                        foreach (var saso in sceneObjects.Values)
                        {
                            if (!sasos.Contains(saso))
                            {
                                removedSasoIds.Add(saso.Id);
                            }
                        }

                        foreach (var id in removedSasoIds)
                        {
                            SendSceneObjectRemoved(id);
                            if (CreateGameObjects)
                            {
                                UpdateInstantiatedSceneObject(sceneObjects[id], null);
                            }
                            sceneObjects.Remove(id);
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
        private System.Numerics.Matrix4x4? GetSceneToWorldTransform()
        {
            var result = System.Numerics.Matrix4x4.Identity;
#if WINDOWS_UWP // On HoloLens 2 device
            if (isOpenXRLoaderActive)
#elif MSFT_OPENXR // In editor and using OpenXR
            if (isOpenXRLoaderActive && isOpenXRRemotingConnected && !ShouldLoadFromFile)
#else // All other cases
            if (false)
#endif // WINDOWS_UWP
            {
#if MSFT_OPENXR
                SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(sceneOriginId);
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    result = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one).ToSystemNumerics();
                }
                else
                {
                    return null;
                }
#endif // MSFT_OPENXR
            }
            else
            {
#if WINDOWS_UWP
                SpatialCoordinateSystem sceneOrigin = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(sceneOriginId);
                SpatialCoordinateSystem worldOrigin = WindowsMixedReality.WindowsMixedRealityUtilities.SpatialCoordinateSystem;

                var sceneToWorld = sceneOrigin.TryGetTransformTo(worldOrigin);

                if (sceneToWorld.HasValue)
                {
                    result = sceneToWorld.Value; // numerics
                }
                else
                {
                    return null;
                }
#endif // WINDOWS_UWP
            }
            return result;
        }

        /// <summary>
        /// Gets scene asynchronously from file or SceneObserver
        /// </summary>
        /// <param name="previousScene">The previous scene</param>
        /// <returns>The retrieved scene</returns>
        private async Task<Scene> GetSceneAsync(Scene previousScene)
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
                    EnableWorldMesh = SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.World),
                    RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(WorldMeshLevelOfDetail)
                };

                if (UsePersistentObjects && previousScene != null)
                {
                    scene = await SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene);
                }
                else
                {
                    scene = await SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius);
                }
            }

            return scene;
        }

        /// <summary>
        /// Converts a <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObject"/> to a MRTK/platform agnostic SpatialAwarenessSceneObject.
        /// </summary>
        /// <param name="sceneObject">The <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneObject"/> to convert</param>
        /// <returns>The converted SpatialAwarenessSceneObject.</returns>
        private SpatialAwarenessSceneObject ConvertSceneObject(SceneObject sceneObject)
        {
            int quadCount = sceneObject.Quads.Count;
            int meshCount = sceneObject.Meshes.Count;

            List<SpatialAwarenessSceneObject.QuadData> quads = new List<SpatialAwarenessSceneObject.QuadData>(quadCount);
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

                    int hashedQuadId = quadIdKey.GetHashCode();
                    var quad = new SpatialAwarenessSceneObject.QuadData
                    { 
                        Id = hashedQuadId,
                        Extents = extents,
                        OcclusionMask = occlusionMaskBytes
                    };
                    
                    if (IdToGuidLookup.ContainsKey(hashedQuadId) && IdToGuidLookup[hashedQuadId] != quadIdKey)
                    {
                        Debug.LogWarning("Possible collision");
                    }
                    IdToGuidLookup[hashedQuadId] = quadIdKey;
                    quads.Add(quad);

                    // Store a cache so we can retrieve best position on plane later.

                    if (!cachedSceneQuads.ContainsKey(hashedQuadId))
                    {
                        cachedSceneQuads.Add(hashedQuadId, new Tuple<SceneQuad, SceneObject>(sceneQuad, sceneObject));
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
            System.Numerics.Quaternion worldRotationSystem;

            System.Numerics.Matrix4x4.Decompose(worldTransformMatrix, out _, out worldRotationSystem, out worldTranslationSystem);

            int hashedId = sceneObject.Id.GetHashCode();
            var result = SpatialAwarenessSceneObject.Create(
                hashedId,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                worldTranslationSystem.ToUnityVector3(),
                worldRotationSystem.ToUnityQuaternion(),
                quads,
                meshes);
            if (IdToGuidLookup.ContainsKey(hashedId) && IdToGuidLookup[hashedId] != sceneObject.Id)
            {
                Debug.LogWarning("Possible collision");
            }
            IdToGuidLookup[hashedId] = sceneObject.Id;

            return result;
        }

        /// <summary>
        /// Converts all SurfaceType-matching <see cref="SceneObject"/>s in a scene to SpatialAwarenessSceneObjects.
        /// </summary>
        /// <param name="scene">The scene containing <see cref="SceneObject"/>s to convert</param>
        /// <returns>Task containing the resulting list of converted SpatialAwarenessSceneObjects.</returns>
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
                if (!SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceType(newObjects[i].Kind)))
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
        /// <param name="existingQueue">The queue where new SpatialAwarenessSceneObjects will be added</param>
        private void AddUniqueTo(List<SpatialAwarenessSceneObject> newObjects, ConcurrentQueue<SpatialAwarenessSceneObject> existingQueue)
        {
            int length = newObjects.Count;

            for (int i = 0; i < length; ++i)
            {
                if (!sceneObjects.ContainsKey(newObjects[i].Id))
                {
                    existingQueue.Enqueue(newObjects[i]);
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
            surfaceTypeName = $"{saso.SurfaceType} {saso.Id}";

            saso.GameObject = new GameObject(surfaceTypeName)
            {
                layer = DefaultPhysicsLayer
            };

            saso.GameObject.transform.SetParent(ObservedObjectParent.transform);

            saso.GameObject.transform.localPosition = saso.Position;
            saso.GameObject.transform.localRotation = saso.Rotation;
            saso.GameObject.transform.localScale = Vector3.one;

            // Make GameObjects for Quads and Meshes
            if (RequestPlaneData)
            {
                // Add MeshFilter, attach shared quad and scale it
                // later, we can update scale of existing quads if they change size
                // (as opposed to modifying the vertices directly, when persisting objects)
                int quadCount = saso.Quads.Count;

                for (int i = 0; i < quadCount; ++i)
                {
                    var quad = saso.Quads[i];
                    InstantiateQuad(saso, quad);
                }
            }

            if (RequestMeshData)
            {
                int meshCount = saso.Meshes.Count;

                for (int i = 0; i < meshCount; ++i)
                {
                    var meshAlias = saso.Meshes[i];
                    InstantiateMesh(saso, meshAlias);
                }
            }
        }

        /// <summary>
        /// Instantiate a Mesh GameObject in the scene
        /// </summary>
        /// <param name="saso">The SpatialAwarenessSceneObject containing the mesh to instantiate</param>
        /// <param name="mesh">The MeshData object to instantiate</param>
        private void InstantiateMesh(SpatialAwarenessSceneObject saso, SpatialAwarenessSceneObject.MeshData mesh)
        {
            var meshGo = new GameObject($"Mesh {mesh.Id}");
            mesh.GameObject = meshGo;

            var meshFilter = meshGo.AddComponent<MeshFilter>();
            meshFilter.mesh = UnityMeshFromMeshData(mesh);

            var meshRenderer = meshGo.AddComponent<MeshRenderer>();

            meshGo.AddComponent<MeshCollider>();

            if (DefaultMaterial)
            {
                meshRenderer.sharedMaterial = DefaultMaterial;
            }

            if (saso.SurfaceType == SpatialAwarenessSurfaceTypes.World && DefaultWorldMeshMaterial)
            {
                meshRenderer.sharedMaterial = DefaultWorldMeshMaterial;
            }

            meshGo.transform.SetParent(saso.GameObject.transform, false);
        }

        /// <summary>
        /// Instantiate a Quad GameObject in the scene
        /// </summary>
        /// <param name="saso">The SpatialAwarenessSceneObject containing the quad to instantiate</param>
        /// <param name="quad">The Quad object to instantiate</param>
        private void InstantiateQuad(SpatialAwarenessSceneObject saso, SpatialAwarenessSceneObject.QuadData quad)
        {
            var quadGo = new GameObject($"Quad {quad.Id}");
            quad.GameObject = quadGo;

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
            }

            if (RequestOcclusionMask)
            {
                if (quad.OcclusionMask != null)
                {
                    var occlusionTexture = OcclusionTexture(quad.OcclusionMask);
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
            quadGo.transform.localScale = new UnityEngine.Vector3(quad.Extents.x, quad.Extents.y, 0);
        }

        /// <summary>
        /// Update an instantiated SpatialAwarenessSceneObject in the scene
        /// </summary>
        /// <param name="existingSaso">The existing SpatialAwarenessSceneObject in the scene</param>
        /// <param name="newSaso">The new SpatialAwarenessSceneObject with updated info</param>
        private void UpdateInstantiatedSceneObject(SpatialAwarenessSceneObject existingSaso, SpatialAwarenessSceneObject newSaso)
        {
            if (newSaso == null)
            {
                GameObject.Destroy(existingSaso.GameObject);
                return;
            }
            
            newSaso.GameObject = existingSaso.GameObject;
            newSaso.GameObject.transform.localPosition = newSaso.Position;
            newSaso.GameObject.transform.localRotation = newSaso.Rotation;

            // Update GameObjects for Quads and Meshes
            if (RequestPlaneData)
            {
                int i = 0;
                while (i < existingSaso.Quads.Count && i < newSaso.Quads.Count)
                {
                    var gameObject = newSaso.Quads[i].GameObject = existingSaso.Quads[i].GameObject;
                    gameObject.name = $"Quad {newSaso.Quads[i].Id}";
                    gameObject.transform.localScale = new Vector3(newSaso.Quads[i].Extents.x, newSaso.Quads[i].Extents.y, 0);
                    if (RequestOcclusionMask && 
                        ((existingSaso.Quads[i].OcclusionMask == null || newSaso.Quads[i].OcclusionMask == null)
                        || !existingSaso.Quads[i].OcclusionMask.SequenceEqual(newSaso.Quads[i].OcclusionMask)))
                    {
                        var meshRender = newSaso.Quads[i].GameObject.GetComponent<MeshRenderer>();
                        meshRender.enabled = true;
                        if (newSaso.Quads[i].OcclusionMask != null)
                        {
                            var occlusionTexture = OcclusionTexture(newSaso.Quads[i].OcclusionMask);
                            meshRender.material.mainTexture = occlusionTexture;
                        }
                        else
                        {
                            meshRender.enabled = false;
                        }
                    }
                    i++;
                }

                if (existingSaso.Quads.Count < newSaso.Quads.Count)
                {
                    for (; i < newSaso.Quads.Count; i++)
                    {
                        InstantiateQuad(newSaso, newSaso.Quads[i]);
                    }

                }
                else
                {
                    for (; i < existingSaso.Quads.Count; i++)
                    {
                        GameObject.Destroy(existingSaso.Quads[i].GameObject);
                    }
                }

            }

            if (RequestMeshData)
            {
                int i = 0;
                while (i < existingSaso.Meshes.Count && i < newSaso.Meshes.Count)
                {
                    var gameObject = newSaso.Meshes[i].GameObject = existingSaso.Meshes[i].GameObject;
                    gameObject.name = $"Mesh {newSaso.Meshes[i].Id}";
                    if (DefaultWorldMeshMaterial && existingSaso.SurfaceType != newSaso.SurfaceType)
                    {
                        if (newSaso.SurfaceType == SpatialAwarenessSurfaceTypes.World)
                        {
                            newSaso.Meshes[i].GameObject.GetComponent<MeshRenderer>().sharedMaterial = DefaultWorldMeshMaterial;
                        }
                        else if (existingSaso.SurfaceType == SpatialAwarenessSurfaceTypes.World)
                        {
                            newSaso.Meshes[i].GameObject.GetComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;
                        }
                    }
                    if (!existingSaso.Meshes[i].UVs.SequenceEqual(newSaso.Meshes[i].UVs) ||
                        !existingSaso.Meshes[i].Indices.SequenceEqual(newSaso.Meshes[i].Indices) ||
                        !existingSaso.Meshes[i].Vertices.SequenceEqual(newSaso.Meshes[i].Vertices))
                    {
                        var meshFilter = newSaso.Meshes[i].GameObject.GetComponent<MeshFilter>();
                        meshFilter.mesh = UnityMeshFromMeshData(newSaso.Meshes[i]);
                    }
                    i++;
                }

                if (existingSaso.Meshes.Count < newSaso.Meshes.Count)
                {
                    for (; i < newSaso.Meshes.Count; i++)
                    {
                        InstantiateMesh(newSaso, newSaso.Meshes[i]);
                    }

                }
                else
                {
                    for (; i < existingSaso.Meshes.Count; i++)
                    {
                        GameObject.Destroy(existingSaso.Meshes[i].GameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Generates occlusion texture from the occlusion mask
        /// </summary>
        /// <param name="textureBytes">The occlusion mask to use</param>
        /// <returns>The generated texture</returns>
        private Texture2D OcclusionTexture(byte[] textureBytes)
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
            if (ObservedObjectParent != null)
            {
                int kidCount = ObservedObjectParent.transform.childCount;

                for (int i = 0; i < kidCount; ++i)
                {
                    UnityEngine.Object.Destroy(ObservedObjectParent.transform.GetChild(i).gameObject);
                }
            }
        }

        /// <inheritdoc />
        public override void ClearObservations()
        {
            base.ClearObservations();
            cachedSceneQuads.Clear();
            CleanupInstantiatedSceneObjects();
            instantiationQueue = new ConcurrentQueue<SpatialAwarenessSceneObject>();
            foreach (var sceneObject in sceneObjects)
            {
                SendSceneObjectRemoved(sceneObject.Key);
            }
            sceneObjects.Clear();
            IdToGuidLookup.Clear();
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

            var serializedScene = await SceneObserver.ComputeSerializedAsync(sceneQuerySettings, QueryRadius);
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
        /// <param name="levelOfDetail">The <see cref="SpatialAwarenessMeshLevelOfDetail"/> to convert.</param>
        /// <returns>The equivalent <see cref="Microsoft.MixedReality.SceneUnderstanding"/> <see cref="SceneMeshLevelOfDetail"/></returns>
        private SceneMeshLevelOfDetail LevelOfDetailToMeshLOD(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            switch (levelOfDetail)
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
            if (meshData.Vertices.Length > 65535)
            {
                unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            unityMesh.SetVertices(new List<Vector3>(meshData.Vertices));
            unityMesh.SetIndices(meshData.Indices, MeshTopology.Triangles, 0);
            unityMesh.SetUVs(0, new List<Vector2>(meshData.UVs));
            unityMesh.RecalculateNormals();

            return unityMesh;
        }

        /// <summary>
        /// Orients the root game object, such that the Scene Understanding floor lies on the Unity world's X-Z plane.
        /// The floor type with the largest area is chosen as the reference.
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

            int hashedId = sceneMesh.Id.GetHashCode();
            var result = new SpatialAwarenessSceneObject.MeshData
            {
                Indices = indices,
                Vertices = vertices,
                Id = hashedId,
                UVs = uvs
            };
            if (IdToGuidLookup.ContainsKey(hashedId) && IdToGuidLookup[hashedId] != sceneMesh.Id)
            {
                Debug.LogWarning("Possible collision");
            }
            IdToGuidLookup[hashedId] = sceneMesh.Id;

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

#endif // SCENE_UNDERSTANDING_PRESENT && UNITY_WSA
    }
}
