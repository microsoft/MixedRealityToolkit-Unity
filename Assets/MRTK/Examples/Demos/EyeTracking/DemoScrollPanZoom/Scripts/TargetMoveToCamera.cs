// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [RequireComponent(typeof(EyeTrackingTarget))]
    [AddComponentMenu("Scripts/MRTK/Examples/TargetMoveToCamera")]
    public class TargetMoveToCamera : BaseEyeFocusHandler
    {
        public static TargetMoveToCamera currentlyFocusedTarget;

        public MonoBehaviour[] ActivateBehaviorsWhenInFront;

        public float DistanceToCamera = 6f;

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private bool isEnabled = false;

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

        protected override void Update()
        {
            base.Update();

            if (isEnabled)
            {
                TransitionToCamera();
            }
        }

        protected override void OnEyeFocusStop()
        {
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
            if (!inTransition)
            {
                originalPosition = transform.position;
            }

            if ((currentlyFocusedTarget != null) && (currentlyFocusedTarget != this))
                currentlyFocusedTarget.ReturnHome();

            currentlyFocusedTarget = this;
            isInNearFocusMode = true;
            inTransition = true;
        }

        public void ReturnHome()
        {
            isInNearFocusMode = false;
            inTransition = true;
            if (setToAutoRotateIfFocused)
            {
                if ((ActivateBehaviorsWhenInFront != null) && (ActivateBehaviorsWhenInFront.Length > 0))
                {
                    for (int i = 0; i < ActivateBehaviorsWhenInFront.Length; i++)
                    {
                        ActivateBehaviorsWhenInFront[i].enabled = false;
                    }
                }
            }
        }

        private void TransitionToCamera()
        {
            if (inTransition)
            {
                Vector3 destination = (isInNearFocusMode) ?
                    (CameraCache.Main.transform.position + (CameraCache.Main.transform.forward * DistanceToCamera))
                    : originalPosition;

                Vector3 incr = (destination - gameObject.transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(destination, transform.position) < minDistToStopTransition)
                {
                    gameObject.transform.position = new Vector3(destination.x, destination.y, destination.z);
                    inTransition = false;

                    if ((!isInNearFocusMode) && (currentlyFocusedTarget == this))
                        currentlyFocusedTarget = null;

                    if (isInNearFocusMode && setToAutoRotateIfFocused)
                    {
                        if ((ActivateBehaviorsWhenInFront != null) && (ActivateBehaviorsWhenInFront.Length > 0))
                        {
                            for (int i = 0; i < ActivateBehaviorsWhenInFront.Length; i++)
                            {
                                ActivateBehaviorsWhenInFront[i].enabled = true;
                            }
                        }
                    }
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
