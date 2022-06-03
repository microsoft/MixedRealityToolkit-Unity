// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Cross-platform, portable set of specifications for what
    /// a PerformanceStats Subsystem is capable of.
    /// Both the subsystem and the associated provider must implement
    /// this interface, preferably with a direct mapping or wrapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    internal interface IPerformanceStatsSubsystem
    {
        #region Memory statistics

        /// <summary>
        /// The maximum RAM that the application is allowed to allocate.
        /// </summary>
        ulong RamLimit { get; }

        /// <summary>
        /// The current amount of RAM that the application has allocated.
        /// </summary>
        ulong AllocatedRam { get; }

        /// <summary>
        /// The largest amount of RAM that the application has allocated
        /// during the session.
        /// </summary>
        ulong PeakAllocatedRam { get; }

        #endregion Memory statistics

        #region Frame statistics

        /// <summary>
        /// The number of frames displayed in a single second over
        /// the sampling period.
        /// </summary>
        float FrameRate { get; }

        #endregion Frame statistics
    }
}