// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Renders a background mesh for a tool tip using a mesh renderer
    /// If the mesh has an offset anchor point you will get odd results
    /// </summary>
    public class ToolTipBackgroundMesh : MonoBehaviour, IToolTipBackground
    {
        [SerializeField]
        [Tooltip("Transform that scale and offset will be applied to.")]
        private Transform backgroundTransform;

        private float depth = 1f;

        /// <summary>
        /// Mesh renderer button for mesh background.
        /// </summary>
        public MeshRenderer BackgroundRenderer;

        /// <summary>
        /// Determines whether background of Tooltip is visible.
        /// </summary>
        public bool IsVisible
        {
            set
            {
                if (BackgroundRenderer == null)
                    return;

                BackgroundRenderer.enabled = value;
            }
        }

        /// <summary>
        /// The Transform for the background of the Tooltip.
        /// </summary>
        public Transform BackgroundTransform
        {
            get
            {
                return backgroundTransform;
            }

            set
            {
                backgroundTransform = value;
            }
        }

        public void OnContentChange(Vector3 localContentSize, Vector3 localContentOffset, Transform contentParentTransform)
        {
            if (BackgroundRenderer == null)
                return;

            //Get the size of the mesh and use this to adjust the local content size on the x / y axis
            //This will accommodate meshes that aren't built to 1,1 scale
            Bounds meshBounds = BackgroundRenderer.GetComponent<MeshFilter>().sharedMesh.bounds;
            localContentSize.x /= meshBounds.size.x;
            localContentSize.y /= meshBounds.size.y;
            localContentSize.z = depth;

            BackgroundTransform.localScale = localContentSize;
        }
    }
}
