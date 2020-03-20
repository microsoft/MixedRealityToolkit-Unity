// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    [MixedRealityServiceProfile(typeof(ISceneTransitionService))]
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Extensions/Scene Transition Service/Scene Transition Service Profile", fileName = "SceneTransitionServiceProfile", order = 100)]
    public class SceneTransitionServiceProfile : BaseMixedRealityProfile
    {
        public bool UseDefaultProgressIndicator => useDefaultProgressIndicator;
        public GameObject DefaultProgressIndicatorPrefab => defaultProgressIndicatorPrefab;
        public SystemType CameraFaderType => cameraFaderType;

        public bool UseFadeColor => useFadeColor;
        public Color FadeColor => fadeColor;
        public float FadeOutTime => fadeOutTime;
        public float FadeInTime => fadeInTime;
        public CameraFaderTargets FadeTargets => fadeTargets;
        public Material CameraFaderMaterial => cameraFaderMaterial;

        [Header("Progress Indicator Options")]
        [SerializeField]
        [Tooltip("If true, system will instantiate and use defaultProgressIndicatorPrefab for transitions.")]
        private bool useDefaultProgressIndicator = true;

        [SerializeField]
        [Tooltip("The default prefab used to show progress. Must include a script implementing IProgressIndicator.")]
        private GameObject defaultProgressIndicatorPrefab = null;

        [Header("Fade Options")]

        [SerializeField]
        [Tooltip("If checked, the transition service will apply a fade during your transition.")]
        private bool useFadeColor = true;

        [SerializeField]
        [ColorUsage(false)]
        [Tooltip("Controls the color of the fade effect.")]
        private Color fadeColor = Color.black;

        [SerializeField]
        [Range(0, 30)]
        [Tooltip("Default setting for the duration of a fade on entering a transition.")]
        private float fadeOutTime = 1f;

        [SerializeField]
        [Range(0, 30)]
        [Tooltip("Default setting for the duration of a fade on exiting a transition.")]
        private float fadeInTime = 0.5f;

        [SerializeField]
        [Tooltip("Controls which cameras will have a fade effect applied to them.")]
        private CameraFaderTargets fadeTargets = CameraFaderTargets.Main;

        [SerializeField]
        [Implements(typeof(ICameraFader), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("Which `ICameraFader` class to use for applying a fade effect to cameras.")]
        private SystemType cameraFaderType = default(SystemType);

        [Header("Optional Assets")]

        [SerializeField]
        [Tooltip("Optional material for your CameraFader class. If an implementation does not use a material, this will be ignored.")]
        private Material cameraFaderMaterial = null;
    }
}