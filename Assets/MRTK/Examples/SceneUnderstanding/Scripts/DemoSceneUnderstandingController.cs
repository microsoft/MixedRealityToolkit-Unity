// todo
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSceneUnderstandingController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        #region Private Fields

        #region Serialized Fields

        [SerializeField]
        private string SavedSceneNamePrefix = "DemoSceneUnderstanding";
        [SerializeField]
        private GameObject StuffToPlace = null;
        [SerializeField]
        private SpatialAwarenessSurfaceTypes surfaceTypeToPlaceOn = SpatialAwarenessSurfaceTypes.Platform;
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
        
        private readonly List<SpatialAwarenessSceneObject> observedSceneObjects = new List<SpatialAwarenessSceneObject>(16);
        private IMixedRealitySceneUnderstandingObserver observer;
        private bool isInit = false;

        // by default stick something on the nearest platform without the user requesting it
        private bool autoPlaceStuffOnce = true;

        #endregion Private Fields

        #region MonoBehaviour Functions

        async void OnEnable()
        {
            await RegisterHandlersAsync();
            observedSceneObjects.Clear();
        }

        void OnDisable()
        {
            CoreServices.SpatialAwarenessSystem?.UnregisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
        }

        async void Update()
        {
            if (!isInit && (observer != null))
            {
                // InitToggleButtonState();
                isInit = true;
            }

            if (autoPlaceStuffOnce)
            {
                autoPlaceStuffOnce = false;
                await Task.Delay(TimeSpan.FromSeconds(4));
                PutStuffOnNearest();
            }
        }

        #endregion MonoBehaviour Functions

        #region IMixedRealitySpatialAwarenessObservationHandler Implementations

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This method called everytime a SceneObject created by the SU observer
            // The eventData contains everything you need do something useful

            if (eventData.SpatialObject.SurfaceType == surfaceTypeToPlaceOn)
            {
                observedSceneObjects.Add(eventData.SpatialObject);
            }

            if (InstantiatePrefabs)
            {
                var prefab = Instantiate(InstantiatedPrefab);
                prefab.transform.SetPositionAndRotation(eventData.SpatialObject.Position, eventData.SpatialObject.Rotation);

                if (InstantiatedParent)
                {
                    prefab.transform.SetParent(InstantiatedParent);
                }

                // A prefab can implement the ISpatialAwarenessSceneObjectConsumer contract
                // this will let the prefab author decide how it wants to "react" to the new sceneObject
                // In the demo scene, the prefab will scale itself to fit quad extents

                foreach (var x in prefab.GetComponents<ISceneUnderstandingSceneObjectConsumer>())
                {
                    x.OnSpatialAwarenessSceneObjectCreated(eventData.SpatialObject);
                }
            }
            else
            {
                foreach (var quad in eventData.SpatialObject.Quads)
                {
                    quad.GameObject.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);
                }

            }
        }

        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {

        }

        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This is never called by the SU observer by design
            throw new System.NotImplementedException();
        }

        #endregion IMixedRealitySpatialAwarenessObservationHandler Implementations


        #region UI Functions

        public void UpdateScene()
        {
            observer.UpdateOnDemand();
        }

        public void SaveSave()
        {
            observer.SaveScene(SavedSceneNamePrefix);
        }

        public void ClearScene()
        {
            observer.ClearObservations();
        }

        public void ToggleAutoUpdate()
        {
            observer.AutoUpdate = !observer.AutoUpdate;
        }

        public void ToggleOcclusionMask()
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
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleGeneratePlanes()
        {
            observer.RequestPlaneData = !observer.RequestPlaneData;
            if (observer.RequestPlaneData)
            {
                observer.RequestMeshData = false;
                meshesToggle.IsToggled = false;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleGenerateMeshes()
        {
            observer.RequestMeshData = !observer.RequestMeshData;
            if (observer.RequestMeshData)
            {
                observer.RequestPlaneData = false;
                quadsToggle.IsToggled = false;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleFloors()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Floor;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleWalls()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Wall;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleCeilings()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Ceiling;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void TogglePlatforms()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Platform;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleInferRegions()
        {
            observer.InferRegions = !observer.InferRegions;
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleWorld()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.World;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }

            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                // Ensure we requesting meshes
                observer.RequestMeshData = true;
                meshesToggle.GetComponent<Interactable>().IsToggled = true;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleBackground()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Background;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleCompletelyInferred()
        {
            var surfaceType = SpatialAwarenessSurfaceTypes.Inferred;
            if (observer.SurfaceTypes.HasFlag(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        #endregion UI Functions

        #region Helper Functions

        async Task RegisterHandlersAsync()
        {
            await new WaitUntil(() => CoreServices.SpatialAwarenessSystem != null);

            observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySceneUnderstandingObserver>();

            if (observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer!");
                return;
            }

            CoreServices.SpatialAwarenessSystem.RegisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
            InitToggleButtonState();
        }

        private void InitToggleButtonState()
        {
            // Configure observer
            autoUpdateToggle.IsToggled = observer.AutoUpdate;
            quadsToggle.IsToggled = observer.RequestPlaneData;
            meshesToggle.IsToggled = observer.RequestMeshData;
            maskToggle.IsToggled = observer.RequestOcclusionMask;
            inferRegionsToggle.IsToggled = observer.InferRegions;

            // Filter display
            platformToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Platform);
            wallToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Wall);
            floorToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Floor);
            ceilingToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Ceiling);
            worldToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World);
            completelyInferred.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Inferred);
            backgroundToggle.IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Background);
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

        public async void PutStuffOnNearest()
        {
            var platformCount = observedSceneObjects.Count;

            if (platformCount < 1)
            {
                await Task.Yield();
                return;
            }

            // Find the Guid of our nearest neighbor

            float closestDistance = float.MaxValue;
            int closestGuid = 0;
            bool foundQuadGuid = false;
            SpatialAwarenessSceneObject closestObject = null;

            var cameraPosition = CameraCache.Main.transform.position;

            for (int i = 0; i < platformCount; ++i)
            {
                var distance = Vector3.Distance(cameraPosition, observedSceneObjects[i].Position);

                if (distance < closestDistance)
                {
                    closestObject = observedSceneObjects[i];

                    closestDistance = Math.Min(distance, closestDistance);

                    if (observedSceneObjects[i].Quads.Count == 0)
                    {
                        continue;
                    }

                    closestGuid = observedSceneObjects[i].Quads[0].Id;
                    foundQuadGuid = true;
                }
            }

            var stuff = Instantiate(StuffToPlace);

            // Place our stuff

            if (closestObject == null)
            {
                return;
            }

            if (foundQuadGuid)
            {
                var bounds = new Bounds(Vector3.zero, Vector3.zero);

                foreach (Renderer r in StuffToPlace.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(r.bounds);
                }

                var queryArea = new Vector2(bounds.size.x, bounds.size.y);

                Vector3 placement;

                if (observer.TryFindCentermostPlacement(closestGuid, queryArea, out placement))
                {
                    stuff.transform.position = placement;
                    stuff.transform.rotation = closestObject.Rotation;
                }
            }
            else
            {
                stuff.transform.position = closestObject.Position;
                stuff.transform.rotation = closestObject.Rotation;
            }

            var tmp = stuff.GetComponentInChildren<TextMeshPro>();

            if (tmp)
            {
                tmp.text = $"Distance = {closestDistance.ToString("F2")}";
            }

            var demoConstraint = stuff.GetComponent<ParentConstraint>();

            if (demoConstraint)
            {
                demoConstraint.rotationAtRest = stuff.transform.rotation.eulerAngles;
                demoConstraint.translationAtRest = stuff.transform.position;
                demoConstraint.constraintActive = true;
            }
        }

        #endregion Helper Functions
    }
}
