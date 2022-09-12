// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Classifications for objects that may appear in the scene.
    /// </summary>
    [Flags]
    public enum ObjectClassification : ushort
    {
        /// <summary>
        /// The object is a person.
        /// </summary>
        People = 1 << 0,

        /// <summary>
        /// The object is a location (ex: an adjacent room).
        /// </summary>
        Places = 1 << 1,

        /// <summary>
        /// The object is interactable and not a person, place or user interface component.
        /// </summary>
        Things = 1 << 2,

        /// <summary>
        /// The object is a user interface component.
        /// </summary>
        UserInterface = 1 << 3,

        /// <summary>
        /// The object is part of the scene background and is, generally,
        /// not interactable.
        /// </summary>
        Background = 1 << 15,

    }
}
