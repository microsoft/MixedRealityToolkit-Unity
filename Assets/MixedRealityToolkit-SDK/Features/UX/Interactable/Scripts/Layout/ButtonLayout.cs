// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Place an object in space relative to another object's scale (See ButtonSize for more info)
    ///     Easily layout button elements in 3D space, in the editor.
    ///     Good for responsive buttons that can stretch and object realignment during runtime
    ///     Works best when using with ButtonSize, but not requied
    /// Use Case:
    /// Create a button and add an element, like an icon the needs to align to the left, even if the background changes size.
    /// </summary>
    [ExecuteInEditMode]
    public class ButtonLayout : MonoBehaviour
    {
        /// <summary>
        /// A vector that sets the transform position from the Anchor's center point.
        /// Vector3.right would aling this center point to the Anchor's right side, based on it's scale
        /// </summary>
        [Tooltip("Where to set this object's center point in relation to the Anchor's center point")]
        [SerializeField]
        private Vector3 Alignment;

        /// <summary>
        /// A scale factor for button layouts, default is based on 2048 pixels to 1 meter.
        /// Similar to values used in designer and 2D art programs and helps create consistancy across teams.
        /// </summary>
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        [SerializeField]
        private float BasePixelSize = 2048;

        /// <summary>
        /// The transform to offset from. 
        /// </summary>
        [Tooltip("The transform this object should be linked and aligned to")]
        [SerializeField]
        private Transform Anchor;

        /// <summary>
        /// An absolute value to offset this object from the center point.
        /// Could be ItemSize / 2 if also using ButtonSize.
        /// </summary>
        [Tooltip("Offset this object's position based on the same pixel based size ratio")]
        [SerializeField]
        private Vector3 AnchorOffset;

        /// <summary>
        /// These positions are applied in Unity Editor only while doing layout.
        /// Turn off for responsive UI type results when editing ItemSize during runtime.
        /// Scales will be applied each frame.
        /// </summary>
        [SerializeField]
        private bool OnlyInEditMode = true;

        /// <summary>
        /// Set the alignment with code or during runtime
        /// </summary>
        /// <param name="alignment"></param>
        public void SetAlignment(Vector3 alignment)
        {
            Alignment = alignment;
        }

        /// <summary>
        /// Get the alignment
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAlignment()
        {
            return Alignment;
        }

        /// <summary>
        /// A transform is required for alignment
        /// </summary>
        protected virtual void Awake()
        {
            if (Anchor == null)
            {
                Anchor = this.transform;
            }
        }

        /// <summary>
        /// Set this object's position
        /// </summary>
        protected virtual void UpdatePosition()
        {
            // set the default directions
            Vector3 startPosition = Anchor.localPosition;

            if (Anchor != this.transform)
            {
                startPosition = Anchor.localPosition + (Vector3.Scale(Anchor.localScale * 0.5f, Alignment));
            }

            transform.localPosition = startPosition + (AnchorOffset / BasePixelSize);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Anchor != null)
            {
                if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
                {
                    UpdatePosition();
                }
            }
        }
    }
}
