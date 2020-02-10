using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Boo.Lang;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSpatialAwarenessController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public GameObject SceneObjectPrefab;

        public Transform ParentGameObject;

        public TextAsset SerializedScene;

        public string SerializedSceneName = "TestSaveSerializedScene.bytes";

        public bool ListenToObserverEvents;

        public GameObject autoUpdateToggle;
        public GameObject quadsToggle;
        public GameObject meshesToggle;
        public GameObject maskToggle;

        IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

        private bool generatePlanes;
        private bool generateMeshes;

        bool isInit = false;

        void Update()
        {
            // Hack around toggle not working in Enable()
            if (!isInit)
            {
                autoUpdateToggle.GetComponent<Interactable>().IsToggled = observer.StartupBehavior == Toolkit.Utilities.AutoStartBehavior.AutoStart;
                quadsToggle.GetComponent<Interactable>().IsToggled = observer.RequestPlaneData;
                meshesToggle.GetComponent<Interactable>().IsToggled = observer.RequestMeshData;
                maskToggle.GetComponent<Interactable>().IsToggled = observer.RequestOcclusionMask;
                isInit = true;
            }
        }

        void OnEnable()
        {
            if (ListenToObserverEvents)
            {
                CoreServices.SpatialAwarenessSystem.RegisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
            }

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

            // toggle IsToggled not working in Enable on mrkt 2.0... do in Update()
            //autoUpdateToggle.GetComponent<Interactable>().IsToggled = observer.StartupBehavior == Toolkit.Utilities.AutoStartBehavior.AutoStart;
            //quadsToggle.GetComponent<Interactable>().IsToggled = observer.RequestPlaneData;
            //meshesToggle.GetComponent<Interactable>().IsToggled = observer.RequestMeshData;
            //maskToggle.GetComponent<Interactable>().IsToggled = observer.RequestOcclusionMask;
        }

        void OnDisable()
        {
            CoreServices.SpatialAwarenessSystem?.UnregisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            var sceneObject = eventData.SpatialObject;
            var go = Instantiate(SceneObjectPrefab);
            go.transform.SetPositionAndRotation(sceneObject.Position, sceneObject.Rotation);
            if (ParentGameObject)
            {
                go.transform.SetParent(ParentGameObject);
            }
            foreach (var x in go.GetComponents<ISpatialAwarenessSceneObjectConsumer>())
            {
                x.OnSpatialAwarenessSceneObjectCreated(sceneObject);
            }
        }

        public void UpdateScene()
        {
            Debug.Log("UpdateScene");
            observer.UpdateOnDemand();
        }

        public void ToggleAutoUpdate()
        {
            Debug.Log("ToggleAutoUpdate");
            observer.AutoUpdate = true;
        }

        public void ToggleOcclusionMask()
        {
            Debug.Log("ToggleOcclusionMask");
            observer.RequestOcclusionMask = !observer.RequestOcclusionMask;
            observer.UpdateOnDemand();
        }

        public void SaveSave()
        {
            Debug.Log("SaveSave");
            observer.SaveScene(SerializedSceneName);
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
