// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSceneUnderstandingController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public string SavedSceneNamePrefix = "DemoSceneUnderstanding";
        public GameObject StuffToPlace;
        public SpatialAwarenessSurfaceTypes surfaceTypeToPlaceOn = SpatialAwarenessSurfaceTypes.Platform;
        public bool InstantiatePrefabs;
        public GameObject InstantiatedPrefab;
        public Transform InstantiatedParent;
        [Header("UI")]
        public GameObject autoUpdateToggle;
        public GameObject quadsToggle;
        public GameObject meshesToggle;
        public GameObject maskToggle;
        public GameObject platformToggle;
        public GameObject wallToggle;
        public GameObject floorToggle;
        public GameObject ceilingToggle;
        public GameObject worldToggle;
        public GameObject completelyToggle;
        public GameObject backgroundToggle;

        List<SpatialAwarenessSceneObject> observedSceneObjects = new List<SpatialAwarenessSceneObject>(16);

        IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

        bool isInit = false;

        // by default stick something on the nearest platform without the user requesting it
        bool autoPlaceStuffOnce = true;

        async Task RegisterHandlersAsync()
        {
            await new WaitUntil(() => CoreServices.SpatialAwarenessSystem != null);

            CoreServices.SpatialAwarenessSystem.RegisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);

            var access = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

            if (access == null)
            {
                Debug.LogError("Couldn't get access to CoreServices.SpatialAwarenessSystem");
                return;
            }

            observer = access.GetDataProvider<IMixedRealitySpatialAwarenessSceneUnderstandingObserver>();

            if (observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer!");
                return;
            }
        }

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
                InitToggleButtonState();
                isInit = true;
            }

            if (autoPlaceStuffOnce)
            {
                autoPlaceStuffOnce = false;
                await Task.Delay(TimeSpan.FromSeconds(4));
                PutStuffOnNearest();
            }
        }

        private void InitToggleButtonState()
        {
            autoUpdateToggle.GetComponent<Interactable>().IsToggled = observer.AutoUpdate;
            quadsToggle.GetComponent<Interactable>().IsToggled = observer.RequestPlaneData;
            meshesToggle.GetComponent<Interactable>().IsToggled = observer.RequestMeshData;
            maskToggle.GetComponent<Interactable>().IsToggled = observer.RequestOcclusionMask;
            platformToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Platform);
            wallToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Wall);
            floorToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Floor);
            ceilingToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Ceiling);
            worldToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World);
            completelyToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.CompletelyInferred);
            backgroundToggle.GetComponent<Interactable>().IsToggled = observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Background);
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This method called everytime a SceneObject created by the SU observer
            // The eventData contains everything you need do something useful

            var sceneObject = eventData.SpatialObject; // alias

            if (sceneObject.SurfaceType == surfaceTypeToPlaceOn)
            {
                observedSceneObjects.Add(sceneObject);
            }

            if (InstantiatePrefabs)
            {
                var prefab = Instantiate(InstantiatedPrefab);
                prefab.transform.SetPositionAndRotation(sceneObject.Position, sceneObject.Rotation);

                if (InstantiatedParent)
                {
                    prefab.transform.SetParent(InstantiatedParent);
                }

                // A prefab can implement the ISpatialAwarenessSceneObjectConsumer contract
                // this will let the prefab author decide how it wants to "react" to the new sceneObject
                // In the demo scene, the prefab will scale itself to fit quad extents

                foreach (var x in prefab.GetComponents<ISceneUnderstandingSceneObjectConsumer>())
                {
                    x.OnSpatialAwarenessSceneObjectCreated(sceneObject);
                }
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
            Guid closestGuid;
            bool foundQuadGuid = false;
            SpatialAwarenessSceneObject closestObject = null;

            for (int i = 0; i < platformCount; ++i)
            {
                var distance = Vector3.Distance(Camera.main.transform.position, observedSceneObjects[i].Position);

                if (distance < closestDistance)
                {
                    closestObject = observedSceneObjects[i];

                    closestDistance = Math.Min(distance, closestDistance);

                    if (observedSceneObjects[i].Quads.Count == 0)
                    {
                        Debug.LogWarning("Can't ask for quads if observer wasn't configured to fetch them!");
                        continue;
                    }

                    closestGuid = observedSceneObjects[i].Quads[0].guid;
                    foundQuadGuid = true;
                }
            }

            Debug.Log($"closestGuid = {closestGuid}, closestDistance = {closestDistance}");

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

                if (observer.TryFindCentermostPlacement((Guid)closestGuid, queryArea, out placement))
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
            tmp.text = $"Distance = {closestDistance.ToString("F2")}";

            var demoConstraint = stuff.GetComponent<ParentConstraint>();
            if (demoConstraint)
            {
                demoConstraint.rotationAtRest = stuff.transform.rotation.eulerAngles;
                demoConstraint.translationAtRest = stuff.transform.position;
                demoConstraint.constraintActive = true;
            }
        }

        #region UI

        public void UpdateScene()
        {
            Debug.Log("UpdateScene");
            observer.UpdateOnDemand();
        }

        public void SaveSave()
        {
            Debug.Log("SaveSave");
            observer.SaveScene(SavedSceneNamePrefix);
        }

        public void ClearScene()
        {
            observer.ClearObservations();
        }

        public void ToggleAutoUpdate()
        {
            Debug.Log("ToggleAutoUpdate");
            observer.AutoUpdate = !observer.AutoUpdate;
        }

        public void ToggleOcclusionMask()
        {
            Debug.Log("ToggleOcclusionMask");
            var observerMask = observer.RequestOcclusionMask;
            observer.RequestOcclusionMask = !observerMask;
            if (observer.RequestOcclusionMask)
            {
                observer.RequestPlaneData = true;
                quadsToggle.GetComponent<Interactable>().IsToggled = true;
                observer.RequestMeshData = false;
                meshesToggle.GetComponent<Interactable>().IsToggled = false;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleGeneratePlanes()
        {
            Debug.Log("ToggleGeneratePlanes");
            observer.RequestPlaneData = !observer.RequestPlaneData;
            if (observer.RequestPlaneData)
            {
                observer.RequestMeshData = false;
                meshesToggle.GetComponent<Interactable>().IsToggled = false;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleGenerateMeshes()
        {
            Debug.Log("ToggleGenerateMeshes");
            observer.RequestMeshData = !observer.RequestMeshData;
            if (observer.RequestMeshData)
            {
                observer.RequestPlaneData = false;
                quadsToggle.GetComponent<Interactable>().IsToggled = false;
                observer.RequestOcclusionMask = false;
                maskToggle.GetComponent<Interactable>().IsToggled = false;
            }
            observer.ClearObservations();
            observer.UpdateOnDemand();
        }

        public void ToggleFloors()
        {
            Debug.Log("ToggleFloors");
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
            Debug.Log("ToggleWalls");
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
            Debug.Log("ToggleCeilings");
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
            Debug.Log("TogglePlatforms");
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
            var surfaceType = SpatialAwarenessSurfaceTypes.CompletelyInferred;
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

        #endregion UI

        // Legacy baggage

        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This is never called by the SU observer by design
            throw new System.NotImplementedException();
        }

        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This is never called by the SU observer by design
            throw new System.NotImplementedException();
        }
    }
}
