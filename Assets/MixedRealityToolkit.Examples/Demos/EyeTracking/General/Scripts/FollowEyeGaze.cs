// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Sample for allowing a GameObject to follow the user's eye gaze
    /// at a given distance of "DefaultDistanceInMeters".
    /// </summary>
    public class FollowEyeGaze : MonoBehaviour
    {
        [Tooltip("Display the game object along the eye gaze ray at a default distance (in meters).")]
        [SerializeField]
        private float defaultDistanceInMeters = 2f;

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private void Update()
        {
            // Update GameObject to the current eye gaze position at a given distance
            if (InputSystem?.EyeGazeProvider?.IsEyeGazeValid == true)
            {
                GameObject target = InputSystem.EyeGazeProvider.GazeTarget;
                if (target != null)
                {
                    // Show the object at the center of the currently looked at target.
                    EyeTrackingTarget etTarget = target.GetComponent<EyeTrackingTarget>();
                    if ((etTarget != null) && (etTarget.cursor_snapToCenter))
                    {
                        Ray rayToCenter = new Ray(CameraCache.Main.transform.position, target.transform.position - CameraCache.Main.transform.position);
                        RaycastHit hitInfo;
                        UnityEngine.Physics.Raycast(rayToCenter, out hitInfo);
                        gameObject.transform.position = hitInfo.point;
                    }
                    else
                    {
                        // Show the object at the hit position of the user's eye gaze ray with the target.
                        gameObject.transform.position = InputSystem.EyeGazeProvider.HitPosition;

                    }
                }
                else
                {
                    // If no target is hit, show the object at a default distance along the gaze ray.
                    gameObject.transform.position = InputSystem.EyeGazeProvider.GazeOrigin + InputSystem.EyeGazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
                }
            }
        }
    }
}
