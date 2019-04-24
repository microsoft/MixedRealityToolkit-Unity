// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Scene System Profile", fileName = "MixedRealitySceneSystemProfile", order = (int)CreateProfileMenuItemIndices.SceneSystem)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySceneSystem))]
    public class MixedRealitySceneSystemProfile : BaseMixedRealityProfile
    {
       
    }
}