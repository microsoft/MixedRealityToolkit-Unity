// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="TextToSpeechSubsystemDescriptor"/>.
    /// </summary>
    public class TextToSpeechSubsystemCinfo : MRTKSubsystemCinfo
    {
        // TODO: Add subsystem specific properties.

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="MRTKSubsystemCinfo"/> to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="TextToSpeechSubsystem"/>, otherwise <see langword="false"/>.</returns>
        public override bool Equals(MRTKSubsystemCinfo other)
        {
            // TODO: Add comparison of subsystem specific property values.
            return base.Equals(other);
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="TextToSpeechSubsystem"/> interface.
    /// </summary>
    public class TextToSpeechSubsystemDescriptor :
        MRTKSubsystemDescriptor<TextToSpeechSubsystem, TextToSpeechSubsystem.Provider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextToSpeechSubsystemDescriptor"/> class.
        /// </summary>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        TextToSpeechSubsystemDescriptor(TextToSpeechSubsystemCinfo cinfo) : base(cinfo)
        {
            // TODO: Initialize subsystem specific properties.
        }

        // TODO: Add subsystem specific properties.

        /// <summary>
        /// Creates a <see cref="TextToSpeechSubsystemDescriptor"/> based on the given parameters.
        /// </summary>
        /// <remarks>
        /// This function will verify that the <see cref="TextToSpeechSubsystemCinfo"/> properties are valid.
        /// </remarks>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The newly created instance of the <see cref="TextToSpeechSubsystemDescriptor"/> class.
        /// </returns>
        internal static TextToSpeechSubsystemDescriptor Create(TextToSpeechSubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<TextToSpeechSubsystem, TextToSpeechSubsystem.Provider>(
                    cinfo.Name,
                    cinfo.SubsystemTypeOverride,
                    cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create TextToSpeechSubsystemDescriptor.");
            }

            return new TextToSpeechSubsystemDescriptor(cinfo);
        }
    }
}
