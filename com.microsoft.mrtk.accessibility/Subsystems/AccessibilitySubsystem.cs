// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    [Preserve]
    /// <summary>
    /// A subsystem that exposes information about the current enabled assistive technologies.
    /// </summary>
    public class AccessibilitySubsystem :
        MRTKSubsystem<AccessibilitySubsystem, AccessibilitySubsystemDescriptor, AccessibilitySubsystem.Provider>,
        IAccessibilitySubsystem
    {
        /// <summary>
        /// Construct the <c>AccessibilitySubsystem</c>.
        /// </summary>
        public AccessibilitySubsystem()
        { }

        /// <summary>
        /// Interface for providing accessibility functionality for the implementation.
        /// </summary>
        [Preserve]
        public abstract class Provider :
            MRTKSubsystemProvider<AccessibilitySubsystem>,
            IAccessibilitySubsystem
        {
            #region IAccessibilitySubsystem implementation

            /// <inheritdoc/>
            public abstract bool InvertTextColor { get; set; }

            /// <inheritdoc/>
            public abstract event Action<bool> InvertTextColorChanged;

            /// <inheritdoc/>
            public abstract void ApplyTextColorInversion(Material material, bool enable);

            #endregion IAccessibilitySubsystem implementation
        }

        #region IAccessibilitySubsystem implementation

        /// <inheritdoc/>
        public bool InvertTextColor
        {
            get => provider.InvertTextColor;
            set => provider.InvertTextColor = value;
        }

        /// <inheritdoc/>
        public event Action<bool> InvertTextColorChanged
        {
            add => provider.InvertTextColorChanged += value;
            remove => provider.InvertTextColorChanged -= value;
        }

        /// <inheritdoc/>
        public void ApplyTextColorInversion(
            Material material,
            bool enable) => provider.ApplyTextColorInversion(material, enable);

        #endregion IAccessibilitySubsystem implementation

        /// <summary>
        /// Registers a accessibility subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="accessibilitySubsystemParams">The parameters defining the accessibility subsystem
        /// functionality implemented by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the values specified in the
        /// <see cref="AccessibilitySubsystemCinfo"/> parameter are invalid. Typically, this will occur
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="AccessibilitySubsystemCinfo.id"/> is <c>null</c> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="AccessibilitySubsystemCinfo.implementationType"/> is <c>null</c></description>
        /// </item>
        /// <item>
        /// <description>if <see cref="AccessibilitySubsystemCinfo.implementationType"/> does not derive from the
        /// <see cref="AccessibilitySubsystem"/> class
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
        public static bool Register(AccessibilitySubsystemCinfo accessibilitySubsystemParams)
        {
            AccessibilitySubsystemDescriptor descriptor = AccessibilitySubsystemDescriptor.Create(accessibilitySubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }
}
