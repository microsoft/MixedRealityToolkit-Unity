using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSpatialAwarenessController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public GameObject SceneObjectPrefab;

        public Transform ParentGameObject;

        public TextAsset SerializedScene;

        public string SerializedSceneName = "TestSaveSerializedScene.bytes";

        IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

        private bool generatePlanes;
        private bool generateMeshes;
        private bool generateEnvironment;

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

        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            Debug.Log($"Removed {eventData.GuidId}");
        }

        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            Debug.Log($"Updated {eventData.GuidId}");
        }

        public void UpdateScene()
        {
            Debug.Log("UpdateScene");
        }

        public void ToggleAutoUpdate()
        {
            Debug.Log("ToggleAutoUpdate");
        }

        public void ToggleOcclusionMask()
        {
            Debug.Log("ToggleOcclusionMask");
        }

        public void LoadScene()
        {
            if (SerializedScene.bytes != null)
            {
                observer.LoadScene(SerializedScene.bytes);
            }
        }

        public void SaveSave()
        {
            observer.SaveScene(SerializedSceneName);
        }

        public void ToggleGeneratePlanes()
        {
            observer.RequestPlaneData = generatePlanes;
            generatePlanes = !generatePlanes;
            observer.Update();
        }

        public void ToggleGenerateMeshes()
        {
            observer.RequestMeshData = generateMeshes;
            generateMeshes = !generateMeshes;
            observer.Update();
        }
    }
}
