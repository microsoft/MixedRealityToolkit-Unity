// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="SpeechRecognitionSubsystemDescriptor"/>.
    /// </summary>
    public class SpeechRecognitionSubsystemCinfo : MRTKSubsystemCinfo
    {
        /// <summary>
        /// Specifies whether the <c>SpeechRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="SpeechRecognitionSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="SpeechRecognitionSubsystemCinfo"/>, otherwise false.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            return base.Equals(other) && IsCloudBased == (other as SpeechRecognitionSubsystemCinfo)?.IsCloudBased;
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="SpeechRecognitionSubsystem"/> interface.
    /// </summary>
    public class SpeechRecognitionSubsystemDescriptor :
        MRTKSubsystemDescriptor<SpeechRecognitionSubsystem, SpeechRecognitionSubsystem.Provider>
    {
        /// <summary>
        /// Constructs a <c>SpeechRecognitionSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        SpeechRecognitionSubsystemDescriptor(SpeechRecognitionSubsystemCinfo cinfo) : base(cinfo)
        {
            IsCloudBased = cinfo.IsCloudBased;
        }

        /// <summary>
        /// Specifies whether the <c>SpeechRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Creates a <c>SpeechRecognitionSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>SpeechRecognitionSubsystemDescriptor</c>.
        /// </returns>
        internal static SpeechRecognitionSubsystemDescriptor Create(SpeechRecognitionSubsystemCinfo cinfo)
        {
           // Validates cinfo.
           if (!XRSubsystemHelpers.CheckTypes<SpeechRecognitionSubsystem, SpeechRecognitionSubsystem.Provider>(cinfo.Name,
                                                                                                               cinfo.SubsystemTypeOverride,
                                                                                                               cinfo.ProviderType))
           {
               throw new ArgumentException("Could not create SpeechRecognitionSubsystemDescriptor.");
           }

           return new SpeechRecognitionSubsystemDescriptor(cinfo);
        }
    }
}
