// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Common.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Get the horizontal FOV from the stereo camera
        /// </summary>
        /// <returns></returns>
        public static float GetHorizontalFieldOfViewRadians(this Camera camera)
        {
            return 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * camera.aspect);
        }

        /// <summary>
        /// Returns if a point will be rendered on the screen in either eye
        /// </summary>
        /// <param name="camera">The camera to check the point against</param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsInFOV(this Camera camera, Vector3 position)
        {
            float verticalFovHalf = camera.fieldOfView * 0.5f;
            float horizontalFovHalf = camera.GetHorizontalFieldOfViewRadians() * Mathf.Rad2Deg * 0.5f;

            Vector3 deltaPos = position - camera.transform.position;
            Vector3 headDeltaPos = MathUtils.TransformDirectionFromTo(null, camera.transform, deltaPos).normalized;

            float yaw = Mathf.Asin(headDeltaPos.x) * Mathf.Rad2Deg;
            float pitch = Mathf.Asin(headDeltaPos.y) * Mathf.Rad2Deg;

            return (Mathf.Abs(yaw) < horizontalFovHalf && Mathf.Abs(pitch) < verticalFovHalf);
        }
    }
}