using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Extensions;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
    [MixedRealityServiceProfile(typeof(ISceneTransitionService))]
    [CreateAssetMenu(fileName = "SceneTransitionServiceProfile", menuName = "MixedRealityToolkit/SceneTransitionService Configuration Profile")]
    public class SceneTransitionServiceProfile : BaseMixedRealityProfile
    {
        public GameObject ProgressIndicatorPrefab => progressIndicatorPrefab;

        [SerializeField]
        [Tooltip("The prefab used to show progress. Must include a scipt implementing IProgressIndicator.")]
        private GameObject progressIndicatorPrefab;
    }
}