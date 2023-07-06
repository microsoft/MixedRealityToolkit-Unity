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
        /// <param name="other">The other <see cref="MicrosoftTextToSpeechSubsystem"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="TextToSpeechSubsystem"/>, otherwise false.</returns>
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
        /// Constructs a <c>TextToSpeechSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        TextToSpeechSubsystemDescriptor(TextToSpeechSubsystemCinfo cinfo) : base(cinfo)
        {
            // TODO: Initialize subsystem specific properties.
        }

        // TODO: Add subsystem specific properties.

        /// <summary>
        /// Creates a <c>TextToSpeechSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>TextToSpeechSubsystemDescriptor</c>.
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
