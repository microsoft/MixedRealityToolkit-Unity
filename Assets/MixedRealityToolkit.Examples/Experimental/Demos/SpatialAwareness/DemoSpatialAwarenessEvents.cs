using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;

public class DemoSceneUnderstandingEvents : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
{
    public IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

    Animator animator;
    public bool triggered = false;

    void OnEnable()
    {
        CoreServices.SpatialAwarenessSystem.RegisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);

        IMixedRealitySpatialAwarenessSystem spatialSystem = null;

        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialSystem))
        {
            // Failed to acquire the system. It may not have been registered
        }

        var access = spatialSystem as IMixedRealityDataProviderAccess;

        observer = access.GetDataProvider<IMixedRealitySpatialAwarenessSceneUnderstandingObserver>();

        Debug.Log($"observer.UpdateInterval = {observer.UpdateInterval}");

        animator = GetComponent<Animator>();
        animator.SetBool("Persistent", observer.UsePersistentObjects);
    }

    void OnDisable()
    {
        CoreServices.SpatialAwarenessSystem.UnregisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
    }

    public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
    {
        Debug.Log($"Added {eventData.SpatialObject.Guid} {eventData.SpatialObject.SurfaceType} with {eventData.SpatialObject.Meshes.Count} meshes.");

        //animator.SetBool("Persistent", true);
    }

    public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
    {
        Debug.Log($"Removed {eventData.GuidId}");
    }

    public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
    {
        Debug.Log($"Updated {eventData.GuidId}");
    }

    public void HandleIt()
    {
        Debug.Log("Got it");
    }
}
