// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="PhraseRecognitionSubsystemDescriptor"/>.
    /// </summary>
    public class PhraseRecognitionSubsystemCinfo : MRTKSubsystemCinfo
    {
        /// <summary>
        /// Specifies whether the <c>PhraseRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="PhraseRecognitionSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="PhraseRecognitionSubsystemCinfo"/>, otherwise false.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            return base.Equals(other) && IsCloudBased == (other as PhraseRecognitionSubsystemCinfo)?.IsCloudBased;
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="PhraseRecognitionSubsystem"/> interface.
    /// </summary>
    public class PhraseRecognitionSubsystemDescriptor :
        MRTKSubsystemDescriptor<PhraseRecognitionSubsystem, PhraseRecognitionSubsystem.Provider>
    {
        /// <summary>
        /// Constructs a <c>PhraseRecognitionSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='PhraseRecognitionSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        PhraseRecognitionSubsystemDescriptor(PhraseRecognitionSubsystemCinfo cinfo) : base(cinfo)
        {
            IsCloudBased = cinfo.IsCloudBased;
        }

        /// <summary>
        /// Specifies whether the <c>PhraseRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Creates a <c>PhraseRecognitionSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='PhraseRecognitionSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>PhraseRecognitionSubsystemDescriptor</c>.
        /// </returns>
        internal static PhraseRecognitionSubsystemDescriptor Create(PhraseRecognitionSubsystemCinfo cinfo)
        {
           // Validates cinfo.
           if (!XRSubsystemHelpers.CheckTypes<PhraseRecognitionSubsystem, PhraseRecognitionSubsystem.Provider>(cinfo.Name,
                                                                                                               cinfo.SubsystemTypeOverride,
                                                                                                               cinfo.ProviderType))
           {
               throw new ArgumentException("Could not create PhraseRecognitionSubsystemDescriptor.");
           }

           return new PhraseRecognitionSubsystemDescriptor(cinfo);
        }
    }
}
