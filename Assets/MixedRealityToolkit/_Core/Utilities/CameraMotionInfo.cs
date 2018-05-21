// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    /// <summary>
    /// CameraMotionInfo calculates the velocity and direction of the camera.
    /// </summary>
    [DisallowMultipleComponent]
    public class CameraMotionInfo : MonoBehaviour
    {
        private const float VelocityThreshold = 0.1f;

        private const float MovementThreshold = 0.01f;

        public Vector3 HeadVelocity { get; private set; } = Vector3.zero;

        public Vector3 MoveDirection { get; private set; } = Vector3.one;

        [SerializeField]
        [Tooltip("Minimum velocity threshold")]
        private float headVelIdleThresh = 0.5f;

        [SerializeField]
        [Tooltip("Maximum velocity threshold")]
        private float headVelMoveThresh = 2f;

        [SerializeField]
        private bool debugDrawHeadVelocity = true;

        [SerializeField]
        private bool debugDrawHeadDirection = true;

        private Vector3 lastHeadPos = Vector3.zero;

        private Vector3 newHeadMoveDirection = Vector3.zero;

        private void FixedUpdate()
        {
            // Update headVelocity
            Vector3 newHeadPos = CameraCache.Main.transform.position;
            Vector3 headDelta = newHeadPos - lastHeadPos;

            if (headDelta.sqrMagnitude < MovementThreshold * MovementThreshold)
            {
                headDelta = Vector3.zero;
            }

            if (Time.fixedDeltaTime > 0)
            {
                float velAdjustRate = 3f * Time.fixedDeltaTime;
                HeadVelocity = HeadVelocity * (1f - velAdjustRate) + headDelta * velAdjustRate / Time.fixedDeltaTime;

                if (HeadVelocity.sqrMagnitude < VelocityThreshold * VelocityThreshold)
                {
                    HeadVelocity = Vector3.zero;
                }
            }

            // Update headDirection
            float velP = Mathf.Clamp01(Mathf.InverseLerp(headVelIdleThresh, headVelMoveThresh, HeadVelocity.magnitude));

            newHeadMoveDirection = Vector3.Lerp(newHeadPos, HeadVelocity, velP).normalized;
            lastHeadPos = newHeadPos;
            float dirAdjustRate = Mathf.Clamp01(5f * Time.fixedDeltaTime);

            MoveDirection = Vector3.Slerp(MoveDirection, newHeadMoveDirection, dirAdjustRate);

            if (debugDrawHeadDirection)
            {
                Debug.DrawLine(lastHeadPos, lastHeadPos + MoveDirection * 10f, Color.Lerp(Color.red, Color.green, velP));
            }

            if (debugDrawHeadVelocity)
            {
                Debug.DrawLine(lastHeadPos, lastHeadPos + HeadVelocity, Color.yellow);
            }
        }
    }
}