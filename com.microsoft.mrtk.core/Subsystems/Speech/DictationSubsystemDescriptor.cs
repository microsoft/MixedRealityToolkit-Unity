// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="DictationSubsystemDescriptor"/>.
    /// </summary>
    public class DictationSubsystemCinfo : MRTKSubsystemCinfo
    {
        /// <summary>
        /// Specifies whether the <c>DictationSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="DictationSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="DictationSubsystemCinfo"/>, otherwise false.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            return base.Equals(other) && IsCloudBased == (other as DictationSubsystemCinfo)?.IsCloudBased;
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="DictationSubsystem"/> interface.
    /// </summary>
    public class DictationSubsystemDescriptor :
        MRTKSubsystemDescriptor<DictationSubsystem, DictationSubsystem.Provider>
    {
        /// <summary>
        /// Constructs a <c>DictationSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        DictationSubsystemDescriptor(DictationSubsystemCinfo cinfo) : base(cinfo)
        {
            IsCloudBased = cinfo.IsCloudBased;
        }

        /// <summary>
        /// Specifies whether the <c>DictationSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Creates a <c>DictationSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>DictationSubsystemDescriptor</c>.
        /// </returns>
        internal static DictationSubsystemDescriptor Create(DictationSubsystemCinfo cinfo)
        {
           // Validates cinfo.
           if (!XRSubsystemHelpers.CheckTypes<DictationSubsystem, DictationSubsystem.Provider>(cinfo.Name,
                                                                                                               cinfo.SubsystemTypeOverride,
                                                                                                               cinfo.ProviderType))
           {
               throw new ArgumentException("Could not create DictationSubsystemDescriptor.");
           }

           return new DictationSubsystemDescriptor(cinfo);
        }
    }
}
