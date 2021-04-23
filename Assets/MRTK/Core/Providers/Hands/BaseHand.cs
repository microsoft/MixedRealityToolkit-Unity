// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public abstract class BaseHand : BaseController, IMixedRealityHand
    {
        // Hand ray
        protected virtual IHandRay HandRay { get; } = new HandRay();

        public override bool IsInPointingPose => HandRay.ShouldShowRay;

        // Velocity internal states
        private float deltaTimeStart;
        private const int velocityUpdateInterval = 6;
        private int frameOn = 0;

        private readonly Vector3[] velocityPositionsCache = new Vector3[velocityUpdateInterval];
        private readonly Vector3[] velocityNormalsCache = new Vector3[velocityUpdateInterval];
        private Vector3 velocityPositionsSum = Vector3.zero;
        private Vector3 velocityNormalsSum = Vector3.zero;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BaseHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null,
            IMixedRealityInputSourceDefinition definition = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, definition)
        { }

        #region Protected InputSource Helpers

        #region Gesture Definitions

        protected void UpdateVelocity()
        {
            if (frameOn < velocityUpdateInterval)
            {
                velocityPositionsCache[frameOn] = GetJointPosition(TrackedHandJoint.Palm);
                velocityPositionsSum += velocityPositionsCache[frameOn];
                velocityNormalsCache[frameOn] = GetPalmNormal();
                velocityNormalsSum += velocityNormalsCache[frameOn];
            }
            else
            {
                int frameIndex = frameOn % velocityUpdateInterval;

                float deltaTime = Time.unscaledTime - deltaTimeStart;

                Vector3 newPosition = GetJointPosition(TrackedHandJoint.Palm);
                Vector3 newNormal = GetPalmNormal();

                Vector3 newPositionsSum = velocityPositionsSum - velocityPositionsCache[frameIndex] + newPosition;
                Vector3 newNormalsSum = velocityNormalsSum - velocityNormalsCache[frameIndex] + newNormal;

                Velocity = (newPositionsSum - velocityPositionsSum) / deltaTime / velocityUpdateInterval;

                Quaternion rotation = Quaternion.FromToRotation(velocityNormalsSum / velocityUpdateInterval, newNormalsSum / velocityUpdateInterval);
                Vector3 rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                AngularVelocity = rotationRate / deltaTime;

                velocityPositionsCache[frameIndex] = newPosition;
                velocityNormalsCache[frameIndex] = newNormal;
                velocityPositionsSum = newPositionsSum;
                velocityNormalsSum = newNormalsSum;
            }

            deltaTimeStart = Time.unscaledTime;
            frameOn++;
        }

        #endregion Gesture Definitions

        /// <inheritdoc />
        public abstract bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose);

        private Vector3 GetJointPosition(TrackedHandJoint jointToGet)
        {
            if (TryGetJoint(jointToGet, out MixedRealityPose pose))
            {
                return pose.Position;
            }
            return Vector3.zero;
        }

        protected Vector3 GetPalmNormal()
        {
            if (TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose pose))
            {
                return -pose.Up;
            }
            return Vector3.zero;
        }

        private float DistanceSqrPointToLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            if (lineStart == lineEnd)
            {
                return (point - lineStart).magnitude;
            }

            float lineSegmentMagnitude = (lineEnd - lineStart).magnitude;
            Vector3 ray = (lineEnd - lineStart);
            ray *= (1.0f / lineSegmentMagnitude);
            float dot = Vector3.Dot(point - lineStart, ray);
            if (dot <= 0)
            {
                return (point - lineStart).sqrMagnitude;
            }
            if (dot >= lineSegmentMagnitude)
            {
                return (point - lineEnd).sqrMagnitude;
            }
            return ((lineStart + (ray * dot)) - point).sqrMagnitude;
        }

        #endregion Private InputSource Helpers
    }
}