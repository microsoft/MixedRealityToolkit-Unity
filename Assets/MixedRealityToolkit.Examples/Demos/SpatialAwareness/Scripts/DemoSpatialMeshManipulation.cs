// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    ///
    /// </summary>
    public class DemoSpatialMeshManipulation : MonoBehaviour
    {
        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The currently active instance of <see cref="IMixedRealitySpatialAwarenessSystem"/>.
        /// </summary>
        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => spatialAwarenessSystem ?? (spatialAwarenessSystem = MixedRealityToolkit.SpatialAwarenessSystem);

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers = null;

        private void Start()
        {
            observers = SpatialAwarenessSystem?.GetObservers<IMixedRealitySpatialAwarenessMeshObserver>();
            Debug.Log($"{observers.Count} Spatial Awareness mesh observers.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public void ChangeMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].DisplayOption = option;
            }
        }
    }
}
