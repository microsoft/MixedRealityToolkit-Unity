// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping.Layout
{
    /// <summary>
    /// A sizing and layout system for RectTransforms that help in building and laying out UI
    /// 
    /// Unity has a robust layout system using RectTransforms, Grids and LayoutGroups.
    /// This component helps to layout 3D primitive objects into the RectTransform system for building more complex layouts.
    /// 
    /// For best results, scale the canvas parent element to 0.0005, 0.0005, 0.0005.
    /// Create a child RectTransform and position and size as needed.
    /// Place a regular Transform based element inside the child RectTransform with this component to automatically size and anchor
    /// this element based on it's parent RectTransform settings.
    /// 
    /// This component also adds depth controls.
    /// 
    /// The size values in this component will match the values used in the RectTransforms.
    /// </summary>
    [ExecuteInEditMode]
    public class SizeToRectTransform : MonoBehaviour
    {
        [Tooltip("The RectTransform that will drive the size and position of this object")]
        public RectTransform ParentRectTransform;

        [Tooltip("The pixel ratio conversion to Unity Units or scale")]
        public float ScaleFactor = 2048;

        [Tooltip("Add depth, a value RectTransforms do not support at this time")]
        public float Depth = 10;

        [Tooltip("The x and y margins")]
        public Vector2 EdgeOffset;

        private RectTransform mRectTransform;

        /// <summary>
        /// Get the parent's rect transform if one has not been set already
        /// </summary>
        private void Awake()
        {
            if (ParentRectTransform == null)
            {
                ParentRectTransform = transform.parent.GetComponent<RectTransform>();
            }

            if (ParentRectTransform == null)
            {
                Debug.LogError("The parent of " + name + "does not have a RectTransform!");
            }

            mRectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Set the scale values of the normal Transform to match the RectTransform's size values.
        /// </summary>
        private void UpdateScale()
        {
            if (ParentRectTransform != null)
            {
                if (mRectTransform == null)
                {
                    transform.localScale = new Vector3((ParentRectTransform.rect.width - EdgeOffset.x) / ScaleFactor, (ParentRectTransform.rect.height - EdgeOffset.y) / ScaleFactor, Depth / ScaleFactor);
                }
                else
                {
                    mRectTransform.sizeDelta = new Vector2(ParentRectTransform.rect.width - EdgeOffset.x, ParentRectTransform.rect.height - EdgeOffset.y);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateScale();
        }
    }
}
