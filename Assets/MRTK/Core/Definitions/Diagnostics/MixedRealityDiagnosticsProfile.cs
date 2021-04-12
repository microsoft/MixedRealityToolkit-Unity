// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Diagnostics Profile", fileName = "MixedRealityDiagnosticsProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    [MixedRealityServiceProfile(typeof(IMixedRealityDiagnosticsSystem))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/diagnostics/diagnostics-system-getting-started")]
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
        [Tooltip("Display the frame info (per frame stats).")]
        private bool showFrameInfo = true;

        /// <summary>
        /// Show or hide the frame info (per frame stats).
        /// </summary>
        public bool ShowFrameInfo => showFrameInfo;

        [SerializeField]
        [Tooltip("Display the memory stats (used, peak, and limit).")]
        private bool showMemoryStats = true;

        /// <summary>
        /// Show or hide the memory stats (used, peak, and limit).
        /// </summary>
        public bool ShowMemoryStats => showMemoryStats;

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

        [SerializeField]
        [Tooltip("A material that the diagnostics system can use to render objects with instanced color support.")]
        private Material defaultInstancedMaterial = null;

        /// <summary>
        /// A material that the diagnostics system can use to render objects with instanced color support.
        /// A asset reference is required here to make sure the shader permutation is pulled into player builds.
        /// </summary>
        public Material DefaultInstancedMaterial => defaultInstancedMaterial;

        [SerializeField]
        [Tooltip("If the diagnostics profiler should be visible while a mixed reality capture is happening on HoloLens.")]
        private bool showProfilerDuringMRC = false;

        /// <summary>
        /// If the diagnostics profiler should be visible while a mixed reality capture is happening on HoloLens.
        /// </summary>
        /// <remarks>This is not usually recommended, as MRC can have an effect on an app's frame rate.</remarks>
        public bool ShowProfilerDuringMRC => showProfilerDuringMRC;
    }
}