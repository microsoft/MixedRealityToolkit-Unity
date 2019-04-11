using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Extensions;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
    [MixedRealityServiceProfile(typeof(ISceneTransitionService))]
    [CreateAssetMenu(fileName = "SceneTransitionServiceProfile", menuName = "MixedRealityToolkit/SceneTransitionService Configuration Profile")]
    public class SceneTransitionServiceProfile : BaseMixedRealityProfile
    {
        // Store config data in serialized fields
    }
}