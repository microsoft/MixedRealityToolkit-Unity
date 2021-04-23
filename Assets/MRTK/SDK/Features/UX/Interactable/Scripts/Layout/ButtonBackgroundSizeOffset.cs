// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Scales an object relative the scale of the Anchor Transform
    /// Works best when using with Layout3DPixelSize, but not required - See LayoutPixelSize for more info
    /// Use Case:
    /// Create a button, then add another element who's size should maintain a consistent size relative to the Anchor.
    /// Like creating a button background using a Cube and ButtonSize. The add another Cube that is 40 pixels smaller than the background.
    ///     Event if the background changes size, this element will remain 40 pixels smaller.
    /// </summary>
    [ExecuteInEditMode]
    [System.Obsolete("This component is no longer supported", true)]
    [AddComponentMenu("Scripts/MRTK/Obsolete/ButtonBackgroundSizeOffset")]
    public class ButtonBackgroundSizeOffset : MonoBehaviour
    {
        /// <summary>
        /// A scale factor for layout3D, default is based on 2048 pixels to 1 meter.
        /// Similar to values used in designer and 2D art programs and helps create consistency across teams.
        /// </summary>
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        [SerializeField]
        private float BasePixelScale = 2048;

        /// <summary>
        /// The transform to offset from. 
        /// </summary>
        [Tooltip("The transform this object should be linked and aligned to")]
        [SerializeField]
        private Transform AnchorTransform = null;

        /// <summary>
        /// Make this object's size scaled relative to the Anchor's size
        /// </summary>
        [Tooltip(" How much to scale compared to the Anchor's size")]
        [SerializeField]
        private Vector3 Scale = Vector3.one;

        /// <summary>
        /// Create an absolute size difference from the Anchor
        /// </summary>
        [Tooltip("That absolute amount to offset the scale")]
        [SerializeField]
        private Vector3 Offset;

        /// <summary>
        /// These scales and positions are applied in Unity Editor only while doing layout.
        /// Turn off for responsive UI type results when editing ItemSize during runtime.
        /// Scales will be applied each frame.
        /// </summary>
        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        [SerializeField]
        private bool OnlyInEditMode = true;

        /// <summary>
        /// Set the objects scale relative to the Anchor
        /// </summary>
        public void SetScale(Vector3 scale)
        {
            Scale = scale;
        }

        // Get the current scale relative to the Anchor
        public Vector3 GetScale()
        {
            return Scale;
        }

        /// <summary>
        /// Set a consistent offset value from the Anchor
        /// </summary>
        public void SetOffset(Vector3 offset)
        {
            Offset = offset;
        }

        /// <summary>
        /// Get the current offset value
        /// </summary>
        public Vector3 GetSOffset()
        {
            return Offset;
        }

        /// <summary>
        /// Set the size based on the Anchor's size and the buffers
        /// </summary>
        private void UpdateSize()
        {
            Vector3 scale = Vector3.Scale(AnchorTransform.localScale, Scale) + Offset / BasePixelScale;
            transform.localScale = scale;
        }

        private void Awake()
        {
            Debug.LogError(this.GetType().Name + " is deprecated");
        }

        void Update()
        {
            if (AnchorTransform != null)
            {
                if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
                {
                    UpdateSize();
                }
            }
        }
    }
}
