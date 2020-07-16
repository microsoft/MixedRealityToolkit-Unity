// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Boundary
{
    /// <summary>
    /// Service manager supporting running the boundary system, without requiring the MixedRealityToolkit object.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/BoundarySystemManager")]
    public class BoundarySystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The scale (room, world, etc) of the experience.")]
        private ExperienceScale experienceScale = ExperienceScale.Room;

        [SerializeField]
        [Tooltip("The boundary system type that will be instantiated.")]
        [Implements(typeof(IMixedRealityBoundarySystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType BoundarySystemType = null;

        [SerializeField]
        [Tooltip("The boundary visualization configuration profile.")]
        private MixedRealityBoundaryVisualizationProfile profile = null;

        private void Awake()
        {
            InitializeManager();
        }

        protected override void OnDestroy()
        {
            UninitializeManager();
            base.OnDestroy();
        }

        /// <summary>
        /// Initialize the manager.
        /// </summary>
        private void InitializeManager()
        {
            // The boundary system class takes arguments for:
            // * The boundary visualization profile
            // * The desired experience scale
            object[] args = { profile, experienceScale };

            Initialize<IMixedRealityBoundarySystem>(BoundarySystemType.Type, args: args);
        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityBoundarySystem>();
        }
    }
}