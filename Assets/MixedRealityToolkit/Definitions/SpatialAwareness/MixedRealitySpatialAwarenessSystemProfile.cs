// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for spatial awareness mesh observers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Spatial Awareness System Profile", fileName = "MixedRealitySpatialAwarenessSystemProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySpatialAwarenessSystem))]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class MixedRealitySpatialAwarenessSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealitySpatialObserverConfiguration[] observerConfigurations = System.Array.Empty<MixedRealitySpatialObserverConfiguration>();

        public MixedRealitySpatialObserverConfiguration[] ObserverConfigurations
        {
            get { return observerConfigurations; }
            internal set { observerConfigurations = value; }
        }
    }
}