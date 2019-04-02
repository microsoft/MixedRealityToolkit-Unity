// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Sample for allowing the game object that this script is attached to follow the user's eye gaze
    /// at a given distance of "DefaultDistanceInMeters". 
    /// </summary>
    public class FollowEyeGaze_GazeProvider : MonoBehaviour
    {
        [Tooltip("Display the game object along the eye gaze ray at a default distance (in meters).")]
        [SerializeField]
        private float defaultDistanceInMeters = 2f;

        private void Update()
        {
            if (MixedRealityToolkit.InputSystem?.GazeProvider != null)
            {
                gameObject.transform.position = MixedRealityToolkit.InputSystem.GazeProvider.GazeOrigin + MixedRealityToolkit.InputSystem.GazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
            }
        }
    }
}