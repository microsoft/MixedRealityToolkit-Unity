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
        public static IReadOnlyCollection<BaseNearInteractionTouchable> Instances { get { return instances.AsReadOnly(); } }
        private static readonly List<BaseNearInteractionTouchable> instances = new List<BaseNearInteractionTouchable>();

        [SerializeField]
        protected TouchableEventType eventsToReceive = TouchableEventType.Touch;

        /// <summary>
        /// The type of event to receive.
        /// </summary>
        public TouchableEventType EventsToReceive => eventsToReceive;

        public bool ColliderEnabled { get { return !usesCollider || touchableCollider.enabled && touchableCollider.gameObject.activeInHierarchy; } }

        /// <summary>
        /// False if no collider is found on validate.
        /// This is used to avoid the perf cost of a null check with the collider.
        /// </summary>
        protected bool usesCollider = false;

        /// <summary>
        /// The collider used by this touchable.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("collider")]
        protected Collider touchableCollider;

        protected float distFront = 0.2f;

        [Tooltip("Distance behind the surface at which you will receive a touch up event")]
        [SerializeField]
        protected float distBack = 0.25f;

        [Tooltip("Distance in front of the surface at which you will receive a touch up event")]
        [SerializeField]
        protected float debounceThreshold = 0.01f;

        public float DistBack => distBack;

        public float DistFront => distFront;

        public float DebounceThreshold => debounceThreshold;

        protected void OnEnable()
        {
            instances.Add(this);
        }

        protected void OnDisable()
        {
            instances.Remove(this);
        }

        protected void OnValidate()
        {
            distBack = Math.Max(distBack, 0);
            distFront = Math.Max(distFront, 0);
            debounceThreshold = Math.Max(debounceThreshold, 0);

            touchableCollider = GetComponent<Collider>();
            usesCollider = touchableCollider != null;
        }

        public abstract float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal);
    }
}