// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Examples.InteractiveElements
{

    public class ScaleWithGesture : GestureInteractiveControl
    {
        public Transform Target;
        public float MinimumScale = 0.3f;
        public Vector3 GestureMultipliers = Vector3.one;

        private NetworkIdentity mNetworkRoot = null;
        private Transform mCameraTransform;
        private Vector3 mStartHandPosition;
        private Vector3 mBaseScale;
        private float mSourceScaleFactor = 1.0f;
        private float mCurrentScaleFactorOffset = 0.0f;
        private Vector3 mPreviousPosition;
        private Vector3 mPreviousHandPosition;

        private void Reset()
        {
            // By default we want this set for ScaleWithGesture.
            GestureData = GestureDataType.Aligned;
        }

        protected override void Awake()
        {
            base.Awake();

            mNetworkRoot = GetComponentInParent<NetworkIdentity>();
            mBaseScale = Target.localScale;
        }

        public override void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {
            switch (gestureState)
            {
                case GestureInteractive.GestureManipulationState.Lost:
                case GestureInteractive.GestureManipulationState.None:
                    return;

                case GestureInteractive.GestureManipulationState.Start:

                    // Build an average of the scale factors, just in case something external changed the scale.
                    mSourceScaleFactor = Target.localScale.x / mBaseScale.x;
                    mSourceScaleFactor += Target.localScale.y / mBaseScale.y;
                    mSourceScaleFactor += Target.localScale.z / mBaseScale.z;
                    mSourceScaleFactor = Mathf.Max(mSourceScaleFactor / 3, MinimumScale);

                    mCameraTransform = Camera.main.transform;
                    mStartHandPosition = mCameraTransform.InverseTransformPoint(startGesturePosition);
                    mPreviousPosition = mCameraTransform.position;
                    mPreviousHandPosition = mStartHandPosition;
                    break;
            }

            Vector3 currentHandPosition = mCameraTransform.InverseTransformPoint(currentGesturePosition);

            // Smooth hand movements
            mPreviousHandPosition = Vector3.Lerp(mPreviousHandPosition, currentHandPosition, 0.5f);

            Vector3 localDirection = mPreviousHandPosition - mStartHandPosition;

            const float MovementZoneRadius = 1.0f / 100.0f;
            float distance = Vector3.Distance(mPreviousPosition, mCameraTransform.position);
            float movementThreshold = 1.0f - Mathf.Sqrt(distance) * MovementZoneRadius;
            movementThreshold = Mathf.Clamp01(movementThreshold);

            mPreviousPosition = Vector3.Lerp(mPreviousPosition, mCameraTransform.position, 0.3f);

            localDirection *= movementThreshold;

            // Scale in all gesture directions.
            mCurrentScaleFactorOffset = localDirection.x * GestureMultipliers.x;
            mCurrentScaleFactorOffset += localDirection.y * GestureMultipliers.y;
            mCurrentScaleFactorOffset += -localDirection.z * GestureMultipliers.z;

            float currentScaleFactor = Mathf.Max(mSourceScaleFactor + mCurrentScaleFactorOffset, MinimumScale);
            Target.localScale = mBaseScale * currentScaleFactor;
            
        }
    }
}
