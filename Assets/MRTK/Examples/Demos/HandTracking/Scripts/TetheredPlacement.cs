// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Helper script to respawn objects if they go too far from their original position. Useful for objects that will fall forever etc.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/TetheredPlacement")]
    public class TetheredPlacement : MonoBehaviour
    {
        [SerializeField, Tooltip("The distance from the GameObject's spawn position at which will trigger a respawn."), FormerlySerializedAs("DistanceThreshold")]
        private float distanceThreshold = 20.0f;

        /// <summary>
        /// The distance from the GameObject's spawn position at which will trigger a respawn.
        /// </summary>
        /// <remarks>Also updates a local cache of this value squared for performant distance checking.</remarks>
        public float DistanceThreshold
        {
            get => distanceThreshold;
            set
            {
                distanceThreshold = value;
                distanceThresholdSquared = distanceThreshold * distanceThreshold;
            }
        }

        private Vector3 localRespawnPosition;
        private Quaternion localRespawnRotation;
        private Rigidbody rigidBody;
        private float distanceThresholdSquared;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            LockSpawnPoint();
            distanceThresholdSquared = distanceThreshold * distanceThreshold;
        }

        private void LateUpdate()
        {
            float distanceSqr = (localRespawnPosition - transform.localPosition).sqrMagnitude;

            if (distanceSqr > distanceThresholdSquared)
            {
                // Reset any velocity from falling or moving when respawning to original location
                if (rigidBody != null)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }

                transform.localPosition = localRespawnPosition;
                transform.localRotation = localRespawnRotation;
            }
        }

        /// <summary>
        /// Updates the local respawn pose to the objects current pose.
        /// </summary>
        public void LockSpawnPoint()
        {
            localRespawnPosition = transform.localPosition;
            localRespawnRotation = transform.localRotation;
        }
    }
}
