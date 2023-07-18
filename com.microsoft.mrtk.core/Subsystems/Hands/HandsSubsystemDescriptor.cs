// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="HandsSubsystemDescriptor"/>.
    /// </summary>
    public class HandsSubsystemCinfo : MRTKSubsystemCinfo
    {
        /// <summary>
        /// Specifies whether this hands subsystem represents a physical platform-based
        /// data stream, or whether it synthesizes poses from non-hand data.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if non-synthesized, and <see langword="false"/> if synthesized.
        /// </value>
        public bool IsPhysicalData { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="HandsSubsystem"/> to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="HandsSubsystem"/>, otherwise <see langword="false"/>.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            return base.Equals(other) && IsPhysicalData == (other as HandsSubsystemCinfo)?.IsPhysicalData;
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="HandsSubsystem"/> interface.
    /// </summary>
    public class HandsSubsystemDescriptor :
        MRTKSubsystemDescriptor<HandsSubsystem, HandsSubsystem.Provider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandsSubsystemDescriptor"/> class.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        HandsSubsystemDescriptor(HandsSubsystemCinfo cinfo) : base(cinfo)
        {
            IsPhysicalData = cinfo.IsPhysicalData;
        }

        /// <summary>
        /// Specifies whether this hands subsystem represents a physical platform-based
        /// data stream, or whether it synthesizes poses from non-hand data.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if non-synthesized, and <see langword="false"/> if synthesized.
        /// </value>
        public bool IsPhysicalData { get; set; }

        /// <summary>
        /// Creates a <see cref="HandsSubsystemDescriptor"/> based on the given parameters.
        /// </summary>
        /// <remarks>
        /// This function will verify that the <see cref="HandsSubsystemCinfo"/> properties are valid.
        /// </remarks>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The newly created instance of the <see cref="HandsSubsystemDescriptor"/> class.
        /// </returns>
        internal static HandsSubsystemDescriptor Create(HandsSubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<HandsSubsystem, HandsSubsystem.Provider>(cinfo.Name,
                                                                                        cinfo.SubsystemTypeOverride,
                                                                                        cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create HandsSubsystemDescriptor.");
            }

            return new HandsSubsystemDescriptor(cinfo);
        }
    }
}
