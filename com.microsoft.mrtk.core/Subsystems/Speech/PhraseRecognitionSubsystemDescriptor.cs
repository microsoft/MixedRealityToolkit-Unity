// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="KeywordRecognitionSubsystemDescriptor"/>.
    /// </summary>
    public class KeywordRecognitionSubsystemCinfo : MRTKSubsystemCinfo
    {
        /// <summary>
        /// Specifies whether the <c>KeywordRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="KeywordRecognitionSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="KeywordRecognitionSubsystemCinfo"/>, otherwise false.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            return base.Equals(other) && IsCloudBased == (other as KeywordRecognitionSubsystemCinfo)?.IsCloudBased;
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="KeywordRecognitionSubsystem"/> interface.
    /// </summary>
    public class KeywordRecognitionSubsystemDescriptor :
        MRTKSubsystemDescriptor<KeywordRecognitionSubsystem, KeywordRecognitionSubsystem.Provider>
    {
        /// <summary>
        /// Constructs a <c>KeywordRecognitionSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='KeywordRecognitionSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        KeywordRecognitionSubsystemDescriptor(KeywordRecognitionSubsystemCinfo cinfo) : base(cinfo)
        {
            IsCloudBased = cinfo.IsCloudBased;
        }

        /// <summary>
        /// Specifies whether the <c>KeywordRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Creates a <c>KeywordRecognitionSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='KeywordRecognitionSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>KeywordRecognitionSubsystemDescriptor</c>.
        /// </returns>
        internal static KeywordRecognitionSubsystemDescriptor Create(KeywordRecognitionSubsystemCinfo cinfo)
        {
           // Validates cinfo.
           if (!XRSubsystemHelpers.CheckTypes<KeywordRecognitionSubsystem, KeywordRecognitionSubsystem.Provider>(cinfo.Name,
                                                                                                               cinfo.SubsystemTypeOverride,
                                                                                                               cinfo.ProviderType))
           {
               throw new ArgumentException("Could not create KeywordRecognitionSubsystemDescriptor.");
           }

           return new KeywordRecognitionSubsystemDescriptor(cinfo);
        }
    }
}
