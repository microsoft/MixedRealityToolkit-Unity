// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="InteractionPolyfillSubsystemDescriptor"/>.
    /// </summary>
    public struct InteractionPolyfillSubsystemCinfo :
        IEquatable<InteractionPolyfillSubsystemCinfo>, IMRTKSubsystemDescriptor
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
        /// <param name="other">The other <see cref="InteractionPolyfillSubsystem"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="InteractionPolyfillSubsystem"/>, otherwise false.</returns>
        public bool Equals(InteractionPolyfillSubsystemCinfo other)
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
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="InteractionPolyfillSubsystem"/> and
        /// <see cref="Equals(InteractionPolyfillSubsystem)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is InteractionPolyfillSubsystemCinfo) && Equals((InteractionPolyfillSubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(InteractionPolyfillSubsystem)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(InteractionPolyfillSubsystemCinfo lhs, InteractionPolyfillSubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(InteractionPolyfillSubsystem)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(InteractionPolyfillSubsystemCinfo lhs, InteractionPolyfillSubsystemCinfo rhs)
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
    /// <see cref="InteractionPolyfillSubsystem"/> interface.
    /// </summary>
    public class InteractionPolyfillSubsystemDescriptor :
        SubsystemDescriptorWithProvider<InteractionPolyfillSubsystem, InteractionPolyfillSubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>InteractionPolyfillSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='InteractionPolyfillSubsystem'>The parameters required to initialize the descriptor.</param>
        public InteractionPolyfillSubsystemDescriptor(InteractionPolyfillSubsystemCinfo cinfo)
        {
            Name = cinfo.Name;
            DisplayName = cinfo.DisplayName;
            Author = cinfo.Author;
            ConfigType = cinfo.ConfigType;
            ProviderType = cinfo.ProviderType;
            SubsystemTypeOverride = cinfo.SubsystemTypeOverride;
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
        /// Creates a <c>InteractionPolyfillSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>InteractionPolyfillSubsystemDescriptor</c>.
        /// </returns>
        internal static InteractionPolyfillSubsystemDescriptor Create(InteractionPolyfillSubsystemCinfo cinfo)
        {
            UnityEngine.Debug.Log("Descriptor Create()");
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<InteractionPolyfillSubsystem, InteractionPolyfillSubsystem.Provider>(cinfo.Name,
                                                                                        cinfo.SubsystemTypeOverride,
                                                                                        cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create InteractionPolyfillSubsystemDescriptor.");
            }

            return new InteractionPolyfillSubsystemDescriptor(cinfo);
        }
    }
}
