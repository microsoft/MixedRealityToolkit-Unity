using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSpatialAwarenessController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public GameObject SceneObjectPrefab;

        public Transform ParentGameObject;

        public GameObject StuffToPlace;

        public string SavedSceneNamePrefix = "DemoSceneUnderstanding";

        public bool InstanciatePrefabs;

        public GameObject autoUpdateToggle;
        public GameObject quadsToggle;
        public GameObject meshesToggle;
        public GameObject maskToggle;
        public GameObject platformToggle;
        public GameObject wallToggle;
        public GameObject floorToggle;
        public GameObject ceilingToggle;

        List<SpatialAwarenessSceneObject> platforms = new List<SpatialAwarenessSceneObject>(16);

        IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

        bool isInit = false;

        bool tryFindDebug = true;

        void OnEnable()
        {
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

            // setting IsToggled not working in Enable() on mrkt 2.0... do in Update()
            // XXX ugly hack
            // InitToggleButtonState();

            platforms.Clear();
        }

        void OnDisable()
        {
            CoreServices.SpatialAwarenessSystem?.UnregisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
        }

        async void Update()
        {
            // Hack around toggle not working in Enable()
            if (!isInit)
            {
                InitToggleButtonState();
                isInit = true;
            }

            if (tryFindDebug)
            {
                tryFindDebug = false;
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
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            var sceneObject = eventData.SpatialObject;

            if (sceneObject.SurfaceType == SpatialAwarenessSurfaceTypes.Platform)
            {
                platforms.Add(sceneObject);
            }

            if (InstanciatePrefabs)
            {
                var prefab = Instantiate(SceneObjectPrefab);
                prefab.transform.SetPositionAndRotation(sceneObject.Position, sceneObject.Rotation);

                if (ParentGameObject)
                {
                    prefab.transform.SetParent(ParentGameObject);
                }

                // A prefab can implement the ISpatialAwarenessSceneObjectConsumer contract
                // this will let the prefab author decide how it wants to "react" to the sceneObject
                // In the demo scene the prefab logic will scale the prefab to fit quad extents

                foreach (var x in prefab.GetComponents<ISpatialAwarenessSceneObjectConsumer>())
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

            for (int i = 0; i < platformCount; ++i)
            {
                var distance = Vector3.Distance(Camera.main.transform.position, platforms[i].Position);

                if (distance < closestDistance)
                {
                    closestDistance = Math.Min(distance, closestDistance);
                    closestGuid = platforms[i].Quads[0].guid;
                }
            }

            Debug.Log($"closestGuid = {closestGuid}, closestDistance = {closestDistance}");

            // Place our stuff

            Vector3 placement;
            Vector2 queryArea = Vector2.one * .01f;
            Quaternion rotation;

            if (observer.TryFindCentermostPlacement(closestGuid, queryArea, out placement, out rotation))
            {
                var stuff = Instantiate(StuffToPlace);
                stuff.transform.position = placement;
                stuff.transform.rotation = rotation;
                Debug.Log($"Found transform @ {placement.ToString("F4")}");
            }
        }

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

        // we do we inherit this baggage?

        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
