// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
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

            #region Accessible object management

            /// <summary>
            /// Attempts to get the collection of accessible objects based on the specified constraints.
            /// </summary>
            /// <param name="classifications">The classifications (people, places, things, etc.) of the <see cref="GameObject"/>s to be returned.</param>
            /// <param name="readerView">In how much of the scene should <see cref="GameObject"/>s be returned?</param>
            /// <param name="maxDistance">The cutoff distance beyond which <see cref="GameObject"/>s will not be returned.</param>
            /// <param name="accessibleObjectsList">`Container in which the requested collection of <see cref="GameObject"/>s will be placed.</param>
            /// <returns>True if the collection of (zero or more) accessible objects is being returned, or false.</returns>
            /// <remarks>
            /// When this method returns, the contents of objectList will be cleared and the requested <see cref="GameObject"/>s will be returned.
            /// <para/>
            /// The contents of the objectList collection is indeterminate when this method returns false.
            /// </remarks>
            internal abstract bool TryGetAccessibleObjects(List<AccessibleObjectClassification> classifications, AccessibleObjectVisibility readerView, float maxDistance, List<GameObject> accessibleObjectsList);

            /// <inheritdoc/>
            public abstract bool TryGetAccessibleObjectClassifications(List<AccessibleObjectClassification> classifications);

            /// <inheritdoc/>
            public abstract bool TryRegisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification);

            /// <inheritdoc/>
            public abstract bool TryUnregisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification);

            #endregion Accessible object management

            #region Text color inversion

            /// <inheritdoc/>
            public abstract bool InvertTextColor { get; set; }

            /// <inheritdoc/>
            public abstract event Action<bool> InvertTextColorChanged;

            /// <inheritdoc/>
            public abstract void ApplyTextColorInversion(Material material, bool enable);

            #endregion Text color inversion

            #endregion IAccessibilitySubsystem implementation
        }

        #region IAccessibilitySubsystem implementation

        #region Accessible object management

        /// <summary>
        /// Attempts to get the collection of accessible objects based on the specified constraints.
        /// </summary>
        /// <param name="classifications">The classifications (people, places, things, etc.) of the <see cref="GameObject"/>s to be returned.</param>
        /// <param name="readerView">In how much of the scene should <see cref="GameObject"/>s be returned?</param>
        /// <param name="maxDistance">The cutoff distance beyond which <see cref="GameObject"/>s will not be returned.</param>
        /// <param name="accessibleObjectList">`Container in which the requested collection of <see cref="GameObject"/>s will be placed.</param>
        /// <returns>True if the collection of (zero or more) accessible objects is being returned, or false.</returns>
        /// <remarks>
        /// When this method returns, the contents of objectList will be cleared and the requested <see cref="GameObject"/>s will be returned.
        /// <para/>
        /// The contents of the objectList collection is indeterminate when this method returns false.
        /// </remarks>
        internal bool TryGetAccessibleObjects(List<AccessibleObjectClassification> classifications, AccessibleObjectVisibility readerView, float maxDistance, List<GameObject> accessibleObjectList) =>
            provider.TryGetAccessibleObjects(classifications, readerView, maxDistance, accessibleObjectList);

        /// <inheritdoc/>
        public bool TryGetAccessibleObjectClassifications(List<AccessibleObjectClassification> classifications) =>
            provider.TryGetAccessibleObjectClassifications(classifications);

        /// <inheritdoc/>
        public bool TryRegisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification) =>
            provider.TryRegisterAccessibleObject(accessibleObject, classification);

        /// <inheritdoc/>
        public bool TryUnregisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification) =>
            provider.TryUnregisterAccessibleObject(accessibleObject, classification);

        #endregion Accessible object management

        #region Text color inversion

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

        #endregion Text color inversion

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
