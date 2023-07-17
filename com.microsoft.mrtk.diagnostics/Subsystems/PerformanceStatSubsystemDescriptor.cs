// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="PerformanceStatsSubsystemDescriptor"/>.
    /// </summary>
    public struct PerformanceStatsSubsystemCinfo :
        IEquatable<PerformanceStatsSubsystemCinfo>,
        IMRTKSubsystemDescriptor
    {
        #region IMRTKDescriptor implementation

        ///<inheritdoc/>
        public string Name { get; set; }

        ///<inheritdoc/>
        public string DisplayName { get; set; }

        ///<inheritdoc/>
        public string Author { get; set; }

        ///<inheritdoc/>
        public Type ConfigType { get; set; }

        ///<inheritdoc/>
        public Type ProviderType { get; set; }

        ///<inheritdoc/>
        public Type SubsystemTypeOverride { get; set; }

        #endregion IMRTKDescriptor implementation

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">
        /// The other <see cref="PerformanceStatsSubsystemCinfo"/> to compare against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="PerformanceStatsSubsystemCinfo"/>, otherwise false.
        /// </returns>
        public bool Equals(PerformanceStatsSubsystemCinfo other)
        {
            return
                ReferenceEquals(Name, other.Name)
                && ReferenceEquals(ProviderType, other.ProviderType)
                && ReferenceEquals(SubsystemTypeOverride, other.SubsystemTypeOverride);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="PerformanceStatsSubsystemCinfo"/> and
        /// <see cref="Equals(PerformanceStatsSubsystemCinfo)"/> also returns <see langword="true"/>; otherwise <see langword="false"/>.</returns>
        public override bool Equals(System.Object obj)
        {
            return (obj is PerformanceStatsSubsystemCinfo cinfo) && Equals(cinfo);
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(PerformanceStatsSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator ==(PerformanceStatsSubsystemCinfo lhs, PerformanceStatsSubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(PerformanceStatsSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator !=(PerformanceStatsSubsystemCinfo lhs, PerformanceStatsSubsystemCinfo rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// This <see cref="GetHashCode"/> override is meant to disable hash lookups of <see cref="PerformanceStatsSubsystemCinfo"/> objects.
        /// </summary>
        /// <remarks>
        /// This will throw a <see cref="ApplicationException"/> if called.
        /// </remarks>
        /// <exception cref="ApplicationException">
        /// Thrown if this function is called.
        /// </exception>
        public override int GetHashCode()
        {
            throw new ApplicationException("Do not hash subsystem descriptors as keys.");
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="PerformanceStatsSubsystem"/> interface.
    /// </summary>
    public class PerformanceStatsSubsystemDescriptor :
        SubsystemDescriptorWithProvider<PerformanceStatsSubsystem, PerformanceStatsSubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceStatsSubsystemDescriptor"/> class.
        /// </summary>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        PerformanceStatsSubsystemDescriptor(PerformanceStatsSubsystemCinfo cinfo)
        {
            Name = cinfo.Name;
            DisplayName = cinfo.DisplayName;
            Author = cinfo.Author;
            ProviderType = cinfo.ProviderType;
            SubsystemTypeOverride = cinfo.SubsystemTypeOverride;
        }

        #region IMRTKDescriptor implementation

        ///<inheritdoc/>
        public string Name { get => id; set => id = value; }

        ///<inheritdoc/>
        public string DisplayName { get; set; }

        ///<inheritdoc/>
        public string Author { get; set; }

        ///<inheritdoc/>
        public Type ConfigType { get; set; }

        ///<inheritdoc/>
        public Type ProviderType { get => providerType; set => providerType = value; }

        ///<inheritdoc/>
        public Type SubsystemTypeOverride { get => subsystemTypeOverride; set => subsystemTypeOverride = value; }

        #endregion IMRTKDescriptor implementation

        /// <summary>
        /// Creates a <see cref="PerformanceStatsSubsystemDescriptor"/> based on the given parameters.
        /// </summary>
        /// <remarks>
        /// This function will verify that the <see cref="PerformanceStatsSubsystemCinfo"/> properties are valid.
        /// </remarks>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The newly created instance of the <see cref="PerformanceStatsSubsystemDescriptor"/> class.
        /// </returns>
        internal static PerformanceStatsSubsystemDescriptor Create(PerformanceStatsSubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<PerformanceStatsSubsystem, PerformanceStatsSubsystem.Provider>(cinfo.Name,
                                                                                                              cinfo.SubsystemTypeOverride,
                                                                                                              cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create BoundarySubsystemDescriptor.");
            }

            return new PerformanceStatsSubsystemDescriptor(cinfo);
        }
    }
}
