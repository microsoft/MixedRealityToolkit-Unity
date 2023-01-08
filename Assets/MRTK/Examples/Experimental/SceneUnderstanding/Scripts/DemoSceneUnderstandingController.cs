// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Examples.Demos; // DemoSpatialMeshHandler not found / Start OnEnable OnDisable OnDestroy no suitable method found to override
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness; // SpatialAwarenessSceneObject IMixedRealitySceneUnderstandingObserver not found
using Microsoft.MixedReality.Toolkit.SpatialAwareness; // important - IMixedRealitySpatialAwarenessObservationHandler MixedRealitySpatialAwarenessEventData SpatialAwarenessSurfaceTypes not found
using Microsoft.MixedReality.Toolkit.UI; // Interactable
using System.Collections.Generic; // IReadOnlyDictionary List Dictionary not found
using System;
using UnityEngine; // Color GameObject Transform SerializeFieldAttribute SerializeField Header HeaderAttribute
using TMPro; // TextMeshPros

namespace Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding
{
    /// <summary>
    /// Demo class to show different ways of visualizing the space using scene understanding.
    /// </summary>
    public class DemoSceneUnderstandingController : DemoSpatialMeshHandler, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        #region Private Fields

        #region Serialized Fields

        [SerializeField]
        private string SavedSceneNamePrefix = "DemoSceneUnderstanding";
        [SerializeField]
        private bool InstantiatePrefabs = false;
        [SerializeField]
        private GameObject InstantiatedPrefab = null;
        [SerializeField]
        private Transform InstantiatedParent = null;

        [Header("UI")]
        [SerializeField]
        private Interactable autoUpdateToggle = null;
        [SerializeField]
        private Interactable quadsToggle = null;
        [SerializeField]
        private Interactable inferRegionsToggle = null;
        [SerializeField]
        private Interactable meshesToggle = null;
        [SerializeField]
        private Interactable maskToggle = null;
        [SerializeField]
        private Interactable platformToggle = null;
        [SerializeField]
        private Interactable wallToggle = null;
        [SerializeField]
        private Interactable floorToggle = null;
        [SerializeField]
        private Interactable ceilingToggle = null;
        [SerializeField]
        private Interactable worldToggle = null;
        [SerializeField]
        private Interactable completelyInferred = null;
        [SerializeField]
        private Interactable backgroundToggle = null;

        #endregion Serialized Fields

        private IMixedRealitySceneUnderstandingObserver observer;

        private List<GameObject> instantiatedPrefabs;

        private Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>> observedSceneObjects;

        #endregion Private Fields

        #region MonoBehaviour Functions

        protected override void Start()
        {
            observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySceneUnderstandingObserver>();

            if (observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer! Please make sure the current build target is set to Universal Windows Platform. "
                    + "Visit https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/scene-understanding for more information.");
                return;
            }
            InitToggleButtonState();
            instantiatedPrefabs = new List<GameObject>();
            observedSceneObjects = new Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>>();
        }

        public void btn_MakeFocusOnFloorChanges() { MakeFocusOnFloorChanges("btn"); } 
        void MakeFocusOnFloorChanges(string source) {
            logInputStates("btn_MakeFocusOnFloorChanges before");
            if (meshesToggle.IsToggled)         { ToggleGenerateMeshes(false);      print(source + " - ToggleGenerateMeshes");      }   // saw log in console
            if (maskToggle.IsToggled)           { ToggleOcclusionMask(false);       print(source + " - ToggleOcclusionMask");       }   //     off so no console log
            if (inferRegionsToggle.IsToggled)   { ToggleInferRegions(false);        print(source + " - ToggleInferRegions");        }   //     off so no console log
            if (platformToggle.IsToggled)       { TogglePlatforms(false);           print(source + " - TogglePlatforms");           }   // saw log in console
            if (wallToggle.IsToggled)           { ToggleWalls(false);               print(source + " - ToggleWalls");               }   // saw log in console
            if (ceilingToggle.IsToggled)        { ToggleCeilings(false);            print(source + " - ToggleCeilings");            }   // saw log in console
            if (worldToggle.IsToggled)          { ToggleWorld(false);               print(source + " - ToggleWorld");               }   //     off so no console log
            if (completelyInferred.IsToggled)   { ToggleCompletelyInferred(false);  print(source + " - ToggleCompletelyInferred");  }   // saw log in console
            if (backgroundToggle.IsToggled)     { ToggleBackground(false);          print(source + " - ToggleBackground");          }   // saw log in console
            
            // from the SpatialAwarenessSample-Import, expect quads to be on. but check if it's off and enable
            if (!autoUpdateToggle.IsToggled)      { ToggleAutoUpdate();             print(source + " - ToggleAutoUpdate");          }   //     on so no console log
            // from the SpatialAwarenessSample-Import, expect quads to be off and we have to enable it
            if (!quadsToggle.IsToggled)           { ToggleGeneratePlanes(false);    print(source + " - ToggleGeneratePlanes");      }   // saw log in console
            ClearAndUpdateObserver();
            logInputStates("btn_MakeFocusOnFloorChanges after");
        }

        protected override void OnEnable()
        {
            RegisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        protected override void OnDisable()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        protected override void OnDestroy()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        #endregion MonoBehaviour Functions

        #region IMixedRealitySpatialAwarenessObservationHandler Implementations

        /// <inheritdoc />
        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This method called everytime a SceneObject created by the SU observer
            // The eventData contains everything you need do something useful

            AddToData(eventData.Id);

            if (observedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                sceneObjectDict.Add(eventData.Id, eventData.SpatialObject);
            }
            else
            {
                observedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }

            if (InstantiatePrefabs && eventData.SpatialObject.Quads.Count > 0)
            {
                var prefab = Instantiate(InstantiatedPrefab);
                prefab.transform.SetPositionAndRotation(eventData.SpatialObject.Position, eventData.SpatialObject.Rotation);
                float sx = eventData.SpatialObject.Quads[0].Extents.x;
                float sy = eventData.SpatialObject.Quads[0].Extents.y;
                prefab.transform.localScale = new Vector3(sx, sy, .1f);
                if (InstantiatedParent)
                {
                    prefab.transform.SetParent(InstantiatedParent);
                }
                instantiatedPrefabs.Add(prefab);
                printFloorCoordinates(eventData);
            }
            else
            {
                foreach (var quad in eventData.SpatialObject.Quads)
                {
                    quad.GameObject.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);
                }

            }
        }

        void printFloorCoordinates(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData) {
            string s = "";
            try {
                if (eventData.SpatialObject == null ) {
                    s = "\neventData.SpatialObject != null";
                    return;
                }
            } catch (Exception e) { s = "\n catch exception: eventData.SpatialObject == null"; }

            try {
                if ( eventData.SpatialObject.SurfaceType == null ) {
                    s = "\neventData.SpatialObject.SurfaceType != null";
                    return;
                }
            } catch (Exception e) { s = "\n catch exception: eventData.SpatialObject.SurfaceType != null"; }

            try {
                if ( (eventData.SpatialObject.SurfaceType == SpatialAwarenessSurfaceTypes.Floor)) {
                    s += "\nCreated floor prefab at " + eventData.SpatialObject.Position;
                } else {
                    s += "\nCreated non-floor prefab";
                }
            } catch (Exception e) { s = "\n catch exception: eventData.SpatialObject.SurfaceType == SpatialAwarenessSurfaceTypes.Floor"; }
            
            print(s);
        }

        /// <inheritdoc />
        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            UpdateData(eventData.Id);

            if (observedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                observedSceneObjects[eventData.SpatialObject.SurfaceType][eventData.Id] = eventData.SpatialObject;
            }
            else
            {
                observedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }
        }

        /// <inheritdoc />
        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            RemoveFromData(eventData.Id);

            foreach (var sceneObjectDict in observedSceneObjects.Values)
            {
                sceneObjectDict?.Remove(eventData.Id);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObservationHandler Implementations

        #region Public Functions

        /// <summary>
        /// Get all currently observed SceneObjects of a certain type.
        /// </summary>
        /// <remarks>
        /// Before calling this function, the observer should be configured to observe the specified type by including that type in the SurfaceTypes property.
        /// </remarks>
        /// <returns>A dictionary with the scene objects of the requested type being the values and their ids being the keys.</returns>
        public IReadOnlyDictionary<int, SpatialAwarenessSceneObject> GetSceneObjectsOfType(SpatialAwarenessSurfaceTypes type)
        {
            if (!observer.SurfaceTypes.IsMaskSet(type))
            {
                Debug.LogErrorFormat("The Scene Objects of type {0} are not being observed. You should add {0} to the SurfaceTypes property of the observer in advance.", type);
            }

            if (observedSceneObjects.TryGetValue(type, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjects))
            {
                return sceneObjects;
            }
            else
            {
                observedSceneObjects.Add(type, new Dictionary<int, SpatialAwarenessSceneObject>());
                return observedSceneObjects[type];
            }
        }

        #region UI Functions

        /// <summary>
        /// Request the observer to update the scene
        /// </summary>
        public void UpdateScene()
        {
            observer.UpdateOnDemand();
        }

        /// <summary>
        /// Request the observer to save the scene
        /// </summary>
        public void SaveScene()
        {
            observer.SaveScene(SavedSceneNamePrefix);
        }

        /// <summary>
        /// Request the observer to clear the observations in the scene
        /// </summary>
        public void ClearScene()
        {
            foreach (GameObject gameObject in instantiatedPrefabs)
            {
                Destroy(gameObject);
            }
            instantiatedPrefabs.Clear();
            observer.ClearObservations();
        }

        /// <summary>
        /// Change the auto update state of the observer
        /// </summary>
        public void ToggleAutoUpdate()
        {
            observer.AutoUpdate = !observer.AutoUpdate;
        }

        /// <summary>
        /// Change whether to request occlusion mask from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleOcclusionMask(bool shouldUpdate)
        {
            var observerMask = observer.RequestOcclusionMask;
            observer.RequestOcclusionMask = !observerMask;
            if (observer.RequestOcclusionMask)
            {
                if (!(observer.RequestPlaneData || observer.RequestMeshData))
                {
                    observer.RequestPlaneData = true;
                    quadsToggle.IsToggled = true;
                }
            }
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request plane data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleGeneratePlanes(bool shouldUpdate)
        {
            observer.RequestPlaneData = !observer.RequestPlaneData;
            if (observer.RequestPlaneData)
            {
                observer.RequestMeshData = false;
                meshesToggle.IsToggled = false;
            }
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request mesh data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleGenerateMeshes(bool shouldUpdate)
        {
            observer.RequestMeshData = !observer.RequestMeshData;
            if (observer.RequestMeshData)
            {
                observer.RequestPlaneData = false;
                quadsToggle.IsToggled = false;
            }
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request floor data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleFloors(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Floor);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request wall data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleWalls(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Wall);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request ceiling data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleCeilings(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Ceiling);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request platform data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void TogglePlatforms(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Platform);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request inferred region data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleInferRegions(bool shouldUpdate)
        {
            observer.InferRegions = !observer.InferRegions;
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request world mesh data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleWorld(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.World);

            if (observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.World))
            {
                // Ensure we requesting meshes
                observer.RequestMeshData = true;
                meshesToggle.GetComponent<Interactable>().IsToggled = true;
            }
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request background data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleBackground(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Background);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        /// <summary>
        /// Change whether to request completely inferred data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleCompletelyInferred(bool shouldUpdate)
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Inferred);
            if (shouldUpdate) { ClearAndUpdateObserver(); }
        }

        public void ToggleOcclusionMask()       { ToggleOcclusionMask(true); }
        public void ToggleGeneratePlanes()      { ToggleGeneratePlanes(true); }
        public void ToggleGenerateMeshes()      { ToggleGenerateMeshes(true); }
        public void ToggleFloors()              { ToggleFloors(true); }
        public void ToggleWalls()               { ToggleWalls(true); }
        public void ToggleCeilings()            { ToggleCeilings(true); }
        public void TogglePlatforms()           { TogglePlatforms(true); }
        public void ToggleInferRegions()        { ToggleInferRegions(true); }
        public void ToggleWorld()               { ToggleWorld(true); }
        public void ToggleBackground()          { ToggleBackground(true); }
        public void ToggleCompletelyInferred()  { ToggleCompletelyInferred(true); }


        #endregion UI Functions

        #endregion Public Functions

        #region Helper Functions

        private void InitToggleButtonState()
        {
            // Configure observer
            autoUpdateToggle.IsToggled = observer.AutoUpdate;
            quadsToggle.IsToggled = observer.RequestPlaneData;
            meshesToggle.IsToggled = observer.RequestMeshData;
            maskToggle.IsToggled = observer.RequestOcclusionMask;
            inferRegionsToggle.IsToggled = observer.InferRegions;

            // Filter display
            platformToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Platform);
            wallToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Wall);
            floorToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Floor);
            ceilingToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Ceiling);
            worldToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.World);
            completelyInferred.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Inferred);
            backgroundToggle.IsToggled = observer.SurfaceTypes.IsMaskSet(SpatialAwarenessSurfaceTypes.Background);

            logInputStates("Start / InitToggleButtonState");
        }

        void logInputStates(string source) {
            string s = source + 
            $"\nautoUpdateToggle.IsToggled: {autoUpdateToggle.IsToggled}" +
            $"\nquadsToggle.IsToggled: {quadsToggle.IsToggled}" +
            $"\nmeshesToggle.IsToggled: {meshesToggle.IsToggled}" +
            $"\nmaskToggle.IsToggled: {maskToggle.IsToggled}" +
            $"\ninferRegionsToggle.IsToggled: {inferRegionsToggle.IsToggled}" +
            $"\nplatformToggle.IsToggled: {platformToggle.IsToggled}" +
            $"\nwallToggle.IsToggled: {wallToggle.IsToggled}" +
            $"\nfloorToggle.IsToggled: {floorToggle.IsToggled}" +
            $"\nceilingToggle.IsToggled: {ceilingToggle.IsToggled}" +
            $"\nworldToggle.IsToggled: {worldToggle.IsToggled}" +
            $"\ncompletelyInferred.IsToggled: {completelyInferred.IsToggled}" +
            $"\nbackgroundToggle.IsToggled: {backgroundToggle.IsToggled}" +
            "\n";

            print(s);
        }

        /// <summary>
        /// Gets the color of the given surface type
        /// </summary>
        /// <param name="surfaceType">The surface type to get color for</param>
        /// <returns>The color of the type</returns>
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

        private void ClearAndUpdateObserver() { ClearAndUpdateObserver("none"); }
        private void ClearAndUpdateObserver(string source)
        {
            logInputStates(source);
            ClearScene();
            observer.UpdateOnDemand();
        }

        private void ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            if (observer.SurfaceTypes.IsMaskSet(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
                print("Disabling? observer.SurfaceTypes &= ~surfaceType");
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
                print("Enabling? observer.SurfaceTypes |= surfaceType");
            }
        }

        public void print(string msg) {
            // my custom logging code
        }

        #endregion Helper Functions
    }
}
