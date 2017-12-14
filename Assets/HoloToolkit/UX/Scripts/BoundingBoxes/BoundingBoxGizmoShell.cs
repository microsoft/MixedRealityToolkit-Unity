//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Draws a bounding box gizmo in the style of the hololens shell
    /// </summary>
    [ExecuteInEditMode]
    public class BoundingBoxGizmoShell : BoundingBoxGizmo
    {
        #region public

        /// <summary>
        /// If true, bounding box edges will display when active
        /// To more closely mimic shell behavior, set this to false
        /// </summary>
        public bool DisplayEdgesWhenSelected = true;

        /// <summary>
        /// Whether to clamp handle size between HandleScaleMin / Max
        /// This may result in bunched-up handles so use with caution
        /// </summary>
        public bool ClampHandleScale = false;

        /// <summary>
        /// User-defined min size for handle
        /// Useful when using the bounding box with extremely small objects
        /// This value is ignored if ClampHandleSize is false
        /// </summary>
        public float HandleScaleMin = 0.1f;

        /// <summary>
        /// User-defined min size for handle
        /// Useful when using the bounding box with extremely large objects
        /// This value is ignored if ClampHandleSize is false
        /// </summary>
        public float HandleScaleMax = 2.0f;

        #endregion

        #region protected

        [SerializeField]
        protected Renderer edgeRenderer;

        [SerializeField]
        protected GameObject xyzHandlesObject;
        [SerializeField]
        protected GameObject xyHandlesObject;
        [SerializeField]
        protected GameObject xzHandlesObject;
        [SerializeField]
        protected GameObject zyHandlesObject;

        #endregion

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(BoundingBoxGizmoShell))]
        public class CustomEditor : MRTKEditor  { }
#endif
        #endregion
    }
}