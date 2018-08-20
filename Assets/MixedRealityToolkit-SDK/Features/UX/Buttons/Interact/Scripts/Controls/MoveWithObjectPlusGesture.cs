// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Controls
{
    /// <summary>
    /// Adds gesture to the gaze manipulation of MoveWithObject
    /// </summary>
    public class MoveWithObjectPlusGesture : MoveInteractiveWithObject
    {
        
        public bool GestureWithGaze = true;
        public float MoveDistanceMultiplier = 1;
        public float AverageGestureDistance = 0.25f;
        public Vector3 GestureOffset;

        private GestureInteractive Gesture;

        private Transform cameraTransform;
        private Vector3 startHandPosition;
        private Vector3 previousForward;
        private Vector3 previousPosition;
        private Vector3 previousHandPosition;
        private Vector3 directionVector;
        
        private Vector3 mPreviousGestureOffset = Vector3.zero;

        // Use this for initialization
        protected override void Start()
        {
            if (ReferenceInteractive == null)
            {
                ReferenceInteractive = this.gameObject;
            }

            Gesture = ReferenceInteractive.GetComponent<GestureInteractive>();
            GestureOffset = Vector3.zero;
        }
        
        protected override void UpdatePosition(Vector3 position, float time)
        {
            //base.UpdatePosition(position, time);

            Vector3 newPosition;
            if (GestureWithGaze)
            {
                newPosition = position + GestureOffset;
            }
            else
            {
                newPosition = transform.position + (GestureOffset - mPreviousGestureOffset);
                mPreviousGestureOffset = GestureOffset;
            }

            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, LerpPositionSpeed * time);

            if (FaceObject)
            {
                Quaternion forwardRotation = Quaternion.LookRotation(this.transform.position - ReferenceObject.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, forwardRotation, LerpRotationSpeed * time);
            }

            if (KeepUpRight)
            {
                Quaternion upRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up);
                this.transform.rotation = upRotation * this.transform.rotation;
            }
        }

        private void GetGestureData()
        {
            GestureHandData data = Gesture.GetGestureHandData();

            //print(data.State);
            
            if (data.State == GestureInteractive.GestureManipulationState.Lost || data.State == GestureInteractive.GestureManipulationState.None)
            {
                StopRunning();
                GestureOffset = Vector3.zero;
            }
            else
            {
                if (data.State == GestureInteractive.GestureManipulationState.Start)
                {
                    cameraTransform = Camera.main.transform;
                    startHandPosition = cameraTransform.InverseTransformPoint(data.StartGesturePosition);
                    previousForward = cameraTransform.forward;
                    previousPosition = cameraTransform.position;
                    previousHandPosition = startHandPosition;

                    StartRunning();
                }

                Vector3 currentHandPosition = cameraTransform.InverseTransformPoint(data.CurrentGesturePosition);

                // Smooth hand movements
                previousHandPosition = Vector3.Lerp(previousHandPosition, currentHandPosition, 0.5f);

                // Calculating the direction in local space of the camera.
                Vector3 localDirection = previousHandPosition - startHandPosition;
                // Need to convert back to world space to get proper direction relative to how the user is facing.
                directionVector = cameraTransform.TransformDirection(localDirection);

                // Throttles the gesture to a max distance.
                Vector3 updatedOffset = directionVector;
                float percentage = updatedOffset.magnitude / AverageGestureDistance;
                updatedOffset.Normalize();

                // Faster user moves their gaze the less the CurrentOffset applies.
                float rotationThreshold = Vector3.Dot(previousForward, cameraTransform.forward) * 0.5f;
                previousForward = Vector3.Lerp(previousForward, cameraTransform.forward, 0.3f);
                previousForward.Normalize();

                const float MovementZoneRadius = 0.5f;
                float movementThreshold = 1.0f - Mathf.Clamp01(Vector3.Distance(previousPosition, cameraTransform.position) / MovementZoneRadius);
                previousPosition = Vector3.Lerp(previousPosition, cameraTransform.position, 0.3f);

                float finalPercentage = percentage * MoveDistanceMultiplier * rotationThreshold * movementThreshold;
                GestureOffset = updatedOffset * finalPercentage;

            }
        }

        protected override void Update()
        {
            if(Gesture != null)
            {
                GetGestureData();
            }

            if (!IsRunning)
            {
                mPreviousGestureOffset = Vector3.zero;
            }

            base.Update();
        }
    }
}
