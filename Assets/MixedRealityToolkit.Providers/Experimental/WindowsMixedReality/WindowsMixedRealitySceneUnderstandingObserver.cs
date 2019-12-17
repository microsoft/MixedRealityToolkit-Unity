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

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Experimental.SpatialAwareness
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Scene Understanding Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessSceneUnderstandingObserverProfile.asset",
        "MixedRealityToolkit.SDK")]
    public class WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver : BaseSpatialSceneObserver, IMixedRealityOnDemandObserver, IWindowsMixedRealitySceneUnderstanding
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealitySpatialAwarenessSceneUnderstandingObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, spatialAwarenessSystem, name, priority, profile)
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
            base.Initialize();
            MeshExtensions.CreateMeshFromQuad(normalizedQuadMesh, 1, 1);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            firstUpdateTimer = new Timer()
            {
                Interval = Math.Max(FirstUpdateDelay, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                AutoReset = false
            };

            updateTimer = new Timer
            {
                Interval = Math.Max(UpdateInterval, Mathf.Epsilon) * 1000.0, // convert to milliseconds
                AutoReset = false
            };

            if (AutoUpdate)
            {
                updateTimer.AutoReset = true;
            };

            // After an initial delay, start the auto update
            firstUpdateTimer.Elapsed += (sender, e) =>
            {
                updateTimer.Start();
            };

            updateTimer.Elapsed += UpdateTimerEventHandler;

            firstUpdateTimer.Start();

            sceneNeedsAlignment = true;
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (canUpdateScene)
            {
                canUpdateScene = false;
                GetScene(); // this is async void, we don't wait on it.
            }

            if (instantiationQueue.Count > 0)
            {
                Debug.Log($"Got {instantiationQueue.Count} things to instantiate.");

                // Make our new objects in batches and tell observers about it
                int batchCount = Math.Min(InstantiationBatchRate, instantiationQueue.Count);

                for (int i = 0; i < batchCount; ++i)
                {
                    var saso = instantiationQueue.Dequeue();

                    Debug.Log($"I see {saso.Quads.Count} quads");
                    Assert.IsTrue(saso.Quads.Count > 0);

                    // Create GameObjects for our data
                    Instantiate(saso);

                    if (!sceneObjects.ContainsKey(saso.Guid))
                    {
                        sceneObjects.Add(saso.Guid, saso);
                        SendSceneObjectAdded(saso, saso.Guid);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            canUpdateScene = false;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
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
                CleanupSceneObjects();
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
            updateTimer.Enabled = false;
        }

        #endregion IMixedRealitySpatialAwarenessObserver

        #region IMixedRealityOnDemandObserver

        /// <inheritdoc/>
        public bool AutoUpdate { get; set; }

        public float FirstUpdateDelay { get; set; }

        /// <inheritdoc/>
        public void UpdateOnDemand()
        {
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled || (SpatialAwarenessSystem == null))
            {
                return;
            }


            canUpdateScene = true;
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

        private Timer firstUpdateTimer = null;
        private Timer updateTimer = null;
        private SceneQuerySettings lastQuerySettings;
        private Dictionary<Guid, SceneMesh> cachedSceneMeshes = new Dictionary<Guid, SceneMesh>(128);
        private Dictionary<Guid, SceneQuad> cachedSceneQuads = new Dictionary<Guid, SceneQuad>(128);
        private Scene previousScene = null;
        private Queue<SpatialAwarenessSceneObject> instantiationQueue = new Queue<SpatialAwarenessSceneObject>();
        private TextAsset serializedScene = null;
        private byte[] sceneBytes;
        private bool canGetScene = true;
        private bool canUpdateScene = false;
        private bool sceneNeedsAlignment;
        private Mesh normalizedQuadMesh = new Mesh();
        private string surfaceTypeName;

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
        /// <summary>
        /// Get a byte array representing the occlusion mask of a plane
        /// </summary>
        /// <param name="saso">The <see cref="SpatialAwarenessMeshObject"/> who's mask to return</param>
        /// <param name="width"><see cref="ushort"/> representing the width of the texture mask to be returned</param>
        /// <param name="height"><see cref="ushort"/> representing the height of the texture mask to be returned</param>
        /// <param name="mask">The plane's occlusion mask as a <see cref="byte[]"/> to generate a texture.</param>
        /// <returns>returns <see cref="false"/> if API returns null.</returns>
        public bool TryGetPlaneValidationMask(SpatialAwarenessSceneObject.Quad quad, ushort textureWidth, ushort textureHeight, out byte[] mask)
        {
            mask = null;

            SceneQuad associatedQuad;
            cachedSceneQuads.TryGetValue(quad.guid, out associatedQuad);

            if (associatedQuad == null)
            {
                return false;
            }

            mask = new byte[textureWidth * textureHeight];

            associatedQuad.GetSurfaceMask(textureWidth, textureHeight, mask);
            return true;
        }

        /// <summary>
        /// Returns best placement position in local space to the plane
        /// </summary>
        /// <param name="plane">The <see cref="SpatialAwarenessMeshObject"/> who's plane will be used for placement</param>
        /// <param name="objExtents">Total width and height of object to be placed in meters.</param>
        /// <param name="placementPosOnPlane">Base position on plane in local space.</param>
        /// <returns>returns <see cref="false"/> if API returns null.</returns>
        public bool TryGetBestPlacementPosition(SpatialAwarenessSceneObject.Quad quad, Vector2 objExtents, out Vector2 placementPosOnPlane)
        {
            placementPosOnPlane = Vector2.zero;

            SceneQuad associatedQuad;
            cachedSceneQuads.TryGetValue(quad.guid, out associatedQuad);

            if (associatedQuad == null)
            {
                return false;
            }

            System.Numerics.Vector2 ext = new System.Numerics.Vector2(objExtents.x, objExtents.y);
            System.Numerics.Vector2 pos = new System.Numerics.Vector2();

            associatedQuad.FindCentermostPlacement(ext, out pos);

            placementPosOnPlane.Set(pos.X, pos.Y);

            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private async void GetScene()
        {
            // Prevent a flood of auto-update messages
            // finish getting whole scene before getting another
            if (canGetScene)
            {
                Debug.Log("GetScene()");
                canGetScene = false;
                await GetSceneAsync();
                canGetScene = true;
            }
        }

        private SpatialAwarenessSceneObject ConvertSceneObject(SceneObject sceneObject)
        {
            int meshCount = sceneObject.Meshes.Count;
            int quadCount = sceneObject.Quads.Count;

            List<SpatialAwarenessSceneObject.MeshData> meshes = new List<SpatialAwarenessSceneObject.MeshData>(meshCount);
            List<SpatialAwarenessSceneObject.Quad> quads = new List<SpatialAwarenessSceneObject.Quad>(quadCount);

            if (GeneratePlanes)
            {
                SceneQuad sceneQuad = null;

                for (int i = 0; i < meshCount; ++i)
                {
                    sceneQuad = sceneObject.Quads[i];

                    var extents = new Vector2(sceneQuad.Extents.X, sceneQuad.Extents.Y);
                    var quad = new SpatialAwarenessSceneObject.Quad(sceneObject.Id, extents);
                    quads.Add(quad);

                    var key = sceneQuad.Id;
                    if (!cachedSceneQuads.ContainsKey(key))
                    {
                        cachedSceneQuads.Add(key, sceneQuad);
                    }
                }
            }

            if (GenerateMeshes)
            {
                SceneMesh sceneMesh = null;

                for (int i = 0; i < quadCount; ++i)
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

            var result = new SpatialAwarenessSceneObject(
                sceneObject.Id,
                SpatialAwarenessSurfaceType(sceneObject.Kind),
                sceneObject.GetLocationAsMatrix(),
                quads,
                meshes);

            return result;
        }

        private List<SpatialAwarenessSceneObject> convertedResult = new List<SpatialAwarenessSceneObject>(64);

        private List<SpatialAwarenessSceneObject> ConvertSceneObjects(List<SceneObject> sceneObjects)
        {
            Debug.Log("Started converting");
            Assert.IsTrue(sceneObjects.Count > 0);

            convertedResult.Clear();

            int sceneObjectCount = sceneObjects.Count;

            for (int i = 0; i < sceneObjectCount; ++i)
            {
                var saso = ConvertSceneObject(sceneObjects[i]);
                convertedResult.Add(saso);
            }

            return convertedResult;
        }

        private async Task GetSceneAsync()
        {
            Scene scene = null;

            var sasos = new List<SpatialAwarenessSceneObject>();

            if (ShouldLoadFromFile)
            {
                // For this particular workflow
                // Build DemoSpatialAwareness example, save bytes
                // Take bytes off device
                // Specify bytes file in MRTK profile
                // If running on device (vs editor) save bytes to unity StreamingAssets
                // Now the bytes file will be loaded from file

                if (!SerializedScene)
                {
                    Debug.LogError("SceneUnderstandingObserver GetSceneAsync() SerializedScene is null!");
                    return;
                }

                // Move onto a background thread for the expensive scene loading stuff

                await new WaitForBackgroundThread();
                {
                    try
                    {
                        scene = GetSceneWithBytes(sceneBytes);
                        var validSurfaceTypesSU = FilterSelectedSurfaceTypes(scene.SceneObjects);
                        sasos = ConvertSceneObjects(validSurfaceTypesSU);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                await new WaitForEndOfFrame();

                Assert.IsNotNull(scene);

                // Orient data so floor with largest area is aligned to XZ plane
                if (sceneNeedsAlignment)
                {
                    Quaternion sceneRotation = AlignedRotationWithScene(sasos);
                    ObservedObjectParent.transform.rotation = sceneRotation;
                    sceneNeedsAlignment = false;
                }

                if (!UsePersistentObjects)
                {
                    CleanupSceneObjects();
                }

                AddUniqueTo(sasos, instantiationQueue);

                return;
            }

            var canAccess = await CanAccessObserver();

            if (canAccess)
            {
                SceneQuerySettings sceneQuerySettings = new SceneQuerySettings()
                {
                    EnableSceneObjectQuads = GeneratePlanes,
                    EnableSceneObjectMeshes = GenerateMeshes,
                    EnableOnlyObservedSceneObjects = RenderInferredRegions,
                    EnableWorldMesh = GenerateEnvironmentMesh,
                    RequestedMeshLevelOfDetail = LevelOfDetailToMeshLOD(LevelOfDetail)
                };

                if (!UsePersistentObjects)
                {
                    CleanupSceneObjects();
                }

                Guid sceneGuid = new Guid();

                await new WaitForBackgroundThread();
                {
                    Task<Scene> task;

                    if (UsePersistentObjects)
                    {
                        if (previousScene != null)
                        {
                            task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius, previousScene);
                        }
                        else
                        {
                            // first time through, we have no history
                            task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius);
                        }
                    }
                    else
                    {
                        task = SceneObserver.ComputeAsync(sceneQuerySettings, QueryRadius);
                    }

                    try
                    {
                        await task;
                        scene = task.Result;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    Assert.IsNotNull(scene);

                    if (UsePersistentObjects)
                    {
                        previousScene = scene;
                    }

                    var validSurfaceTypes = FilterSelectedSurfaceTypes(scene.SceneObjects);

                    // store this so we don't have to reference scene outside this thread
                    sceneGuid = scene.OriginSpatialGraphNodeId;

                    sasos = ConvertSceneObjects(validSurfaceTypes);
                }

                // Return to the main thread
                await new WaitForEndOfFrame();

                Debug.Log($"Got {sasos.Count} objects");

                if (!UsePersistentObjects)
                {
                    CleanupSceneObjects();
                }

                UpdateObserverTransformWithSceneGuid(sceneGuid);

                AddUniqueTo(sasos, instantiationQueue);
            }
        }

        private void UpdateTimerEventHandler(object sender, ElapsedEventArgs args)
        {
            canUpdateScene = true;
            return;
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

            StartupBehavior = profile.StartupBehavior;
            AutoUpdate = profile.AutoUpdate;
            DefaultMaterial = profile.DefaultMaterial;
            SurfaceTypes = profile.SurfaceTypes;
            GenerateMeshes = profile.GenerateMeshes;
            GeneratePlanes = profile.GeneratePlanes;
            GenerateEnvironmentMesh = profile.GenerateEnvironmentMesh;
            UsePersistentObjects = profile.UsePersistentObjects;
            UpdateInterval = profile.UpdateInterval;
            FirstUpdateDelay = profile.FirstUpdateDelay;
            ShouldLoadFromFile = profile.ShouldLoadFromFile;
            SerializedScene = profile.SerializedScene;
            RenderInferredRegions = profile.RenderInferredRegions;
            LevelOfDetail = profile.LevelOfDetail;
            InstantiationBatchRate = profile.InstantiationBatchRate;
            ObservationExtents = profile.ObservationExtents;
            QueryRadius = profile.QueryRadius;
        }

        private async Task<bool> CanAccessObserver()
        {
            if (IsRunning)
            {
                return true;
            }

            if (SpatialAwarenessSystem == null)
            {
                return false;
            }

            if (CoreServices.CameraSystem.IsOpaque)
            {
                // We're not a HoloLens
                return false;
            }

            if (!IsRunning)
            {
                if (SceneObserver.IsSupported())
                {
                    try
                    {
                        var access = await SceneObserver.RequestAccessAsync();
                        if (access == SceneObserverAccessStatus.Allowed)
                        {
                            IsRunning = true;
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        throw;
                    }
                }
            }

            return false;
        }

        private void CleanupObserver()
        {
            Dispose(true);
        }

        private List<SceneObject> filteredListResult = new List<SceneObject>(128);

        private List<SceneObject> FilterSelectedSurfaceTypes(IReadOnlyList<SceneObject> newObjects)
        {
            filteredListResult.Clear();

            int count = newObjects.Count;

            for (int i = 0; i < count; ++i)
            {
                if (!SurfaceTypes.HasFlag(SpatialAwarenessSurfaceType(newObjects[i].Kind)))
                {
                    continue;
                }

                filteredListResult.Add(newObjects[i]);
            }

            return filteredListResult;
        }

        private void AddUniqueTo(List<SpatialAwarenessSceneObject> newObjects, Queue<SpatialAwarenessSceneObject> existing)
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

        private void UpdateObserverTransformWithSceneGuid(Guid sceneGuid)
        {
            System.Numerics.Matrix4x4 sceneToUnityTransform = GetSceneObjectToUnityTransform(sceneGuid);
            SetTransformFromMatrix4x4(ObservedObjectParent.transform, sceneToUnityTransform);
        }

        private Scene GetSceneWithBytes(byte[] sceneData)
        {
            Scene result = null;

            try
            {
                if (UsePersistentObjects && previousScene != null)
                {
                    result = Scene.Deserialize(sceneData, previousScene);
                }
                else
                {
                    // This happens first time through as we have no history yet
                    result = Scene.Deserialize(sceneData);
                }
                previousScene = result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return result;
        }

        private void Instantiate(SpatialAwarenessSceneObject saso)
        {
            // Until this point the SASO has been a data representation

            surfaceTypeName = saso.SurfaceType.ToString();

            saso.GameObject = new GameObject(surfaceTypeName)
            {
                layer = PhysicsLayer
            };

            SetTransformFromMatrix4x4(
                saso.GameObject.transform,
                SwapRuntimeAndUnityCoordinateSystem(saso.TransformMatrix),
                true);

            saso.GameObject.transform.SetParent(ObservedObjectParent.transform, false);

            // Maybe make GameObjects for Quads and Meshes

            if (!GeneratePlanes && !GenerateMeshes)
            {
                return;
            }

            if (GeneratePlanes)
            {
                // Add MeshFilter, attach shared quad and scale it
                // later, we can update scale of existing quads if they change size
                // (as opposed to modifying the vertexs directly, when persisting objects)
                int quadCount = saso.Quads.Count;

                for (int i = 0; i < quadCount; ++i)
                {
                    var go = new GameObject("Quad");

                    var mf = go.AddComponent<MeshFilter>();
                    mf.mesh = normalizedQuadMesh;

                    var mr = go.AddComponent<MeshRenderer>();

                    mr.sharedMaterial = DefaultMaterial;

                    var scale = new Vector3(saso.Quads[i].extents.x, saso.Quads[i].extents.y, 0);
                    go.transform.localScale = scale;

                    go.AddComponent<BoxCollider>();

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            if (GenerateMeshes)
            {
                int meshCount = saso.Meshes.Count;

                for (int i = 0; i < meshCount; ++i)
                {
                    var go = new GameObject("Mesh");

                    var mf = go.AddComponent<MeshFilter>();
                    mf.mesh = UnityMeshFromMeshData(saso.Meshes[i]);

                    var mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = DefaultMaterial;

                    go.AddComponent<MeshCollider>();

                    go.transform.SetParent(saso.GameObject.transform, false);
                }
            }

            return;
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

        private void CleanupSceneObjects()
        {
            int kidCount = ObservedObjectParent.transform.childCount;
            for (int i = 0; i < kidCount; ++i)
            {
                UnityEngine.Object.Destroy(ObservedObjectParent.transform.GetChild(i).gameObject);
            }

            cachedSceneQuads.Clear();
            cachedSceneMeshes.Clear();
            sceneObjects.Clear();
            instantiationQueue.Clear();
        }

#if WINDOWS_UWP
        /// <summary>
        /// Saves the <see cref="SceneUnderstanding.SceneProcessor"/>'s data stream as a file for later use
        /// </summary>
        /// <param name="sceneBuffer">the <see cref="byte[]"/></param>
        private void SaveToFile(byte[] sceneBuffer)
        {
            DateTime currDateTime = DateTime.Now;
            string currDateTimeString = currDateTime.Date.Year + "-" + currDateTime.Date.Month + "-" + currDateTime.Date.Day + "_" +
                                        currDateTime.TimeOfDay.Hours + "-" + currDateTime.TimeOfDay.Minutes + "-" + currDateTime.TimeOfDay.Seconds;

            folderLocation = ApplicationData.Current.LocalFolder;
            SUfile = folderLocation.CreateFileAsync("SU_" + currDateTimeString).GetAwaiter().GetResult();
            WriteSUFileAsync(SUfile, sceneBuffer);
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
                    Debug.Log("SceneUnderstanding LOD is set to custom, falling back.");
                    return SceneMeshLevelOfDetail.Medium;

                case SpatialAwarenessMeshLevelOfDetail.Coarse:
                    return SceneMeshLevelOfDetail.Coarse;

                case SpatialAwarenessMeshLevelOfDetail.Medium:
                default:
                    return SceneMeshLevelOfDetail.Medium;

                case SpatialAwarenessMeshLevelOfDetail.Fine:
                    return SceneMeshLevelOfDetail.Fine;
            }
        }

        /// <summary>
        /// Computes the <see cref="SceneObject"/> to a valid <see cref="System.Numerics.Matrix4x4?"/> in <see cref="UnityEngine"/> coordinate space.
        /// </summary>
        /// <param name="sceneId">the <see cref="Guid"/> representing the <see cref="SceneObject"/>.</param>
        /// <returns>A valid <see cref="System.Numerics.Matrix4x4?"/> consumable in <see cref="UnityEngine"/>.</returns>
        private static System.Numerics.Matrix4x4 GetSceneObjectToUnityTransform(Guid sceneId)
        {
#if WINDOWS_UWP
            SpatialCoordinateSystem sceneSpatialCoordinateSystem = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(sceneId);
            SpatialCoordinateSystem unitySpatialCoordinateSystem = (SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr());

            System.Numerics.Matrix4x4? sceneToUnityTransform = sceneSpatialCoordinateSystem.TryGetTransformTo(unitySpatialCoordinateSystem);

            if (sceneToUnityTransform.HasValue)
            {
                return SwapRuntimeAndUnityCoordinateSystem(sceneToUnityTransform.Value);
            }
#endif
            return System.Numerics.Matrix4x4.Identity;
        }

        /// <summary>
        /// Helper to convert from Right hand to Left hand coordinates using <see cref="System.Numerics.Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix">The Right handed <see cref="System.Numerics.Matrix4x4"/>.</param>
        /// <returns>The <see cref="System.Numerics.Matrix4x4"/> in left hand form.</returns>
        private static System.Numerics.Matrix4x4 SwapRuntimeAndUnityCoordinateSystem(System.Numerics.Matrix4x4 matrix)
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
        /// Takes in a transformation matrix and assigns it to a Unity transform.
        /// </summary>
        /// <param name="transformationMatrix">Transformation matrix.</param>
        /// <param name="unityTransform">Unity transform.</param>
        /// <param name="updateLocalTransformOnly">Whether to update local or absolute transform.</param>
        private static void SetTransformFromMatrix4x4(Transform unityTransform, System.Numerics.Matrix4x4 transformationMatrix, bool updateLocalTransformOnly = false)
        {
            Vector3 t;
            Quaternion r;
            Vector3 s;

            GetTRSFromMatrix4x4(transformationMatrix, out t, out r, out s);

            // NOTE: Scale is ignored.
            if (updateLocalTransformOnly)
            {
                unityTransform.localPosition = t;
                unityTransform.localRotation = r;
            }
            else
            {
                unityTransform.SetPositionAndRotation(t, r);
            }
        }

        private static void GetTRSFromMatrix4x4(System.Numerics.Matrix4x4 matrix, out Vector3 translation, out Quaternion rotation, out Vector3 scale)
        {
            System.Numerics.Vector3 t;
            System.Numerics.Vector3 s;
            System.Numerics.Quaternion r;

            System.Numerics.Matrix4x4.Decompose(matrix, out s, out r, out t);

            translation = new Vector3(t.X, t.Y, t.Z);
            rotation = new Quaternion(r.X, r.Y, r.Z, r.W);
            scale = new Vector3(s.X, s.Y, s.Z);
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
            unityMesh.RecalculateNormals();

            return unityMesh;
        }

        /// <summary>
        /// Orients the root game object, such that the Scene Understanding floor lies on the Unity world's X-Z plane.
        /// </summary>
        /// <param name="scene">Scene Understanding scene.</param>
        private Quaternion AlignedRotationWithScene(List<SpatialAwarenessSceneObject> sasos)
        {
            float areaForlargestFloorSoFar = 0;
            SpatialAwarenessSceneObject floorSceneObject = null;
            SpatialAwarenessSceneObject.Quad? floorQuad = null;

            // Find the largest floor quad.
            var count = sasos.Count;
            for (var i = 0; i < count; ++i)
            {
                if (sasos[i].SurfaceType == SpatialAwarenessSurfaceTypes.Floor)
                {
                    var quads = sasos[i].Quads;

                    Assert.IsNotNull(quads);

                    var qcount = quads.Count;
                    for (int j = 0; j < qcount; j++)
                    {
                        float quadArea = quads[j].extents.x * quads[j].extents.y;

                        if (quadArea > areaForlargestFloorSoFar)
                        {
                            areaForlargestFloorSoFar = quadArea;
                            floorSceneObject = sasos[i];
                            floorQuad = quads[j];
                        }
                    }
                }
            }

            if (floorQuad.HasValue)
            {
                // Compute the floor quad's normal.
                float halfWidthMeters = floorQuad.Value.extents.x * .5f;
                float halfHeightMeters = floorQuad.Value.extents.y * .5f;

                System.Numerics.Vector3 point1 = new System.Numerics.Vector3(-halfWidthMeters, -halfHeightMeters, 0);
                System.Numerics.Vector3 point2 = new System.Numerics.Vector3(halfWidthMeters, -halfHeightMeters, 0);
                System.Numerics.Vector3 point3 = new System.Numerics.Vector3(-halfWidthMeters, halfHeightMeters, 0);

                System.Numerics.Matrix4x4 floorTransform = floorSceneObject.TransformMatrix;

                floorTransform = SwapRuntimeAndUnityCoordinateSystem(floorTransform);

                System.Numerics.Vector3 tPoint1 = System.Numerics.Vector3.Transform(point1, floorTransform);
                System.Numerics.Vector3 tPoint2 = System.Numerics.Vector3.Transform(point2, floorTransform);
                System.Numerics.Vector3 tPoint3 = System.Numerics.Vector3.Transform(point3, floorTransform);

                System.Numerics.Vector3 p21 = tPoint2 - tPoint1;
                System.Numerics.Vector3 p31 = tPoint3 - tPoint1;

                System.Numerics.Vector3 floorNormal = System.Numerics.Vector3.Cross(p21, p31);

                // Numerics to Unity conversion.
                Vector3 floorNormalUnity = new Vector3(floorNormal.X, floorNormal.Y, floorNormal.Z);

                // Get the rotation between the floor normal and Unity world's up vector.
                return Quaternion.FromToRotation(floorNormalUnity, Vector3.up);
            }

            return Quaternion.identity;
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

            var vertices = new Vector3[sceneMesh.VertexCount];

            var meshVertices = new System.Numerics.Vector3[sceneMesh.VertexCount];
            sceneMesh.GetVertexPositions(meshVertices);

            var vlength = meshVertices.Length;

            for (int i = 0; i < vlength; ++i)
            {
                vertices[i] = new Vector3(meshVertices[i].X, meshVertices[i].Y, -meshVertices[i].Z);
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
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Background;

                case SceneObjectKind.Wall:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Wall;

                case SceneObjectKind.Floor:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Floor;

                case SceneObjectKind.Ceiling:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Ceiling;

                case SceneObjectKind.Platform:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Platform;

                case SceneObjectKind.World:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.World;

                case SceneObjectKind.Unknown:
                default:
                    return Toolkit.SpatialAwareness.SpatialAwarenessSurfaceTypes.Unknown;
            }
        }

        #endregion Private Methods
    }
}
