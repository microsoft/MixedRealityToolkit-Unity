// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="Links"/> used in <see cref="BoundsControl"/>
    /// This class provides all data members needed to create a link of a bounds control
    /// </summary>
    [CreateAssetMenu(fileName = "LinksConfiguration", menuName = "Mixed Reality/Toolkit/Bounds Control/Links Configuration")]
    public class LinksConfiguration : ScriptableObject
    {
        #region Serialized Properties
        [SerializeField]
        [Tooltip("Material used for wireframe display")]
        private Material wireframeMaterial;

        /// <summary>
        /// Material used for wireframe display
        /// </summary>
        public Material WireframeMaterial
        {
            get { return wireframeMaterial; }
            set
            {
                if (wireframeMaterial != value)
                {
                    wireframeMaterial = value;
                    TrySetDefaultMaterial();
                    wireFrameChanged.Invoke(WireframeChangedEventType.Material);
                }
            }
        }

        [SerializeField]
        [Tooltip("Radius for wireframe edges")]
        private float wireframeEdgeRadius = 0.001f;

        /// <summary>
        /// Radius for wireframe edges
        /// </summary>
        public float WireframeEdgeRadius
        {
            get { return wireframeEdgeRadius; }
            set
            {
                if (wireframeEdgeRadius != value)
                {
                    wireframeEdgeRadius = value;
                    wireFrameChanged.Invoke(WireframeChangedEventType.Radius);
                }
            }
        }

        [SerializeField]
        [Tooltip("Shape used for wireframe display")]
        private WireframeType wireframeShape = WireframeType.Cubic;

        /// <summary>
        /// Shape used for wireframe display
        /// </summary>
        public WireframeType WireframeShape
        {
            get { return wireframeShape; }
            set
            {
                if (wireframeShape != value)
                {
                    wireframeShape = value;
                    wireFrameChanged.Invoke(WireframeChangedEventType.Shape);
                }
            }
        }

        [SerializeField]
        [Tooltip("Show a wireframe around the bounds control when checked. Wireframe parameters below have no effect unless this is checked")]
        private bool showWireframe = true;

        /// <summary>
        /// Show a wireframe around the bounds control when checked. Wireframe parameters below have no effect unless this is checked
        /// </summary>
        public bool ShowWireFrame
        {
            get { return showWireframe; }
            set
            {
                if (showWireframe != value)
                {
                    showWireframe = value;
                    wireFrameChanged.Invoke(WireframeChangedEventType.Visibility);
                }
            }
        }

        #endregion Serialized Properties

        internal enum WireframeChangedEventType
        {
            Visibility,
            Radius,
            Shape,
            Material
        }
        internal class WireFrameEvent : UnityEvent<WireframeChangedEventType> { }
        internal WireFrameEvent wireFrameChanged = new WireFrameEvent();

        public void Awake()
        {
            TrySetDefaultMaterial();
        }

        private void TrySetDefaultMaterial()
        {
            if (wireframeMaterial == null)
            {
                wireframeMaterial = VisualUtils.CreateDefaultMaterial();
            }
        }
    }
}
