// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="PhraseRecognitionSubsystemDescriptor"/>.
    /// </summary>
    public struct PhraseRecognitionSubsystemCinfo :
        IEquatable<PhraseRecognitionSubsystemCinfo>, IMRTKSubsystemDescriptor
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
        /// Specifies whether the <c>PhraseRecognitionSubsystem</c> is cloud based.
        /// </summary>
        public bool IsCloudBased { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="PhraseRecognitionSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="PhraseRecognitionSubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(PhraseRecognitionSubsystemCinfo other)
        {
            return
                ReferenceEquals(Name, other.Name)
                && ReferenceEquals(ProviderType, other.ProviderType)
                && ReferenceEquals(SubsystemTypeOverride, other.SubsystemTypeOverride)
                && IsCloudBased == other.IsCloudBased;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="PhraseRecognitionSubsystemCinfo"/> and
        /// <see cref="Equals(PhraseRecognitionSubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is PhraseRecognitionSubsystemCinfo) && Equals((PhraseRecognitionSubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(PhraseRecognitionSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(PhraseRecognitionSubsystemCinfo lhs, PhraseRecognitionSubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(PhraseRecognitionSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(PhraseRecognitionSubsystemCinfo lhs, PhraseRecognitionSubsystemCinfo rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            throw new ApplicationException("Do not hash subsystem descriptors as keys.");
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="PhraseRecognitionSubsystem"/> interface.
    /// </summary>
    public class PhraseRecognitionSubsystemDescriptor :
        SubsystemDescriptorWithProvider<PhraseRecognitionSubsystem, PhraseRecognitionSubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>PhraseRecognitionSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='PhraseRecognitionSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        PhraseRecognitionSubsystemDescriptor(PhraseRecognitionSubsystemCinfo PhraseRecognitionSubsystemCinfo)
        {
            Name = PhraseRecognitionSubsystemCinfo.Name;
            DisplayName = PhraseRecognitionSubsystemCinfo.DisplayName;
            Author = PhraseRecognitionSubsystemCinfo.Author;
            ProviderType = PhraseRecognitionSubsystemCinfo.ProviderType;
            SubsystemTypeOverride = PhraseRecognitionSubsystemCinfo.SubsystemTypeOverride;
            IsCloudBased = PhraseRecognitionSubsystemCinfo.IsCloudBased;
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
        /// <exception cref="ArgumentException">Thrown when the values specified in the
        /// <paramref name="PhraseRecognitionSubsystemCinfo"/> parameter are invalid. Typically, this will occur
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.id"/> is <c>null</c> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.implementationType"/> is <c>null</c>
        /// </description>
        /// </item>
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.implementationType"/> does not derive from the
        /// <c>PhraseRecognitionSubsystem</c> class
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
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
