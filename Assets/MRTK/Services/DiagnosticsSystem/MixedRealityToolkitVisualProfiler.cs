// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Text;
using Unity.Profiling;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Media.Capture;
using Windows.System;
#else
using UnityEngine.Profiling;
#endif

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// 
    /// ABOUT: The VisualProfiler provides a drop in, single file, solution for viewing 
    /// your Windows Mixed Reality Unity application's frame rate and memory usage. Missed 
    /// frames are displayed over time to visually find problem areas. Memory is reported 
    /// as current, peak and max usage in a bar graph. 
    /// 
    /// USAGE: To use this profiler simply add this script as a component of any GameObject in 
    /// your Unity scene. The profiler is initially enabled (toggle-able via the initiallyActive 
    /// property), but can be toggled via the enabled/disable voice commands keywords.
    /// 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/MixedRealityToolkitVisualProfiler")]
    public class MixedRealityToolkitVisualProfiler : MonoBehaviour
    {
        private static readonly int maxStringLength = 32;
        private static readonly int maxTargetFrameRate = 120;
        private static readonly int maxFrameTimings = 128;
        private static readonly int frameRange = 30;
        private static readonly Vector2 defaultWindowRotation = new Vector2(10.0f, 20.0f);
        private static readonly Vector3 defaultWindowScale = new Vector3(0.2f, 0.04f, 1.0f);
        private static readonly Vector3[] backgroundScales = { new Vector3(1.05f, 1.2f, 1.2f), new Vector3(1.0f, 0.5f, 1.0f), new Vector3(1.0f, 0.25f, 1.0f) };
        private static readonly Vector3[] backgroundOffsets = { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.25f, 0.0f), new Vector3(0.0f, 0.375f, 0.0f) };
        private static readonly string usedMemoryString = "Used: ";
        private static readonly string peakMemoryString = "Peak: ";
        private static readonly string limitMemoryString = "Limit: ";
        private static readonly string voiceCommandString = "Say \"Toggle Profiler\" to show/hide";
        private static readonly string visualProfilerTitleString = "MRTK Visual Profiler";

        public Transform WindowParent { get; set; } = null;

        [Header("Profiler Settings")]
        [SerializeField, Tooltip("Is the profiler currently visible.")]
        private bool isVisible = false;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        private bool ShouldShowProfiler =>
#if WINDOWS_UWP
            (appCapture == null || !appCapture.IsCapturingVideo || showProfilerDuringMRC) &&
#endif // WINDOWS_UWP
            isVisible;

        [SerializeField, Tooltip("Should the frame info (colored bars) be displayed.")]
        private bool frameInfoVisible = true;

        public bool FrameInfoVisible
        {
            get { return frameInfoVisible; }
            set { frameInfoVisible = value; }
        }

        [SerializeField, Tooltip("Should memory stats (used, peak, and limit) be displayed.")]
        private bool memoryStatsVisible = true;

        public bool MemoryStatsVisible
        {
            get { return memoryStatsVisible; }
            set { memoryStatsVisible = value; }
        }

        [SerializeField, Tooltip("The amount of time, in seconds, to collect frames for frame rate calculation.")]
        private float frameSampleRate = 0.1f;

        public float FrameSampleRate
        {
            get { return frameSampleRate; }
            set { frameSampleRate = value; }
        }

        [Header("Window Settings")]
        [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        public TextAnchor WindowAnchor
        {
            get { return windowAnchor; }
            set { windowAnchor = value; }
        }

        [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        public Vector2 WindowOffset
        {
            get { return windowOffset; }
            set { windowOffset = value; }
        }

        [SerializeField, Range(0.5f, 5.0f), Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 1.0f;

        public float WindowScale
        {
            get { return windowScale; }
            set { windowScale = Mathf.Clamp(value, 0.5f, 5.0f); }
        }

        [SerializeField, Range(0.0f, 100.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 5.0f;

        public float WindowFollowSpeed
        {
            get { return windowFollowSpeed; }
            set { windowFollowSpeed = Mathf.Abs(value); }
        }

        [SerializeField]
        [Tooltip("If the diagnostics profiler should be visible while a mixed reality capture is happening on HoloLens.")]
        private bool showProfilerDuringMRC = false;

        /// <summary>
        /// If the diagnostics profiler should be visible while a mixed reality capture is happening on HoloLens.
        /// </summary>
        /// <remarks>This is not usually recommended, as MRC can have an effect on an app's frame rate.</remarks>
        public bool ShowProfilerDuringMRC
        {
            get { return showProfilerDuringMRC; }
            set { showProfilerDuringMRC = value; }
        }

        [Header("UI Settings")]
        [SerializeField, Range(0, 3), Tooltip("How many decimal places to display on numeric strings.")]
        private int displayedDecimalDigits = 1;

        [System.Serializable]
        private struct FrameRateColor
        {
            [Range(0.0f, 1.0f), Tooltip("The percentage of the target frame rate.")]
            public float percentageOfTarget;
            [Tooltip("The color to display for frames which meet or exceed the percentage of the target frame rate.")]
            public Color color;
        }

        [SerializeField, Tooltip("A list of colors to display for different percentage of target frame rates.")]
        private FrameRateColor[] frameRateColors = new FrameRateColor[]
        {
            // Green
            new FrameRateColor() { percentageOfTarget = 0.95f, color = new Color(127 / 256.0f, 186 / 256.0f, 0 / 256.0f, 1.0f) },
            // Yellow
            new FrameRateColor() { percentageOfTarget = 0.75f, color = new Color(255 / 256.0f, 185 / 256.0f, 0 / 256.0f, 1.0f) },
            // Red
            new FrameRateColor() { percentageOfTarget = 0.0f, color = new Color(255 / 256.0f, 0 / 256.0f, 0 / 256.0f, 1.0f) },
        };

        [SerializeField, Tooltip("The color of the window backplate.")]
        private Color baseColor = new Color(80 / 256.0f, 80 / 256.0f, 80 / 256.0f, 1.0f);
        [SerializeField, Tooltip("The color to display for current memory usage values.")]
        private Color memoryUsedColor = new Color(0 / 256.0f, 164 / 256.0f, 239 / 256.0f, 1.0f);
        [SerializeField, Tooltip("The color to display for peak (aka max) memory usage values.")]
        private Color memoryPeakColor = new Color(255 / 256.0f, 185 / 256.0f, 0 / 256.0f, 1.0f);
        [SerializeField, Tooltip("The color to display for the platforms memory usage limit.")]
        private Color memoryLimitColor = new Color(150 / 256.0f, 150 / 256.0f, 150 / 256.0f, 1.0f);

        private Transform window;
        private Transform background;
        private TextMesh cpuFrameRateText;
        private TextMesh gpuFrameRateText;
        private Transform memoryStats;
        private TextMesh usedMemoryText;
        private TextMesh peakMemoryText;
        private TextMesh limitMemoryText;
        private TextMesh voiceCommandText;
        private TextMesh mrtkText;
        private Transform usedAnchor;
        private Transform peakAnchor;
        private Quaternion windowHorizontalRotation;
        private Quaternion windowHorizontalRotationInverse;
        private Quaternion windowVerticalRotation;
        private Quaternion windowVerticalRotationInverse;

        private Matrix4x4[] frameInfoMatrices;
        private Vector4[] frameInfoColors;
        private MaterialPropertyBlock frameInfoPropertyBlock;
        private int colorID;
        private int parentMatrixID;
        private int frameCount;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private FrameTiming[] frameTimings = new FrameTiming[maxFrameTimings];
        private string[] cpuFrameRateStrings;
        private string[] gpuFrameRateStrings;
        private char[] stringBuffer = new char[maxStringLength];

        private ulong memoryUsage;
        private ulong peakMemoryUsage;
        private ulong limitMemoryUsage;

        // Rendering resources.
        [SerializeField, HideInInspector]
        private Material defaultMaterial;
        [SerializeField, HideInInspector]
        private Material defaultInstancedMaterial;
        private Material backgroundMaterial;
        private Material foregroundMaterial;
        private Material textMaterial;
        private Mesh quadMesh;

#if WINDOWS_UWP
        private AppCapture appCapture;
#endif // WINDOWS_UWP

        private void Reset()
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                defaultMaterial.SetFloat("_ZWrite", 1.0f);
                defaultMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                defaultMaterial.renderQueue = 5000;
            }

            if (defaultInstancedMaterial == null)
            {
                Shader defaultInstancedShader = Shader.Find("Hidden/Instanced-Colored");

                if (defaultInstancedShader != null)
                {
                    defaultInstancedMaterial = new Material(defaultInstancedShader);
                    defaultInstancedMaterial.enableInstancing = true;
                    defaultInstancedMaterial.SetFloat("_ZWrite", 1.0f);
                    defaultInstancedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                    defaultInstancedMaterial.renderQueue = 5000;
                }
                else
                {
                    Debug.LogWarning("A shader supporting instancing could not be found for the VisualProfiler, falling back to traditional rendering. This may impact performance.");
                }
            }

            if (Application.isPlaying)
            {
                backgroundMaterial = new Material(defaultMaterial);
                foregroundMaterial = new Material(defaultMaterial);
                defaultMaterial.renderQueue = foregroundMaterial.renderQueue - 1;
                backgroundMaterial.renderQueue = defaultMaterial.renderQueue - 1;

                MeshRenderer meshRenderer = new GameObject().AddComponent<TextMesh>().GetComponent<MeshRenderer>();
                textMaterial = new Material(meshRenderer.sharedMaterial);
                textMaterial.renderQueue = defaultMaterial.renderQueue;
                Destroy(meshRenderer.gameObject);

                MeshFilter quadMeshFilter = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>();

                if (defaultInstancedMaterial != null)
                {
                    // Create a quad mesh with artificially large bounds to disable culling for instanced rendering.
                    // TODO: Use shared mesh with normal bounds once Unity allows for more control over instance culling.
                    quadMesh = quadMeshFilter.mesh;
                    quadMesh.bounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);
                }
                else
                {
                    quadMesh = quadMeshFilter.sharedMesh;
                }

                Destroy(quadMeshFilter.gameObject);
            }

            stopwatch.Reset();
            stopwatch.Start();
        }

        private void Start()
        {
            Reset();
            BuildWindow();
            BuildFrameRateStrings();

#if WINDOWS_UWP
            appCapture = AppCapture.GetForCurrentView();
#endif // WINDOWS_UWP
        }

        private void OnDestroy()
        {
            if (window != null)
            {
                Destroy(window.gameObject);
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.LateUpdate");

        private void LateUpdate()
        {
            if (window == null)
            {
                return;
            }

            using (LateUpdatePerfMarker.Auto())
            {
                // Update window transformation.
                Transform cameraTransform = CameraCache.Main ? CameraCache.Main.transform : null;

                if (ShouldShowProfiler && cameraTransform != null)
                {
                    float t = Time.deltaTime * windowFollowSpeed;
                    window.position = Vector3.Lerp(window.position, CalculateWindowPosition(cameraTransform), t);
                    window.rotation = Quaternion.Slerp(window.rotation, CalculateWindowRotation(cameraTransform), t);
                    window.localScale = defaultWindowScale * windowScale;
                    CalculateBackgroundSize();
                }

                // Capture frame timings every frame and read from it depending on the frameSampleRate.
                FrameTimingManager.CaptureFrameTimings();

                ++frameCount;
                float elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;

                if (elapsedSeconds >= frameSampleRate)
                {
                    int cpuFrameRate = (int)(1.0f / (elapsedSeconds / frameCount));
                    int gpuFrameRate = 0;

                    // Many platforms do not yet support the FrameTimingManager. When timing data is returned from the FrameTimingManager we will use
                    // its timing data, else we will depend on the stopwatch.
                    uint frameTimingsCount = FrameTimingManager.GetLatestTimings((uint)Mathf.Min(frameCount, maxFrameTimings), frameTimings);

                    if (frameTimingsCount != 0)
                    {
                        float cpuFrameTime, gpuFrameTime;
                        AverageFrameTiming(frameTimings, frameTimingsCount, out cpuFrameTime, out gpuFrameTime);
                        cpuFrameRate = (int)(1.0f / (cpuFrameTime / frameCount));
                        gpuFrameRate = (int)(1.0f / (gpuFrameTime / frameCount));
                    }

                    // Update frame rate text.
                    cpuFrameRateText.text = cpuFrameRateStrings[Mathf.Clamp(cpuFrameRate, 0, maxTargetFrameRate)];

                    if (gpuFrameRate != 0)
                    {
                        gpuFrameRateText.gameObject.SetActive(true);
                        gpuFrameRateText.text = gpuFrameRateStrings[Mathf.Clamp(gpuFrameRate, 0, maxTargetFrameRate)];
                    }

                    // Update frame colors.
                    if (frameInfoVisible)
                    {
                        for (int i = frameRange - 1; i > 0; --i)
                        {
                            frameInfoColors[i] = frameInfoColors[i - 1];
                        }

                        frameInfoColors[0] = CalculateFrameColor(cpuFrameRate);
                        frameInfoPropertyBlock.SetVectorArray(colorID, frameInfoColors);
                    }

                    // Reset timers.
                    frameCount = 0;
                    stopwatch.Reset();
                    stopwatch.Start();
                }

                // Draw frame info.
                if (ShouldShowProfiler && frameInfoVisible)
                {
                    Matrix4x4 parentLocalToWorldMatrix = window.localToWorldMatrix;

                    if (defaultInstancedMaterial != null && SystemInfo.supportsInstancing)
                    {
                        frameInfoPropertyBlock.SetMatrix(parentMatrixID, parentLocalToWorldMatrix);
                        Graphics.DrawMeshInstanced(quadMesh, 0, defaultInstancedMaterial, frameInfoMatrices, frameInfoMatrices.Length, frameInfoPropertyBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
                    }
                    else
                    {
                        // If a instanced material is not available or instancing isn't supported, fall back to non-instanced rendering.
                        for (int i = 0; i < frameInfoMatrices.Length; ++i)
                        {
                            frameInfoPropertyBlock.SetColor(colorID, frameInfoColors[i]);
                            Graphics.DrawMesh(quadMesh, parentLocalToWorldMatrix * frameInfoMatrices[i], defaultMaterial, 0, null, 0, frameInfoPropertyBlock, false, false, false);
                        }
                    }
                }

                // Update memory statistics.
                if (ShouldShowProfiler && memoryStatsVisible)
                {
                    ulong limit = AppMemoryUsageLimit;

                    if (limit != limitMemoryUsage)
                    {
                        if (WillDisplayedMemoryUsageDiffer(limitMemoryUsage, limit, displayedDecimalDigits))
                        {
                            MemoryUsageToString(stringBuffer, displayedDecimalDigits, limitMemoryText, limitMemoryString, limit);
                        }

                        limitMemoryUsage = limit;
                    }

                    ulong usage = AppMemoryUsage;

                    if (usage != memoryUsage)
                    {
                        usedAnchor.localScale = new Vector3((float)usage / limitMemoryUsage, usedAnchor.localScale.y, usedAnchor.localScale.z);

                        if (WillDisplayedMemoryUsageDiffer(memoryUsage, usage, displayedDecimalDigits))
                        {
                            MemoryUsageToString(stringBuffer, displayedDecimalDigits, usedMemoryText, usedMemoryString, usage);
                        }

                        memoryUsage = usage;
                    }

                    if (memoryUsage > peakMemoryUsage)
                    {
                        peakAnchor.localScale = new Vector3((float)memoryUsage / limitMemoryUsage, peakAnchor.localScale.y, peakAnchor.localScale.z);

                        if (WillDisplayedMemoryUsageDiffer(peakMemoryUsage, memoryUsage, displayedDecimalDigits))
                        {
                            MemoryUsageToString(stringBuffer, displayedDecimalDigits, peakMemoryText, peakMemoryString, memoryUsage);
                        }

                        peakMemoryUsage = memoryUsage;
                    }
                }

                // Update visibility state.
                window.gameObject.SetActive(ShouldShowProfiler);
                memoryStats.gameObject.SetActive(memoryStatsVisible);
            }
        }

        private static readonly ProfilerMarker CalculateWindowPositionPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.CalculateWindowPosition");

        private Vector3 CalculateWindowPosition(Transform cameraTransform)
        {
            using (CalculateWindowPositionPerfMarker.Auto())
            {
                float windowDistance = Mathf.Max(16.0f / CameraCache.Main.fieldOfView, CameraCache.Main.nearClipPlane + 0.25f);
                Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
                Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
                Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

                switch (windowAnchor)
                {
                    case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
                    case TextAnchor.UpperCenter: position += verticalOffset; break;
                    case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
                    case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
                    case TextAnchor.MiddleRight: position += horizontalOffset; break;
                    case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
                    case TextAnchor.LowerCenter: position -= verticalOffset; break;
                    case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
                }

                return position;
            }
        }

        private static readonly ProfilerMarker CalculateWindowRotationPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.CalculateWindowRotation");

        private Quaternion CalculateWindowRotation(Transform cameraTransform)
        {
            using (CalculateWindowRotationPerfMarker.Auto())
            {
                Quaternion rotation = cameraTransform.rotation;

                switch (windowAnchor)
                {
                    case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
                    case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
                    case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
                    case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
                    case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
                    case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
                    case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
                    case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
                }

                return rotation;
            }
        }

        private static readonly ProfilerMarker CalculateFrameColorPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.CalculateFrameColor");

        private Color CalculateFrameColor(int frameRate)
        {
            using (CalculateFrameColorPerfMarker.Auto())
            {
                // Ideally we would query a device specific API (like the HolographicFramePresentationReport) to detect missed frames.
                // But, many of these APIs are inaccessible in Unity. Currently missed frames are assumed when the average cpuFrameRate 
                // is under the target frame rate.

                int colorCount = frameRateColors.Length;

                if (colorCount == 0)
                {
                    return baseColor;
                }

                float percentageOfTarget = frameRate / AppTargetFrameRate;
                int lastColor = colorCount - 1;

                for (int i = 0; i < lastColor; ++i)
                {
                    if (percentageOfTarget >= frameRateColors[i].percentageOfTarget)
                    {
                        return frameRateColors[i].color;
                    }
                }

                return frameRateColors[lastColor].color;
            }
        }

        private static readonly ProfilerMarker CalculateBackgroundSizePerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.CalculateBackgroundSize");

        private void CalculateBackgroundSize()
        {
            using (CalculateBackgroundSizePerfMarker.Auto())
            {
                if (memoryStatsVisible)
                {
                    background.localPosition = backgroundOffsets[0];
                    background.localScale = backgroundScales[0];
                }
                else if (frameInfoVisible)
                {
                    background.localPosition = backgroundOffsets[1];
                    background.localScale = backgroundScales[1];
                }
                else
                {
                    background.localPosition = backgroundOffsets[2];
                    background.localScale = backgroundScales[2];
                }
            }
        }

        private void BuildWindow()
        {
            // Initialize property block state.
            colorID = Shader.PropertyToID("_Color");
            parentMatrixID = Shader.PropertyToID("_ParentLocalToWorldMatrix");

            // Build the window root.
            {
                window = new GameObject("VisualProfiler").transform;
                window.parent = WindowParent;
                window.localScale = defaultWindowScale;
                windowHorizontalRotation = Quaternion.AngleAxis(defaultWindowRotation.y, Vector3.right);
                windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
                windowVerticalRotation = Quaternion.AngleAxis(defaultWindowRotation.x, Vector3.up);
                windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
            }

            // Build the window background.
            {
                background = CreateQuad("Background", window).transform;
                InitializeRenderer(background.gameObject, backgroundMaterial, colorID, baseColor);
                CalculateBackgroundSize();
            }

            // Add frame rate text and frame indicators.
            {
                cpuFrameRateText = CreateText("CPUFrameRateText", new Vector3(-0.495f, 0.5f, 0.0f), window, TextAnchor.UpperLeft, textMaterial, Color.white, string.Empty);
                gpuFrameRateText = CreateText("GPUFrameRateText", new Vector3(0.495f, 0.5f, 0.0f), window, TextAnchor.UpperRight, textMaterial, Color.white, string.Empty);
                gpuFrameRateText.gameObject.SetActive(false);

                frameInfoMatrices = new Matrix4x4[frameRange];
                frameInfoColors = new Vector4[frameRange];
                Vector3 scale = new Vector3(1.0f / frameRange, 0.2f, 1.0f);
                Vector3 position = new Vector3(0.5f - (scale.x * 0.5f), 0.15f, 0.0f);

                for (int i = 0; i < frameRange; ++i)
                {
                    frameInfoMatrices[i] = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(scale.x * 0.8f, scale.y, scale.z));
                    position.x -= scale.x;
                    frameInfoColors[i] = CalculateFrameColor((int)AppTargetFrameRate);
                }

                frameInfoPropertyBlock = new MaterialPropertyBlock();
                frameInfoPropertyBlock.SetVectorArray(colorID, frameInfoColors);
            }

            // Add memory usage text and bars.
            {
                memoryStats = new GameObject("MemoryStats").transform;
                memoryStats.parent = window;
                memoryStats.localScale = Vector3.one;

                usedMemoryText = CreateText("UsedMemoryText", new Vector3(-0.495f, 0.0f, 0.0f), memoryStats, TextAnchor.UpperLeft, textMaterial, memoryUsedColor, usedMemoryString);
                peakMemoryText = CreateText("PeakMemoryText", new Vector3(0.0f, 0.0f, 0.0f), memoryStats, TextAnchor.UpperCenter, textMaterial, memoryPeakColor, peakMemoryString);
                limitMemoryText = CreateText("LimitMemoryText", new Vector3(0.495f, 0.0f, 0.0f), memoryStats, TextAnchor.UpperRight, textMaterial, Color.white, limitMemoryString);
                voiceCommandText = CreateText("VoiceCommandText", new Vector3(-0.525f, -0.7f, 0.0f), memoryStats, TextAnchor.UpperLeft, textMaterial, Color.white, voiceCommandString);
                mrtkText = CreateText("MRTKText", new Vector3(0.52f, -0.7f, 0.0f), memoryStats, TextAnchor.UpperRight, textMaterial, Color.white, visualProfilerTitleString);
                voiceCommandText.fontSize = 32;
                mrtkText.fontSize = 32;

                GameObject limitBar = CreateQuad("LimitBar", memoryStats);
                InitializeRenderer(limitBar, defaultMaterial, colorID, memoryLimitColor);
                limitBar.transform.localScale = new Vector3(0.99f, 0.2f, 1.0f);
                limitBar.transform.localPosition = new Vector3(0.0f, -0.37f, 0.0f);

                {
                    usedAnchor = CreateAnchor("UsedAnchor", limitBar.transform);
                    GameObject bar = CreateQuad("UsedBar", usedAnchor);
                    Material material = new Material(foregroundMaterial);
                    material.renderQueue += 1;
                    InitializeRenderer(bar, material, colorID, memoryUsedColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
                {
                    peakAnchor = CreateAnchor("PeakAnchor", limitBar.transform);
                    GameObject bar = CreateQuad("PeakBar", peakAnchor);
                    InitializeRenderer(bar, foregroundMaterial, colorID, memoryPeakColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
            }

            window.gameObject.SetActive(ShouldShowProfiler);
            memoryStats.gameObject.SetActive(memoryStatsVisible);
        }

        private void BuildFrameRateStrings()
        {
            cpuFrameRateStrings = new string[maxTargetFrameRate + 1];
            gpuFrameRateStrings = new string[maxTargetFrameRate + 1];
            string displayedDecimalFormat = string.Format("{{0:F{0}}}", displayedDecimalDigits);

            StringBuilder stringBuilder = new StringBuilder(32);
            StringBuilder milisecondStringBuilder = new StringBuilder(16);

            for (int i = 0; i < cpuFrameRateStrings.Length; ++i)
            {
                float miliseconds = (i == 0) ? 0.0f : (1.0f / i) * 1000.0f;
                milisecondStringBuilder.AppendFormat(displayedDecimalFormat, miliseconds);
                stringBuilder.AppendFormat("CPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                cpuFrameRateStrings[i] = stringBuilder.ToString();
                stringBuilder.Length = 0;
                stringBuilder.AppendFormat("GPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                gpuFrameRateStrings[i] = stringBuilder.ToString();
                milisecondStringBuilder.Length = 0;
                stringBuilder.Length = 0;
            }
        }

        private static Transform CreateAnchor(string name, Transform parent)
        {
            Transform anchor = new GameObject(name).transform;
            anchor.parent = parent;
            anchor.localScale = Vector3.one;
            anchor.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);

            return anchor;
        }

        private static GameObject CreateQuad(string name, Transform parent)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.name = name;
            quad.transform.parent = parent;

            return quad;
        }

        private static TextMesh CreateText(string name, Vector3 position, Transform parent, TextAnchor anchor, Material material, Color color, string text)
        {
            GameObject obj = new GameObject(name);
            obj.transform.localScale = Vector3.one * 0.0016f;
            obj.transform.parent = parent;
            obj.transform.localPosition = position;
            TextMesh textMesh = obj.AddComponent<TextMesh>();
            textMesh.fontSize = 48;
            textMesh.anchor = anchor;
            textMesh.color = color;
            textMesh.text = text;
            textMesh.richText = false;

            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            OptimizeRenderer(renderer);

            return textMesh;
        }

        private static Renderer InitializeRenderer(GameObject obj, Material material, int colorID, Color color)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorID, color);
            renderer.SetPropertyBlock(propertyBlock);

            OptimizeRenderer(renderer);

            return renderer;
        }

        private static void OptimizeRenderer(Renderer renderer)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.allowOcclusionWhenDynamic = false;
        }

        private static readonly ProfilerMarker MemoryUsageToStringPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.MemoryUsageToString");

        private static void MemoryUsageToString(char[] stringBuffer, int displayedDecimalDigits, TextMesh textMesh, string prefixString, ulong memoryUsage)
        {
            using (MemoryUsageToStringPerfMarker.Auto())
            {
                // Using a custom number to string method to avoid the overhead, and allocations, of built in string.Format/StringBuilder methods.
                // We can also make some assumptions since the domain of the input number (memoryUsage) is known.
                float memoryUsageMB = ConvertBytesToMegabytes(memoryUsage);
                int memoryUsageIntegerDigits = (int)memoryUsageMB;
                int memoryUsageFractionalDigits = (int)((memoryUsageMB - memoryUsageIntegerDigits) * Mathf.Pow(10.0f, displayedDecimalDigits));
                int bufferIndex = 0;

                for (int i = 0; i < prefixString.Length; ++i)
                {
                    stringBuffer[bufferIndex++] = prefixString[i];
                }

                bufferIndex = MemoryItoA(memoryUsageIntegerDigits, stringBuffer, bufferIndex);
                stringBuffer[bufferIndex++] = '.';

                if (memoryUsageFractionalDigits != 0)
                {
                    bufferIndex = MemoryItoA(memoryUsageFractionalDigits, stringBuffer, bufferIndex);
                }
                else
                {
                    for (int i = 0; i < displayedDecimalDigits; ++i)
                    {
                        stringBuffer[bufferIndex++] = '0';
                    }
                }

                stringBuffer[bufferIndex++] = 'M';
                stringBuffer[bufferIndex++] = 'B';
                textMesh.text = new string(stringBuffer, 0, bufferIndex);
            }
        }

        private static readonly ProfilerMarker MemoryItoAPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.MemoryItoA");

        private static int MemoryItoA(int value, char[] stringBuffer, int bufferIndex)
        {
            using (MemoryItoAPerfMarker.Auto())
            {
                int startIndex = bufferIndex;

                for (; value != 0; value /= 10)
                {
                    stringBuffer[bufferIndex++] = (char)((char)(value % 10) + '0');
                }

                char temp;
                for (int endIndex = bufferIndex - 1; startIndex < endIndex; ++startIndex, --endIndex)
                {
                    temp = stringBuffer[startIndex];
                    stringBuffer[startIndex] = stringBuffer[endIndex];
                    stringBuffer[endIndex] = temp;
                }

                return bufferIndex;
            }
        }

        private static float AppTargetFrameRate
        {
            get
            {
                // If the current XR SDK does not report refresh rate information, assume 60Hz.
                float refreshRate = UnityEngine.XR.XRDevice.refreshRate;
                return ((int)refreshRate == 0) ? 60.0f : refreshRate;
            }
        }

        private static readonly ProfilerMarker AverageFrameTimingPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.AverageFrameTiming");

        private static void AverageFrameTiming(FrameTiming[] frameTimings, uint frameTimingsCount, out float cpuFrameTime, out float gpuFrameTime)
        {
            using (AverageFrameTimingPerfMarker.Auto())
            {
                double cpuTime = 0.0f;
                double gpuTime = 0.0f;

                for (int i = 0; i < frameTimingsCount; ++i)
                {
                    cpuTime += frameTimings[i].cpuFrameTime;
                    gpuTime += frameTimings[i].gpuFrameTime;
                }

                cpuTime /= frameTimingsCount;
                gpuTime /= frameTimingsCount;

                cpuFrameTime = (float)(cpuTime * 0.001);
                gpuFrameTime = (float)(gpuTime * 0.001);
            }
        }

        private static ulong AppMemoryUsage
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsage;
#else
                return (ulong)Profiler.GetTotalAllocatedMemoryLong();
#endif
            }
        }

        private static ulong AppMemoryUsageLimit
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsageLimit;
#else
                return ConvertMegabytesToBytes(SystemInfo.systemMemorySize);
#endif
            }
        }

        private static readonly ProfilerMarker WillDisplayedMemoryUsageDifferPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkitVisualProfiler.WillDisplayedMemoryUsageDiffer");

        private static bool WillDisplayedMemoryUsageDiffer(ulong oldUsage, ulong newUsage, int displayedDecimalDigits)
        {
            using (WillDisplayedMemoryUsageDifferPerfMarker.Auto())
            {
                float oldUsageMBs = ConvertBytesToMegabytes(oldUsage);
                float newUsageMBs = ConvertBytesToMegabytes(newUsage);
                float decimalPower = Mathf.Pow(10.0f, displayedDecimalDigits);

                return (int)(oldUsageMBs * decimalPower) != (int)(newUsageMBs * decimalPower);
            }
        }

        private static ulong ConvertMegabytesToBytes(int megabytes)
        {
            return ((ulong)megabytes * 1024UL) * 1024UL;
        }

        private static float ConvertBytesToMegabytes(ulong bytes)
        {
            return (bytes / 1024.0f) / 1024.0f;
        }
    }
}
