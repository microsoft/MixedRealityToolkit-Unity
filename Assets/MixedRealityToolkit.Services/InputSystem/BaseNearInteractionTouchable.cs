// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        [Tooltip("Distance behind the surface at which you will receive a touch started event")]
        [SerializeField]
        [FormerlySerializedAs("distBack")]
        protected float pokeThreshold = 0.25f;
        /// <summary>
        /// Distance behind the surface at which the touchable becomes active.
        /// </summary>
        /// <remarks>
        /// When the pointer distance to the touchable object becomes less than -PokeThreshold (i.e. behind the surface),
        /// then the Touch Started event is raised and the touchable object becomes tracked by the pointer.
        /// </remarks>
        public float PokeThreshold => pokeThreshold;

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
        public float DebounceThreshold => debounceThreshold;

        protected void OnValidate()
        {
            pokeThreshold = Math.Max(pokeThreshold, 0);
            debounceThreshold = Math.Max(debounceThreshold, 0);
        }

        public abstract float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal);
    }

    /// <summary>
    /// Base class for all touchables using colliders.
    /// </summary>
    [RequireComponent(typeof(Collider))]
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

        protected new void OnValidate()
        {
            base.OnValidate();

            touchableCollider = GetComponent<Collider>();
        }
    }
}
