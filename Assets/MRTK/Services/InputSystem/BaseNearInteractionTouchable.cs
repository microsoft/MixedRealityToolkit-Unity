// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for all NearInteractionTouchables.
    /// </summary>
    /// <remarks>
    /// Add this component to objects to raise touch events when in [PokePointer](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) proximity.
    /// The object layer must be included of the [PokeLayerMasks](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer.PokeLayerMasks).
    /// </remarks>
    public abstract class BaseNearInteractionTouchable : MonoBehaviour
    {
        [SerializeField]
        protected TouchableEventType eventsToReceive = TouchableEventType.Touch;

        /// <summary>
        /// The type of event to receive.
        /// </summary>
        public TouchableEventType EventsToReceive { get => eventsToReceive; set => eventsToReceive = value; }

        [Tooltip("Distance in front of the surface at which you will receive a touch completed event")]
        [SerializeField]
        protected float debounceThreshold = 0.01f;
        /// <summary>
        /// Distance in front of the surface at which you will receive a touch completed event.
        /// </summary>
        /// <remarks>
        /// When the touchable is active and the pointer distance becomes greater than +DebounceThreshold (i.e. in front of the surface),
        /// then the Touch Completed event is raised and the touchable object is released by the pointer.
        /// </remarks>
        public float DebounceThreshold { get => debounceThreshold; set => debounceThreshold = value; }

        protected virtual void OnValidate()
        {
            debounceThreshold = Math.Max(debounceThreshold, 0);
        }

        public abstract float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal);
    }

    /// <summary>
    /// Obsolete base class for all touchables using colliders.
    /// Use <see cref="BaseNearInteractionTouchable"/> instead.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [System.Obsolete("Use BaseNearIntearctionTouchable instead of ColliderNearInteractionTouchable", true)]
    public abstract class ColliderNearInteractionTouchable : BaseNearInteractionTouchable
    {
        public bool ColliderEnabled { get { return touchableCollider.enabled && touchableCollider.gameObject.activeInHierarchy; } }

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
    }

}
