// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    public class BoundarySystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        private ExperienceScale expereinceScale = ExperienceScale.Room;

        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealityBoundarySystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType BoundarySystemType = null;

        // todo: tooltips, etc
        [SerializeField]
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
        ///  Initialize the manager.
        /// </summary>
        private void InitializeManager()
        {
            // The boundary system class takes arguments for:
            // * The registrar
            // * The boundary visualization profile
            // * The desired experience scale
            object[] args = { this, profile, expereinceScale };

            Initialize<IMixedRealityBoundarySystem>(BoundarySystemType.Type, args: args);
        }

        /// <summary>
        ///  Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityBoundarySystem>();
        }
    }
}