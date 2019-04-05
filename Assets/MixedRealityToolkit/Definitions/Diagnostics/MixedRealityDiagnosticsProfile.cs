// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Diagnostics Profile", fileName = "MixedRealityDiagnosticsProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    [MixedRealityServiceProfile(typeof(IMixedRealityDiagnosticsSystem))]
    public class MixedRealityDiagnosticsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [FormerlySerializedAs("visible")]
        [Tooltip("Display all enabled diagnostics")]
        private bool showDiagnostics = true;

        /// <summary>
        /// Show or hide diagnostic visualizations.
        /// </summary>
        public bool ShowDiagnostics => showDiagnostics;

        [SerializeField]
        [Tooltip("Display profiler")]
        private bool showProfiler = true;

        /// <summary>
        /// Show or hide the profiler UI.
        /// </summary>
        public bool ShowProfiler => showProfiler;

        [SerializeField]
        [FormerlySerializedAs("frameRateDuration")]
        [Tooltip("The amount of time, in seconds, to collect frames for frame rate calculation.")]
        [Range(0, 5)]
        private float frameSampleRate = 0.1f;

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        public float FrameSampleRate => frameSampleRate;

        [SerializeField]
        [Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        /// <summary>
        /// What part of the view port to anchor the window to.
        /// </summary>
        public TextAnchor WindowAnchor => windowAnchor;

        [SerializeField]
        [Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        /// <summary>
        /// The offset from the view port center applied based on the window anchor selection.
        /// </summary>
        public Vector2 WindowOffset => windowOffset;

        [SerializeField]
        [Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 1.0f;

        /// <summary>
        /// Use to scale the window size up or down, can simulate a zooming effect.
        /// </summary>
        public float WindowScale => windowScale;

        [SerializeField]
        [Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 5.0f;

        /// <summary>
        /// How quickly to interpolate the window towards its target position and rotation.
        /// </summary>
        public float WindowFollowSpeed => windowFollowSpeed;
    }
}