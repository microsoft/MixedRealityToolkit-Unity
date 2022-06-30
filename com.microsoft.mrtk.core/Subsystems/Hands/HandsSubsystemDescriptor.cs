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
        /// True if non-synthesized. False if synthesized.
        /// </value>
        public bool IsPhysicalData { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="HandsSubsystem"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="HandsSubsystem"/>, otherwise false.</returns>
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
        /// Constructs a <c>HandsSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='HandsSubsystem'>The parameters required to initialize the descriptor.</param>
        HandsSubsystemDescriptor(HandsSubsystemCinfo cinfo) : base(cinfo)
        {
            IsPhysicalData = cinfo.IsPhysicalData;
        }

        /// <summary>
        /// Specifies whether this hands subsystem represents a physical platform-based
        /// data stream, or whether it synthesizes poses from non-hand data.
        /// </summary>
        /// <value>
        /// True if non-synthesized. False if synthesized.
        /// </value>
        public bool IsPhysicalData { get; set; }

        /// <summary>
        /// Creates a <c>HandsSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>HandsSubsystemDescriptor</c>.
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
