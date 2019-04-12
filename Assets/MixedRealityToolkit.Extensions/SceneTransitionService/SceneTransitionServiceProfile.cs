// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    [MixedRealityServiceProfile(typeof(ISceneTransitionService))]
    [CreateAssetMenu(fileName = "SceneTransitionServiceProfile", menuName = "MixedRealityToolkit/SceneTransitionService Configuration Profile")]
    public class SceneTransitionServiceProfile : BaseMixedRealityProfile
    {
        public bool UseProgressIndicator => useProgressIndicator;
        public GameObject ProgressIndicatorPrefab => progressIndicatorPrefab;

        public bool UseFadeColor => useFadeColor;
        public Color FadeColor => fadeColor;
        public float FadeOutTime => fadeOutTime;
        public float FadeInTime => fadeInTime;
        public CameraFaderTargets FadeTargets => fadeTargets;

        [Header("Progress Indicator Options")]
        [SerializeField]
        private bool useProgressIndicator = true;
        [SerializeField]
        [Tooltip("The prefab used to show progress. Must include a scipt implementing IProgressIndicator.")]
        private GameObject progressIndicatorPrefab = null;

        [Header("Fade Options")]
        [SerializeField]
        private bool useFadeColor = true;
        [SerializeField]
        private Color fadeColor = Color.black;
        [SerializeField]
        private float fadeOutTime = 1f;
        [SerializeField]
        private float fadeInTime = 0.5f;
        [SerializeField]
        private CameraFaderTargets fadeTargets = CameraFaderTargets.Main;
    }
}