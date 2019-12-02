// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The default implementation of IMixedRealityRaycastProvider.
    /// </summary>
    public class DefaultRaycastProvider : BaseCoreSystem, IMixedRealityRaycastProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public DefaultRaycastProvider(
            IMixedRealityServiceRegistrar registrar,
            MixedRealityInputSystemProfile profile) : this(profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        public DefaultRaycastProvider(
            MixedRealityInputSystemProfile profile) : base(profile)
        { }

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Default Raycast Provider";

        /// <inheritdoc />
        public bool Raycast(RayStep step, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo)
        {
            bool result = MixedRealityRaycaster.RaycastSimplePhysicsStep(step, step.Length, prioritizedLayerMasks, focusIndividualCompoundCollider, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(result, physicsHit);
            return result;
        }

        /// <inheritdoc />
        public bool SphereCast(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSpherePhysicsStep(step, radius, step.Length, prioritizedLayerMasks, focusIndividualCompoundCollider, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(result, physicsHit);
            return result;
        }

        /// <inheritdoc />
        public RaycastResult GraphicsRaycast(EventSystem eventSystem, PointerEventData pointerEventData, LayerMask[] layerMasks)
        {
            return eventSystem.Raycast(pointerEventData, layerMasks);
        }
    }
}
