// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    /// <summary>
    /// Listens to a BoundingBoxManipulate object and draws a gizmo around the target object
    /// </summary>
    [ExecuteInEditMode]
    public class BoundingBoxGizmo : MonoBehaviour
    {
        /// <summary>
        /// The bounding box we're following
        /// </summary>
        [SerializeField]
        protected BoundingBox boundingBox;

        /// <summary>
        /// Draws any custom gizmo elements around the manipulator's target object
        /// </summary>
        protected virtual void DrawGizmoObjects() {
            // empty
        } 

        protected virtual void LateUpdate() {
            DrawGizmoObjects();
        }
    }
}