// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionTouchableVolume to your scene and configure a touchable volume
    /// in order to get PointerDown and PointerUp events whenever a PokePointer collides with this volume.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/NearInteractionTouchableVolume")]
    public class NearInteractionTouchableVolume : BaseNearInteractionTouchable
    {
        /// <summary>
        /// Is the touchable collider enabled and active in the scene.
        /// </summary>
        public bool ColliderEnabled => touchableCollider.enabled && touchableCollider.gameObject.activeInHierarchy;

        /// <summary>
        /// The collider used by this touchable.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("collider")]
        private Collider touchableCollider;
        public Collider TouchableCollider => touchableCollider;

        protected override void OnValidate()
        {
            base.OnValidate();

            touchableCollider = GetComponent<Collider>();
        }

        /// <inheritdoc />
        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            Vector3 closest = TouchableCollider.ClosestPoint(samplePoint);

            normal = (samplePoint - closest);
            if (normal == Vector3.zero)
            {
                // inside object, use vector to centre as normal
                normal = samplePoint - TouchableCollider.bounds.center;
                normal.Normalize();
                // Return value less than zero so that when poke pointer is inside
                // object, it will not raise a touch up event.
                return -1;
            }
            else
            {
                float dist = normal.magnitude;
                normal.Normalize();
                return dist;
            }
        }
    }
}