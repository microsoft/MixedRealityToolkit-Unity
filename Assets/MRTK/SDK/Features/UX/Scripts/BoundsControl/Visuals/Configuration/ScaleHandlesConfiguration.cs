// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="ScaleHandles"/> used in <see cref="BoundsControl"/>
    /// This class provides all data members needed to create scale handles for <see cref="BoundsControl"/>
    /// </summary>
    [CreateAssetMenu(fileName = "ScaleHandlesConfiguration", menuName = "Mixed Reality/Toolkit/Bounds Control/Scale Handles Configuration")]
    public class ScaleHandlesConfiguration : HandlesBaseConfiguration
    {
        #region serialized fields
        [SerializeField]
        [Tooltip("Prefab used to display handles for 2D slate. If not set, default box shape will be used")]
        GameObject handleSlatePrefab = null;

        /// <summary>
        /// Prefab used to display handles for 2D slate. If not set, default box shape will be used
        /// </summary>
        public GameObject HandleSlatePrefab
        {
            get { return handleSlatePrefab; }
            set
            {
                if (handleSlatePrefab != value)
                {
                    handleSlatePrefab = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Prefab);
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show scale handles")]
        private bool showScaleHandles = true;

        /// <summary>
        /// Public property to Set the visibility of the corner cube Scaling handles.
        /// </summary>
        public bool ShowScaleHandles
        {
            get
            {
                return showScaleHandles;
            }
            set
            {
                if (showScaleHandles != value)
                {
                    showScaleHandles = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        [SerializeField]
        [Tooltip("Scale mode that is applied when interacting with scale handles - default is uniform scaling. Non uniform mode scales the control according to hand / controller movement in space.")]
        private HandleScaleMode scaleBehavior = HandleScaleMode.Uniform;

        /// <summary>
        /// Scale behavior that is applied when interacting with scale handles - default is uniform scaling. Non uniform mode scales the control according to hand / controller movement in space.
        /// </summary>
        public HandleScaleMode ScaleBehavior
        {
            get
            {
                return scaleBehavior;
            }
            set
            {
                if (scaleBehavior != value)
                {
                    scaleBehavior = value;
                }
            }
        }


        #endregion serialized fields

        /// <summary>
        /// Fabricates an instance of ScaleHandles, applying
        /// this config to it whilst creating it.
        /// </summary>
        /// <returns>New TranslationHandles</returns>
        internal virtual ScaleHandles ConstructInstance()
        {
            // Return a new ScaleHandles, using this config as the active config.
            return new ScaleHandles(this);
        }
    }
}
