// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="HandsAggregatorSubsystemDescriptor"/>.
    /// </summary>
    public struct HandsAggregatorSubsystemCinfo :
        IEquatable<HandsAggregatorSubsystemCinfo>, IMRTKSubsystemDescriptor
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
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="HandsAggregatorSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="HandsAggregatorSubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(HandsAggregatorSubsystemCinfo other)
        {
            return
                ReferenceEquals(Name, other.Name)
                && ReferenceEquals(ProviderType, other.ProviderType)
                && ReferenceEquals(SubsystemTypeOverride, other.SubsystemTypeOverride);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="HandsAggregatorSubsystemCinfo"/> and
        /// <see cref="Equals(HandsAggregatorSubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is HandsAggregatorSubsystemCinfo) && Equals((HandsAggregatorSubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(HandsAggregatorSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(HandsAggregatorSubsystemCinfo lhs, HandsAggregatorSubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(HandsAggregatorSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(HandsAggregatorSubsystemCinfo lhs, HandsAggregatorSubsystemCinfo rhs)
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
    /// <see cref="HandsAggregatorSubsystem"/> interface.
    /// </summary>
    public class HandsAggregatorSubsystemDescriptor :
        SubsystemDescriptorWithProvider<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>HandsAggregatorSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='HandsAggregatorSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        HandsAggregatorSubsystemDescriptor(HandsAggregatorSubsystemCinfo HandsAggregatorSubsystemCinfo)
        {
            Name = HandsAggregatorSubsystemCinfo.Name;
            DisplayName = HandsAggregatorSubsystemCinfo.DisplayName;
            Author = HandsAggregatorSubsystemCinfo.Author;
            ConfigType = HandsAggregatorSubsystemCinfo.ConfigType;
            ProviderType = HandsAggregatorSubsystemCinfo.ProviderType;
            SubsystemTypeOverride = HandsAggregatorSubsystemCinfo.SubsystemTypeOverride;
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
        /// Creates a <c>HandsAggregatorSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>HandsAggregatorSubsystemDescriptor</c>.
        /// </returns>
        internal static HandsAggregatorSubsystemDescriptor Create(HandsAggregatorSubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>(cinfo.Name,
                                                                                                            cinfo.SubsystemTypeOverride,
                                                                                                            cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create HandsAggregatorSubsystemDescriptor.");
            }

            return new HandsAggregatorSubsystemDescriptor(cinfo);
        }
    }
}
