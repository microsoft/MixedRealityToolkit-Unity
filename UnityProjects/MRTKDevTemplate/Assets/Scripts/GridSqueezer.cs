// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demonstration script showing how to squeeze or resize
    /// a grid layout to fit a fixed number of columns, while
    /// resizing children to fit.
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    [ExecuteAlways]
    [AddComponentMenu("MRTK/Examples/Grid Squeezer")]
    public class GridSqueezer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Should the grid squeezer continually update in Play mode?")]
        private bool dynamicFitting = true;

        /// <summary>
        /// Should the grid squeezer continually update in Play mode?
        /// </summary>
        public bool DynamicFitting
        {
            get => dynamicFitting;
            set => dynamicFitting = value;
        }

        [SerializeField]
        [Tooltip("The GridLayoutGroup component attached to this GameObject.")]
        private GridLayoutGroup gridLayoutGroup;

        /// <summary>
        /// The GridLayoutGroup component attached to this GameObject.
        /// </summary>
        public GridLayoutGroup GridLayoutGroup
        {
            get
            {
                if (gridLayoutGroup == null)
                {
                    gridLayoutGroup = GetComponent<GridLayoutGroup>();
                }
                return gridLayoutGroup;
            }
            set => gridLayoutGroup = value;
        }

        [SerializeField]
        [Tooltip("The RectTransform to fit the collider onto.")]
        private RectTransform rectTransform;

        /// <summary>
        /// The <see cref="UnityEngine.RectTransform"/> to fit the collider onto.
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }
                return rectTransform;
            }
            set => rectTransform = value;
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            if (!(Application.isPlaying && !DynamicFitting) && GridLayoutGroup != null && RectTransform != null && RectTransform.hasChanged)
            {
                if (gridLayoutGroup.constraint == UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    gridLayoutGroup.cellSize = new Vector2(rectTransform.rect.width / gridLayoutGroup.constraintCount, gridLayoutGroup.cellSize.x);
                }
            }
        }
    }
}
#pragma warning restore CS1591