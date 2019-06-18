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
    /// Base class for all NearInteractionTouchables
    ///
    /// Technical details:
    /// Provides a listing of near field touch proximity bounds.
    /// This is used to detect if a contact point is near an object to turn on near field interactions
    /// </summary>
    public abstract class BaseNearInteractionTouchable : MonoBehaviour
    {
        [SerializeField]
        protected TouchableEventType eventsToReceive = TouchableEventType.Touch;

        /// <summary>
        /// The type of event to receive.
        /// </summary>
        public TouchableEventType EventsToReceive => eventsToReceive;

        [Tooltip("Distance behind the surface at which you will receive a touch started event")]
        [SerializeField]
        [FormerlySerializedAs("distBack")]
        protected float pokeThreshold = 0.25f;
        public float PokeThreshold => pokeThreshold;

        [Tooltip("Distance in front of the surface at which you will receive a touch completed event")]
        [SerializeField]
        protected float debounceThreshold = 0.01f;
        public float DebounceThreshold => debounceThreshold;

        protected void OnValidate()
        {
            pokeThreshold = Math.Max(pokeThreshold, 0);
            debounceThreshold = Math.Max(debounceThreshold, 0);
        }

        public abstract float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal);
    }

    /// <summary>
    /// Base class for all touchables using colliders
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