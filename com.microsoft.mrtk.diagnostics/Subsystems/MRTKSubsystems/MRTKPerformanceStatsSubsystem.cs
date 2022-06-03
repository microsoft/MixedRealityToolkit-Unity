// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;

using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

#if WINDOWS_UWP
using Windows.System;
#else
using UnityEngine.Profiling;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Default implementation of the <see cref="PerformanceStatsSubsystem"/>, supporting
    /// performance statistics information.
    /// </summary>
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.toolkit.performancestats",
        DisplayName = "MRTK Performance Stats Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(PerformanceStatsProvider),
        SubsystemTypeOverride = typeof(MRTKPerformanceStatsSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class MRTKPerformanceStatsSubsystem : PerformanceStatsSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<MRTKPerformanceStatsSubsystem, PerformanceStatsSubsystemCinfo>();

            if (!PerformanceStatsSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        class PerformanceStatsProvider : Provider
        {
            private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            /// <inheritdoc/>
            public override void Start()
            {
                stopwatch.Start();
                RamLimit = GetRamLimit();
            }

            /// <inheritdoc/>
            public override void Stop()
            {
                stopwatch.Stop();
            }

            /// <inheritdoc/>
            public override void FixedUpdate()
            {
                UpdateMemoryStatus();
            }

            /// <inheritdoc/>
            public override void LateUpdate()
            {
                UpdateFrameRate();
            }

            #region IPerformanceStatsSubsystem implementation

            /// <inheritdoc/>
            public override ulong RamLimit { get; protected set; } = 0UL;

            /// <inheritdoc/>
            public override ulong AllocatedRam { get; protected set; } = 0UL;

            /// <inheritdoc/>
            public override ulong PeakAllocatedRam { get; protected set; } = 0UL;

            /// <inheritdoc/>
            public override float FrameRate { get; protected set; } = 0f;

            #endregion IPerformanceStatsSubsystem implementation

            // todo: make this configurable
            // [SerializeField]
            // [Tooltip("The time, in seconds, to use when calculating frame rate.")]
            // [Range(0.1f, 2f)]
            private float samplePeriod = 0.1f;

            private int frameCount = 0;

            private static readonly ProfilerMarker UpdateFrameRatePerfMarker =
                new ProfilerMarker("[MRTK] MRTKPerformanceStatsSubsystem.UpdateFrameRate");

            /// <summary>
            /// Updates the current frame rate and raises any appropriate events.
            /// </summary>
            protected virtual void UpdateFrameRate()
            {
                using (UpdateFrameRatePerfMarker.Auto())
                {
                    ++frameCount;

                    // Get the elapsed time, in seconds, since we started the stopwatch.
                    float elapsedTime = stopwatch.ElapsedMilliseconds * 0.001f;

                    // If we have reached the sample period, report
                    // the frame rates.
                    if (elapsedTime >= samplePeriod)
                    {
                        // Calculate the frame rate.
                        FrameRate = 1f / (elapsedTime / frameCount);

                        // Reset state for the next sampling.
                        frameCount = 0;
                        stopwatch.Restart();

                        // todo: raise event(s)
                    }
                }
            }

            /// <summary>
            /// Returns the size, in bytes, of the applications RAM limit.
            /// </summary>
            /// <returns>
            /// The maximum RAM the application is allowed to utilize.
            /// </returns>
            private ulong GetRamLimit()
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsageLimit;
#else
                return MathUtilities.MegabytesToBytes(SystemInfo.systemMemorySize);
#endif // WINDOWS_UWP
            }

            /// <summary>
            /// Updates the memory status and raises any appropriate events.
            /// </summary>
            protected virtual void UpdateMemoryStatus()
            {
#if WINDOWS_UWP
                AllocatedRam = MemoryManager.AppMemoryUsage;
#else
                AllocatedRam = (ulong)Profiler.GetTotalAllocatedMemoryLong();
#endif // WINDOWS_UWP

                if (AllocatedRam > PeakAllocatedRam)
                {
                    PeakAllocatedRam = AllocatedRam;
                }

                // todo: raise event(s)
            }
        }
    }
}