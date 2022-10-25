// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="AccessibilitySubsystemDescriptor"/>.
    /// </summary>
    public struct AccessibilitySubsystemCinfo :
        IEquatable<AccessibilitySubsystemCinfo>, IMRTKSubsystemDescriptor
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
        /// <param name="other">The other <see cref="AccessibilitySubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="AccessibilitySubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(AccessibilitySubsystemCinfo other)
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
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref=AccessibilitySubsystemCinfo"/> and
        /// <see cref="Equals(AccessibilitySubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return (obj is AccessibilitySubsystemCinfo cinfo) && Equals(cinfo);
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(AccessibilitySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(AccessibilitySubsystemCinfo lhs, AccessibilitySubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(AccessibilitySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(AccessibilitySubsystemCinfo lhs, AccessibilitySubsystemCinfo rhs)
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
    /// <see cref="AccessibilitySubsystem"/> interface.
    /// </summary>
    public class AccessibilitySubsystemDescriptor :
        SubsystemDescriptorWithProvider<AccessibilitySubsystem, AccessibilitySubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>AccessibilitySubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='AccessibilitySubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        AccessibilitySubsystemDescriptor(AccessibilitySubsystemCinfo accessibilitySubsystemCinfo)
        {
            Name = accessibilitySubsystemCinfo.Name;
            DisplayName = accessibilitySubsystemCinfo.DisplayName;
            Author = accessibilitySubsystemCinfo.Author;
            ProviderType = accessibilitySubsystemCinfo.ProviderType;
            SubsystemTypeOverride = accessibilitySubsystemCinfo.SubsystemTypeOverride;
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
        /// Creates a <c>BoundarySubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>BoundarySubsystemDescriptor</c>.
        /// </returns>
        internal static AccessibilitySubsystemDescriptor Create(AccessibilitySubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<AccessibilitySubsystem, AccessibilitySubsystem.Provider>(cinfo.Name,
                                                                                              cinfo.SubsystemTypeOverride,
                                                                                              cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create AccessibilitySubsystemDescriptor.");
            }

            Debug.Log("Successfully created new descriptor");
            return new AccessibilitySubsystemDescriptor(cinfo);
        }
    }
}
