﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591


using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Helper script to re-spawn objects if they go too far from their original position. 
    /// </summary>
    /// <remarks>
    /// The helper is useful for objects that will fall forever.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Tethered Placement")]
    internal class TetheredPlacement : MonoBehaviour
    {
        [SerializeField, Tooltip("The distance from the GameObject's spawn position at which will trigger a respawn.")]
        private float distanceThreshold = 20.0f;

        private Vector3 localRespawnPosition;
        private Quaternion localRespawnRotation;
        private Rigidbody rigidBody;
        private float distanceThresholdSquared;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            localRespawnPosition = transform.localPosition;
            localRespawnRotation = transform.localRotation;
            distanceThresholdSquared = distanceThreshold * distanceThreshold;
        }

        /// <summary>
        /// A Unity event function that is called every frame after normal update functions, if this object is enabled.
        /// </summary>
        private void LateUpdate()
        {
            float distanceSqr = (localRespawnPosition - transform.localPosition).sqrMagnitude;

            if (distanceSqr > distanceThresholdSquared)
            {
                // Reset any velocity from falling or moving when re-spawning to original location
                if (rigidBody != null)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }

                transform.localPosition = localRespawnPosition;
                transform.localRotation = localRespawnRotation;
            }
        }
    }
}
#pragma warning restore CS1591