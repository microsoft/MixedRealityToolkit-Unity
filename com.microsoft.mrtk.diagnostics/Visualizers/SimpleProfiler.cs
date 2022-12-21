// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Script to display performance statistics as simple text objects.
    /// </summary>
    [AddComponentMenu("MRTK/Diagnostics/Simple Profiler")]
    public class SimpleProfiler : MonoBehaviour
    {
        #region Frame properties

        [SerializeField]
        [Tooltip("Text Mesh Pro object to use when displaying the frame rate.")]
        private TMP_Text frameRate = null;

        /// <summary>
        /// Text Mesh Pro object to use when displaying the application frame rate.
        /// </summary>
        public TMP_Text FrameRate
        {
            get => frameRate;
            set => frameRate = value;
        }

        [SerializeField]
        [Tooltip("Text Mesh Pro object to use when displaying the display refresh rate.")]
        private TMP_Text refreshRate = null;

        /// <summary>
        /// Text Mesh Pro object to use when displaying the display refresh rate.
        /// </summary>
        public TMP_Text RefreshRate
        {
            get => refreshRate;
            set => refreshRate = value;
        }

        #endregion Frame properties

        #region Memory properties

        [SerializeField]
        [Tooltip("Text Mesh Pro object to use when displaying the RAM addressable by the application.")]
        private TMP_Text ramLimit = null;

        /// <summary>
        /// Text Mesh Pro object to use when displaying the RAM addressable
        /// by the application.
        /// </summary>
        public TMP_Text RamLimit
        {
            get => ramLimit;
            set => ramLimit = value;
        }

        [SerializeField]
        [Tooltip("Text Mesh Pro object to use when displaying the allocated RAM.")]
        private TMP_Text allocatedRam = null;

        /// <summary>
        /// Text Mesh Pro object to use when displaying the allocated RAM.
        /// </summary>
        public TMP_Text AllocatedRam
        {
            get => allocatedRam;
            set => allocatedRam = value;
        }

        [SerializeField]
        [Tooltip("Text Mesh Pro object to use when displaying the peak allocated RAM.")]
        private TMP_Text peakAllocatedRam = null;

        /// <summary>
        /// Text Mesh Pro object to use when displaying the peak allocated RAM.
        /// </summary>
        public TMP_Text PeakAllocatedRam
        {
            get => peakAllocatedRam;
            set => peakAllocatedRam = value;
        }

        #endregion Memory properties

        private const string noData = "--";

        private void Start()
        {
            RamLimit.text = noData;
            AllocatedRam.text = noData;
            PeakAllocatedRam.text = noData;

            FrameRate.text = noData;
            RefreshRate.text = noData;
        }

        private void FixedUpdate()
        {
            if (PerformanceStatsHelpers.Subsystem != null)
            {
                ulong limitBytes = PerformanceStatsHelpers.Subsystem.RamLimit;
                float limitMegabytes = MathUtilities.BytesToMegabytes(limitBytes);
                RamLimit.text = $"{limitBytes} ({limitMegabytes:0.00})";

                ulong allocatedBytes = PerformanceStatsHelpers.Subsystem.AllocatedRam;
                float allocatedMegabytes = MathUtilities.BytesToMegabytes(allocatedBytes);
                AllocatedRam.text = $"{allocatedBytes} ({allocatedMegabytes:0.00})";

                ulong peakBytes = PerformanceStatsHelpers.Subsystem.PeakAllocatedRam;
                float peakMegabytes = MathUtilities.BytesToMegabytes(peakBytes);
                PeakAllocatedRam.text = $"{peakBytes} ({peakMegabytes:0.00})";

                FrameRate.text = PerformanceStatsHelpers.Subsystem.FrameRate.ToString("0.00");
            }

            float refRate = -1f;
            bool? haveRefreshRate = XRSubsystemHelpers.DisplaySubsystem?.TryGetDisplayRefreshRate(out refRate);
            RefreshRate.text = (haveRefreshRate.HasValue && haveRefreshRate.Value) ?
                refRate.ToString("0.00") : noData;
        }
    }
}
