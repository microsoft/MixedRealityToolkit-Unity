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

        private void Awake()
        {
            if (touchableCollider == null)
            {
                touchableCollider = GetComponent<Collider>();
            }
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

                // Try to calculate the proper penetration distance, to allow more accurate processing of touchable volumes.
                // Return value less than zero so that when poke pointer is inside object, it will not raise a touch up event.
                float rayScale = 1.1f;
                Vector3 outsidePoint = TouchableCollider.bounds.center + normal * (TouchableCollider.bounds.extents.magnitude * rayScale);
                if (TouchableCollider.Raycast(new Ray(outsidePoint, -normal), out RaycastHit raycastHit, TouchableCollider.bounds.size.magnitude * rayScale))
                {
                    return -Vector3.Distance(raycastHit.point, samplePoint);
                }
                else
                {
                    // Somehow we didn't hit the object, although we're touching it.
                    // Fallback to the max possible value, so other volumes may get favored over this.
                    return -TouchableCollider.bounds.extents.magnitude;
                }
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