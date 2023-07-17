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
        /// Specifies whether the <see cref="DictationSubsystemCinfo"/> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="DictationSubsystemCinfo"/> to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="DictationSubsystemCinfo"/>, otherwise <see langword="false"/>.</returns>
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
        /// Initializes a new instance of the <see cref="DictationSubsystemDescriptor"/> class.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        DictationSubsystemDescriptor(DictationSubsystemCinfo cinfo) : base(cinfo)
        {
            IsCloudBased = cinfo.IsCloudBased;
        }

        /// <summary>
        /// Specifies whether the  <see cref="DictationSubsystem"/> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Creates a <see cref="DictationSubsystemDescriptor"/> based on the given parameters.
        /// </summary>
        /// <remarks>
        /// This function will verify that the <see cref="DictationSubsystemCinfo"/> properties are valid.
        /// </remarks>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The newly created instance of the <see cref="DictationSubsystemDescriptor"/> class.
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
