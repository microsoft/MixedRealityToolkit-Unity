using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class RaycastProvider : BaseDataProvider, IMixedRealityRaycastProvider
    {
        public RaycastProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile profile) : base(registrar, inputSystem, null, DefaultPriority, profile)
        { }

        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="physicsHit"></param>
        /// <returns>Whether or not the raycast hit something.</returns>
        public bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSimplePhysicsStep(step, step.Length, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }

        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> within a specified maximum distance.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="maxDistance"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="physicsHit"></param>
        /// <returns>Whether or not the raycast hit something.</returns>
        public bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSimplePhysicsStep(step, maxDistance, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }

        /// <summary>
        /// Sphere raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="radius"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="physicsHit"></param>
        /// <returns>Whether or not the raycast hit something.</returns>
        public bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSpherePhysicsStep(step, radius, step.Length, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }

        public bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSpherePhysicsStep(step, radius, maxDistance, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }
    }
}
