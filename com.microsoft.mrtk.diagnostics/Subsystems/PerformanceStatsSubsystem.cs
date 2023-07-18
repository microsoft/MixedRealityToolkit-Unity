// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// A subsystem that exposes performance statistics (ex: memory usage, frame rate).
    /// </summary>
    public class PerformanceStatsSubsystem :
        MRTKSubsystem<PerformanceStatsSubsystem, PerformanceStatsSubsystemDescriptor, PerformanceStatsSubsystem.Provider>,
        IPerformanceStatsSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceStatsSubsystem"/> class.
        /// </summary>
        public PerformanceStatsSubsystem()
        { }

        /// <summary>
        /// Interface for providing hardware status functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<PerformanceStatsSubsystem>, IPerformanceStatsSubsystem
        {
            #region IPerformanceStatsSubsystem implementation

            /// <inheritdoc/>
            public abstract ulong RamLimit { get; protected set; }

            /// <inheritdoc/>
            public abstract ulong AllocatedRam { get; protected set; }

            /// <inheritdoc/>
            public abstract ulong PeakAllocatedRam { get; protected set; }

            /// <inheritdoc/>
            public abstract float FrameRate { get; protected set; }

            #endregion IPerformanceStatsSubsystem implementation
        }

        #region PerformanceStatsSubsystem implementation

        /// <inheritdoc/>
        public ulong RamLimit => provider.RamLimit;

        /// <inheritdoc/>
        public ulong AllocatedRam => provider.AllocatedRam;

        /// <inheritdoc/>
        public ulong PeakAllocatedRam => provider.PeakAllocatedRam;

        /// <inheritdoc/>
        public float FrameRate => provider.FrameRate;

        #endregion PerformanceStatsSubsystem implementation

        /// <summary>
        /// Registers a PerformanceStats subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="performanceStatsSubsystemParams">The parameters defining the PerformanceStats subsystem
        /// functionality implemented by the subsystem provider.</param>
        /// <returns>
        /// <see langword="true"/> if the subsystem implementation is registered. Otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the values specified in the
        /// <see cref="PerformanceStatsSubsystemCinfo"/> parameter are invalid. Typically, this will occur in the following cases:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The <see cref="PerformanceStatsSubsystemCinfo.Name"/> is <see langword="null"/>  or empty</description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="PerformanceStatsSubsystemCinfo.ProviderType"/> is <see langword="null"/></description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="PerformanceStatsSubsystemCinfo.ProviderType"/> does not derive from the <see cref="PerformanceStatsSubsystem.Provider"/> type.</description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="PerformanceStatsSubsystemCinfo.SubsystemTypeOverride"/> does not derive from the <see cref="PerformanceStatsSubsystem"/> type.</description>
        ///     </item>
        /// </list>
        /// </exception>
        public static bool Register(PerformanceStatsSubsystemCinfo performanceStatsSubsystemParams)
        {
            PerformanceStatsSubsystemDescriptor descriptor = PerformanceStatsSubsystemDescriptor.Create(performanceStatsSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }
}
