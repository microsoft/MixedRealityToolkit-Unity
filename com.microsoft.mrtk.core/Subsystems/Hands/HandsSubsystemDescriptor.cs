// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="HandsSubsystemDescriptor"/>.
    /// </summary>
    public struct HandsSubsystemCinfo :
        IEquatable<HandsSubsystemCinfo>, IMRTKSubsystemDescriptor
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
        public bool Equals(HandsSubsystemCinfo other)
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
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="HandsSubsystem"/> and
        /// <see cref="Equals(HandsSubsystem)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is HandsSubsystemCinfo) && Equals((HandsSubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(HandsSubsystem)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(HandsSubsystemCinfo lhs, HandsSubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(HandsSubsystem)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(HandsSubsystemCinfo lhs, HandsSubsystemCinfo rhs)
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
    /// <see cref="HandsSubsystem"/> interface.
    /// </summary>
    public class HandsSubsystemDescriptor :
        SubsystemDescriptorWithProvider<HandsSubsystem, HandsSubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>HandsSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='HandsSubsystem'>The parameters required to initialize the descriptor.</param>
        HandsSubsystemDescriptor(HandsSubsystemCinfo handsSubsystemCinfo)
        {
            Name = handsSubsystemCinfo.Name;
            DisplayName = handsSubsystemCinfo.DisplayName;
            Author = handsSubsystemCinfo.Author;
            ConfigType = handsSubsystemCinfo.ConfigType;
            ProviderType = handsSubsystemCinfo.ProviderType;
            SubsystemTypeOverride = handsSubsystemCinfo.SubsystemTypeOverride;
            IsPhysicalData = handsSubsystemCinfo.IsPhysicalData;
        }

        #region IMRTKDescriptor implementation

        ///<inheritdoc/>
        public string Name { get => id; set => id = value; }

        ///<inheritdoc/>
        public string DisplayName { get; set; }

        ///<inheritdoc/>
        public string Author { get; set; }

        ///<inheritdoc/>
        public Type ProviderType { get => providerType; set => providerType = value; }

        ///<inheritdoc/>
        public Type SubsystemTypeOverride { get => subsystemTypeOverride; set => subsystemTypeOverride = value; }

        ///<inheritdoc/>
        public Type ConfigType { get; set; }

        #endregion IMRTKDescriptor implementation

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
