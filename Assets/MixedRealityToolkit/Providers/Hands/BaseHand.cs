// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public abstract class BaseHand : BaseController, IMixedRealityHand
    {
        // Hand ray
        protected HandRay HandRay { get; } = new HandRay();
        public override bool IsInPointingPose
        {
            get
            {
                return HandRay.ShouldShowRay;
            }
        }

        // Velocity internal states
        private float deltaTimeStart;
        private Vector3 lastPosition;
        private Vector3 lastPalmNormal;
        private readonly int velocityUpdateInterval = 9;
        private int frameOn = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public BaseHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        #region Protected InputSource Helpers

        #region Gesture Definitions

        protected void UpdateVelocity()
        {
            if (frameOn == 0)
            {
                deltaTimeStart = Time.unscaledTime;
                lastPosition = GetJointPosition(TrackedHandJoint.Palm);
                lastPalmNormal = GetPalmNormal();
                //lastPalmNormal = new Vector3(1, 0, 0);
            }
            else if (frameOn == velocityUpdateInterval)
            {
                //update linear velocity
                float deltaTime = Time.unscaledTime - deltaTimeStart;
                Vector3 newVelocity = (GetJointPosition(TrackedHandJoint.Palm) - lastPosition) / deltaTime;
                Velocity = (Velocity * 0.8f) + (newVelocity * 0.2f);

                //update angular velocity
                Vector3 currentPalmNormal = GetPalmNormal();
                //currentPalmNormal = new Vector3(0, 1, 0);
                Quaternion rotation = Quaternion.FromToRotation(lastPalmNormal, currentPalmNormal);
                Vector3 rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                AngularVelocity = rotationRate / deltaTime;
            }

            frameOn++;
            frameOn = frameOn > velocityUpdateInterval ? 0 : frameOn;
        }

        #endregion Gesture Definitions

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