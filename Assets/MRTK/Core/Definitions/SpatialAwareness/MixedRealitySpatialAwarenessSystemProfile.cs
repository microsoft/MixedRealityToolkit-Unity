// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for spatial awareness mesh observers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Spatial Awareness System Profile", fileName = "MixedRealitySpatialAwarenessSystemProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySpatialAwarenessSystem))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
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