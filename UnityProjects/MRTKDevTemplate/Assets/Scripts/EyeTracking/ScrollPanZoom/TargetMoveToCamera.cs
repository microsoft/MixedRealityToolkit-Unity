// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [AddComponentMenu("Scripts/MRTK/Examples/TargetMoveToCamera")]
    public class TargetMoveToCamera : OnLookAtRotateByEyeGaze
    {
        public static TargetMoveToCamera currentlyFocusedTarget;

        public float DistanceToCamera = 6f;

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private float minDistToStopTransition = 1f;

        [SerializeField]
        private bool setToAutoRotateIfFocused = true;

        private Vector3 originalPosition;
        private bool inTransition = false;
        private bool isInNearFocusMode = false;

        private void Start()
        {
            originalPosition = transform.position;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                TransitionToCamera();

                if (isInNearFocusMode && setToAutoRotateIfFocused)
                {
                    base.ProcessInteractable(updatePhase);
                }
            }
        }

        public void OnSelect()
        {
            if (isInNearFocusMode)
            {
                ReturnHome();
            }
            else
            {
                TransitionToUser();
            }
        }

        public void TransitionToUser()
        {
            SpeechRecognitionKeyword = "send back";

            if (!inTransition)
            {
                originalPosition = transform.position;
            }

            if (currentlyFocusedTarget != null && currentlyFocusedTarget != this)
                currentlyFocusedTarget.ReturnHome();

            currentlyFocusedTarget = this;
            isInNearFocusMode = true;
            inTransition = true;
        }

        public void ReturnHome()
        {
            SpeechRecognitionKeyword = "come to me";

            isInNearFocusMode = false;
            inTransition = true;
        }

        private void TransitionToCamera()
        {
            if (inTransition)
            {
                Vector3 destination = isInNearFocusMode ?
                    Camera.main.transform.position + (Camera.main.transform.forward * DistanceToCamera)
                    : originalPosition;

                Vector3 incr = speed * Time.deltaTime * (destination - gameObject.transform.position);

                if (Vector3.Distance(destination, transform.position) < minDistToStopTransition)
                {
                    gameObject.transform.position = new Vector3(destination.x, destination.y, destination.z);
                    inTransition = false;

                    if (!isInNearFocusMode && currentlyFocusedTarget == this)
                        currentlyFocusedTarget = null;
                }
                else
                {
                    Vector3 oldPos = gameObject.transform.position;
                    gameObject.transform.position = new Vector3(oldPos.x + incr.x, oldPos.y + incr.y, oldPos.z + incr.z);
                }
            }
        }
    }
}
