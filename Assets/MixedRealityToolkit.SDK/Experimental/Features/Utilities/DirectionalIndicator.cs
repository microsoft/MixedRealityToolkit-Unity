// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    [RequireComponent(typeof(Orbital))]
    public class DirectionalIndicator : MonoBehaviour
    {
        // reference orbital

        public GameObject TrackedTarget;

        //directionIndicatorDefaultRotation

        // directionalIndicator?

        private void Update()
        {
            // should be visible?
            // Calculate orientation
        }

        private bool IsTargetVisible()
        {
            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = mainCamera.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > VisibilitySafeFactor && targetViewportPosition.x < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.y > VisibilitySafeFactor && targetViewportPosition.y < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.z > 0);
        }

        private void GetDirectionIndicatorPositionAndRotation(Vector3 camToObjectDirection, Transform cameraTransform, out Vector3 position, out Quaternion rotation)
        {
            // Find position:
            // Save the cursor transform position in a variable.
            Vector3 origin = Cursor.transform.position;
            // Project the camera to target direction onto the screen plane.
            Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * cameraTransform.forward);
            cursorIndicatorDirection.Normalize();

            // If the direction is 0, set the direction to the right.
            // This will only happen if the camera is facing directly away from the target.
            if (cursorIndicatorDirection == Vector3.zero)
            {
                cursorIndicatorDirection = cameraTransform.right;
            }

            // The final position is translated from the center of the screen along this direction vector.
            position = origin + cursorIndicatorDirection * MetersFromCursor;

            // Find the rotation from the facing direction to the target object.
            rotation = Quaternion.LookRotation(cameraTransform.forward, cursorIndicatorDirection) * directionIndicatorDefaultRotation;
        }
    }
}
}
