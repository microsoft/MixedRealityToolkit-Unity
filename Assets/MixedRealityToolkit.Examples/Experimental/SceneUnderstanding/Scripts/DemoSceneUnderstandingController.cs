using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSceneUnderstandingController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public string SavedSceneNamePrefix = "DemoSceneUnderstanding";
        public GameObject StuffToPlace;
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
        public GameObject usePersistentToggle;
        public GameObject completelyToggle;
        public GameObject backgroundToggle;

        List<SpatialAwarenessSceneObject> platforms = new List<SpatialAwarenessSceneObject>(16);

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
            platforms.Clear();
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
                PutStuffOnNearestPlatform();
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
            usePersistentToggle.GetComponent<Interactable>().IsToggled = observer.UsePersistentObjects;
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This method called everytime a SceneObject created by the SU observer
            // The eventData contains everything you need do something useful

            var sceneObject = eventData.SpatialObject; // alias

            if (sceneObject.SurfaceType == SpatialAwarenessSurfaceTypes.Platform)
            {
                platforms.Add(sceneObject);
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

        public async void PutStuffOnNearestPlatform()
        {
            var platformCount = platforms.Count;

            if (platformCount < 1)
            {
                await Task.Yield();
                return;
            }

            // Find the Guid of our nearest neighbor

            float closestDistance = float.MaxValue;
            Guid closestGuid;
            bool foundGuid = false;
            SpatialAwarenessSceneObject closestPlatform = null;

            for (int i = 0; i < platformCount; ++i)
            {
                var distance = Vector3.Distance(Camera.main.transform.position, platforms[i].Position);

                if (distance < closestDistance)
                {
                    closestDistance = Math.Min(distance, closestDistance);
                    if (platforms[i].Quads.Count == 0)
                    {
                        Debug.LogWarning("Can't ask for quads if observer wasn't configured to fetch them!");
                        continue;
                    }
                    closestGuid = platforms[i].Quads[0].guid;
                    foundGuid = true;
                    closestPlatform = platforms[i];
                }
            }

            Debug.Log($"closestGuid = {closestGuid}, closestDistance = {closestDistance}");

            // Place our stuff

            if (!foundGuid)
            {
                Debug.LogWarning("Can't find placement for quad!");
                return;
            }

            var bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (Renderer r in StuffToPlace.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }

            var queryArea = new Vector2(bounds.size.x, bounds.size.y);

            Vector3 placement;

            if (observer.TryFindCentermostPlacement((Guid)closestGuid, queryArea, out placement))
            {
                var stuff = Instantiate(StuffToPlace);
                stuff.transform.position = placement;
                stuff.transform.rotation = closestPlatform.Rotation;
                var tmp = stuff.GetComponentInChildren<TextMeshPro>();
                tmp.text = $"Distance = {closestDistance.ToString("F2")}";
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
            }
            observer.UpdateOnDemand();
        }

        public void ToggleGeneratePlanes()
        {
            Debug.Log("ToggleGeneratePlanes");
            observer.RequestPlaneData = !observer.RequestPlaneData;
            observer.UpdateOnDemand();
        }

        public void ToggleGenerateMeshes()
        {
            Debug.Log("ToggleGenerateMeshes");
            observer.RequestMeshData = !observer.RequestMeshData;
            if (!observer.RequestMeshData)
            {
                observer.SurfaceTypes &= ~SpatialAwarenessSurfaceTypes.World;
                worldToggle.GetComponent<Interactable>().IsToggled = false;
            }
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
            observer.UpdateOnDemand();
        }

        public void TogglePersistentObjects()
        {
            observer.UsePersistentObjects = !observer.UsePersistentObjects;
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
